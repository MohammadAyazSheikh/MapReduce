using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_Reduce
{
   


   
    public partial class MainForm : Form
    {

      
        
     

        //The collection of all clients logged into the room (an array of type ClientInfo)
        ArrayList clientList;

        //The main socket on which the server listens to the clients
        Socket serverSocket;

        //client id
        int id = 0;

        List<int> price_List = new List<int>();

        byte[] byteData = new byte[1024];
        public MainForm()
        {
            clientList = new ArrayList();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Start_Server();
        }

        void Start_Server()
        {
            try
            {
                //We are using TCP sockets
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Assign the any IP of the machine and listen on port number 1000
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1000);

                //Bind and listen on the given address
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(50);

                //Accept the incoming clients
                serverSocket.BeginAccept(new AsyncCallback(Accept_Callback), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Accept_Callback(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAccept(ar);

                //Start listening for more clients
                serverSocket.BeginAccept(new AsyncCallback(Accept_Callback), null);

                //Once the client connects then start receiving the commands from her
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,new AsyncCallback(Receive_Callback), clientSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error",   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Receive_Callback(IAsyncResult ar)
        {
            try
            {
                Socket Current_ClientSocket = (Socket)ar.AsyncState;
                Current_ClientSocket.EndReceive(ar);



                //Transform the array of bytes received from the user into an
                //intelligent form of object Data
                Data msgReceived = new Data(byteData);
              
              

                //We will send this object in response the users request
                Data msgToSend = new Data();

                byte[] message;
                

                //If the message is to login, logout, or simple text message
                //then when send to others the type of the message remains the same
                msgToSend.cmd = msgReceived.cmd;
                msgToSend.Name = msgReceived.Name;

                switch (msgReceived.cmd)
                {
                    case Command.Login:

                        //When a user logs in to the server then we add her to our
                        //list of clients
                        id++;
                        Client_Info clientInfo = new Client_Info();
                        clientInfo.socket = Current_ClientSocket;
                        clientInfo.strName = msgReceived.Name;
                        clientInfo.id = id;
                        clientInfo.flag = false;

                        clientList.Add(clientInfo);
                        
                        //Set the text of the message that we will broadcast to all users
                        msgToSend.Message = "<<<" + msgReceived.Name + " has joined the room>>>";
                        break;

                    case Command.Logout:

                        //When a user wants to log out of the server then we search for her 
                        //in the list of clients and close the corresponding connection

                        int nIndex = 0;
                        foreach (Client_Info client in clientList)
                        {
                            if (client.socket == Current_ClientSocket)
                            {
                                clientList.RemoveAt(nIndex);
                                break;
                            }
                            ++nIndex;
                        }

                        Current_ClientSocket.Close();

                        msgToSend.Message = "<<<" + msgReceived.Name + " has left the room>>>";
                        break;

                    case Command.Message:

                        //Set the text of the message that we will broadcast to all users
                        msgToSend.Message = msgReceived.Name + ": " + msgReceived.Message;
                        foreach (Client_Info client in clientList)
                        {
                            if (client.socket == Current_ClientSocket)
                            {
                                client.flag = true;
                                if ((int.Parse(msgReceived.Message) > 0))
                                {
                                    price_List.Add(int.Parse(msgReceived.Message));
                                }
                               
                            }
                        }

                        break;

                    case Command.List:

                        //Send the names of all users in the chat room to the new user
                        msgToSend.cmd = Command.List;
                        msgToSend.Name = null;
                        msgToSend.Message = null;

                        //Collect the names of the user in the chat room
                        foreach (Client_Info client in clientList)
                        {
                            //To keep things simple we use asterisk as the marker to separate the user names
                            msgToSend.Message += client.strName + "*";
                        }

                        message = msgToSend.ConvertToByte();

                        //Send the name of the users in the chat room
                        Current_ClientSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(Send_Callback), Current_ClientSocket);
                        break;
                    case Command.Result:
                        break;
                }

              
                if (msgToSend.cmd != Command.List)   //List messages are not broadcasted
                {
                    message = msgToSend.ConvertToByte();

                    foreach (Client_Info clientInfo in clientList)
                    {
                        if (clientInfo.socket != Current_ClientSocket ||msgToSend.cmd != Command.Login)
                        {
                            //Send the message to all users
                            clientInfo.socket.BeginSend(message, 0, message.Length, SocketFlags.None,  new AsyncCallback(Send_Callback), clientInfo.socket);
                        }
                    }
                    this.BeginInvoke((MethodInvoker)delegate () { txtLog.Text += msgToSend.Message + "\r\n"; });
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        //below code is for making sure that all client has sent their min price
                        int count = 0;
                        foreach (Client_Info client in clientList)
                        {
                            if (client.flag == true)
                            {
                                count++;
                            }

                        }

                        if (count == clientList.Count)
                        {
                            try
                            {
                                int minPrice = price_List.Min();
                                txtLog.Text += "count var = " + count + " : list count = " + clientList.Count + " min price = " + minPrice + "\r\n";

                                foreach (Client_Info client in clientList)
                                {
                                    if (client.socket == Current_ClientSocket)
                                    {
                                        if (client.id == 1)
                                            lblResult.Text = txtInput.Text + " is available at 'Shopbuzz.pk\n' with lowest price "+minPrice +"Rs.";
                                        else if (client.id == 2)
                                            lblResult.Text = txtInput.Text + " is available at 'Ishopping.pk\n' with lowest price " + minPrice + "Rs.";
                                        break;
                                    }
                                }
                               
                            }
                            catch (Exception)
                            {

                                
                                txtLog.Text += "Sorry, We Are Unable To Crawl Data "+"\r\n";
                            }
                           
                            foreach (Client_Info client in clientList)
                            {
                                client.flag = false;

                            }
                        }

                    });
                }

                //If the user is logging out then we need not listen from her
                if (msgReceived.cmd != Command.Logout)
                {
                    //Start listening to the message send by the user
                    Current_ClientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Receive_Callback), Current_ClientSocket);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Send_Callback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                lblResult.Text = "Wait....";
                //Fill the info for the message to be send
                Data msgSnd = new Data();

                msgSnd.cmd = Command.Result;



                //Send it to the all clients

                foreach (Client_Info clientInfo in clientList)
                {
                    msgSnd.Message = txtInput.Text + "," + clientInfo.id;
                    byte[] byteData = msgSnd.ConvertToByte();
                    //Send the message to all users
                    clientInfo.socket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Send_Callback), clientInfo.socket);

                }


            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
