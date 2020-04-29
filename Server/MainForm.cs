using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        //flag for from closing 
        bool FormCloseFlag = false;

        //The collection of all clients logged into the room (an array of type ClientInfo)
        List<Client_Info> clientList;

        //The main socket on which the server listens to the clients
        Socket serverSocket;

        //client id
        int id = 0;

        //this queue is for logout client's ids
        Queue<int> id_List = new Queue<int>();
        //we we will add min price from each website in this list
        List<int> price_List = new List<int>();

        List<Product> pro_list = new List<Product>();
      
        byte[] byteData = new byte[1024];

        public MainForm()
        {
            clientList = new List<Client_Info>();
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
                Client_Info clientInfo = new Client_Info();

                //If the message is to login, logout, or simple text message
                //then when send to others the type of the message remains the same
                msgToSend.cmd = msgReceived.cmd;
                msgToSend.Name = msgReceived.Name;

                switch (msgReceived.cmd)
                {
                    case Command.Login:

                        //When a user logs in to the server then we add her to our
                        //list of clients
                   
                        if (clientList.Count < 4)
                        {
                            
                            if (id_List.Count > 0)
                            {
                               
                                clientInfo.socket = Current_ClientSocket;
                                clientInfo.strName = msgReceived.Name;
                                clientInfo.id = id_List.Dequeue();
                                clientInfo.flag = false;
                                clientList.Add(clientInfo);
                                //Set the text of the message that we will broadcast to all users
                                msgToSend.Message = "<<<" + msgReceived.Name + " has joined the room and Id is" + clientInfo.id + ">>>";
                            }
                            else
                            {
                                id++;
                                clientInfo.socket = Current_ClientSocket;
                                clientInfo.strName = msgReceived.Name;
                                clientInfo.id = id;
                                clientInfo.flag = false;
                                clientList.Add(clientInfo);
                                //Set the text of the message that we will broadcast to all users
                                msgToSend.Message = "<<<" + msgReceived.Name + " has joined the room and ID is " + id + ">>>";
                               

                            }
                            this.BeginInvoke((MethodInvoker)delegate (){txtLog.Text += msgToSend.Message + "\n"; });

                        }
                        else
                        {
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                
                                txtLog.Text += "Server: Only 4 clients can be connected\n";    
                            });

                            //sending new client to logout because of no room for 5th client.
                            Data msg = new Data();
                            msg.Message = "Logout";
                            msg.cmd = Command.Message;
                            byte[] arrByte = msg.ConvertToByte();
                            arrByte = msg.ConvertToByte();
                            Current_ClientSocket.BeginSend(arrByte, 0, arrByte.Length, SocketFlags.None, new AsyncCallback(Send_Callback), Current_ClientSocket);
                        }
                    
                        break;

                    case Command.Logout:

                        //When a user wants to log out of the server then we search for her 
                        //in the list of clients and close the corresponding connection

                        int nIndex = 0;
                        foreach (Client_Info client in clientList)
                        {
                            if (client.socket == Current_ClientSocket)
                            {
                                id_List.Enqueue(client.id);
                                clientList.RemoveAt(nIndex);
                                msgToSend.Message = "<<<" +client.strName +" has left the room>>>";
                                break;
                            }
                            ++nIndex;
                        }
                       
                        Current_ClientSocket.Close();

                     
                        break;


                    case Command.Message:

                      
                      

                        //Set the text of the message that we will broadcast to all users
                        msgToSend.Message = msgReceived.Name + ": " + msgReceived.Message;
                        BeginInvoke((MethodInvoker)delegate () { txtLog.Text += msgToSend.Message +"\n"; });
                        //saving min price from client
                        foreach (Client_Info client in clientList)
                        {
                            if (client.socket == Current_ClientSocket)
                            {
                                //makeking flag true of current client to make sure client has sent the min price
                                client.flag = true;

                                // if client return some valid price means if the price is > 0 so we will ad this 
                                //price in the list
                               
                                if ((int.Parse(msgReceived.Message) > 0))
                                {
                                    price_List.Add(int.Parse(msgReceived.Message));

                                    pro_list.Add(new Product()
                                    {
                                        price = int.Parse(msgReceived.Message),
                                        Client_Id = client.id
                                    });

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
   
                }


                if (msgToSend.cmd != Command.List)   //List messages are not broadcasted
                {
                    message = msgToSend.ConvertToByte();

                    foreach (Client_Info client_ in clientList)
                    {
                        if (client_.socket != Current_ClientSocket || msgToSend.cmd != Command.Login)
                        {
                            //Send the message to all users
                            client_.socket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(Send_Callback), client_.socket);
                        }
                    }

                   
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        
                        //below code is for making sure that all client has sent their min price
                        int count = 0;
                        foreach (Client_Info client in clientList)
                        {
                            //counting how many clients flag
                            //are true who has sent the min price
                            if (client.flag == true)
                            {
                                count++;
                            }

                        }

                        //cheking counted true flags are equal to total clients
                        if (count == clientList.Count)
                        {
                            try
                            {

                                btnSearch.Enabled = true;

                                if (price_List != null && pro_list != null)
                                {
                                    //getting min price from price sent from all client
                                    int minPrice = price_List.Min();

                                    txtLog.Text += "Flag Count = " + count + " : Clientlist Count = " + clientList.Count + " Min Price = " + minPrice + "\r\n";


                                    //below code for getting the site name which has min price 
                                    //copying list
                                    List<Product> Product_list = pro_list.ToList();
                                    foreach (Product Product in Product_list)
                                    {
                                        if (Product.price == minPrice)
                                        {
                                            int id = Product.Client_Id;
                                            if (id == 1)
                                                lblResult.Text = txtInput.Text + " is available at 'PriceOye.pk'\n with lowest price " + minPrice + "Rs.";
                                            else if (id == 2)
                                                lblResult.Text = txtInput.Text + " is available at 'IShopping.pk'\n with lowest price " + minPrice + "Rs.";
                                            else if (id == 3)
                                                lblResult.Text = txtInput.Text + " is available at 'HomeShopping.pk'\n with lowest price " + minPrice + "Rs.";
                                            else if (id == 4)
                                                lblResult.Text = txtInput.Text + " is available at 'ShopBuzz.pk'\n with lowest price " + minPrice + "Rs.";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    lblResult.Text = "Sorry, we cant find min price\n";
                                }

                                price_List.Clear();
                                pro_list.Clear();

                            }
                            catch (Exception e)
                            {

                                txtLog.Text += "Sorry, We Are Unable To Crawl Data " + "\r\n"+ e.Message.ToString();

                            }

                            //making all clients flag false again
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
                btnSearch.Enabled = false;
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

        private void btnClose_Click(object sender, EventArgs e)
        {
           
            try
            {
               
                Data msgSnd = new Data();

                msgSnd.cmd = Command.Result;
                msgSnd.Message = "ServerLogOut";


                //Send it to the all clients for logging Out             
                foreach (Client_Info clientInfo in clientList)
                {

                    byte[] byteData = msgSnd.ConvertToByte();
                    //Send the message to all users
                    clientInfo.socket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Send_Callback), clientInfo.socket);
                    //stop listenting from client
                    clientInfo.socket.Close();

                }

                //closing server
                FormCloseFlag = true; 
                this.Close();

            }
            catch (Exception)
            {
                MessageBox.Show("", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if close button is clicked
            //we will close the form
            if (FormCloseFlag)
            {
                e.Cancel = false;
            }
            else
            {
                //else preventing form from closing 
                e.Cancel = true;
            }
           
        }
    }
}
