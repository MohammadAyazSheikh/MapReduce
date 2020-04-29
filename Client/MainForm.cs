using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MainForm : Form
    {
        //flag for from closing 
        bool FormCloseFlag = false;
        public Socket clientSocket; //The main client socket
        public string strName;      //Name by which the user logs into the room
        Product product = new Product();


        private byte[] byteData = new byte[1024];
        public MainForm()
        {

            InitializeComponent();
            //btnSend.Enabled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "Client: " + strName;


            //The user has logged into the system so we now request the server to send
            //the names of all users who are in the chat room
            Data msgToSend = new Data();
            msgToSend.cmd = Command.List;
            msgToSend.Name = strName;
            msgToSend.Message = null;

            byteData = msgToSend.ConvertToByte();

            clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Send_Callback), null);


            byteData = new byte[1024];
            //Start listening to the data asynchronously
            clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Receive_Callback), null);
        }

     

        private void Send_Callback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client Error: " + strName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Receive_Callback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndReceive(ar);

                Data msgReceived = new Data(byteData);

                //servr will send msg to client to logout if the there
                //was no room for new client 
                if (msgReceived.Message == "Logout")
                {
                    //Fill the info for the message to be send
                    Data msgToSend = new Data();

                    msgToSend.Name = strName;
                    msgToSend.Message = " ";
                    msgToSend.cmd = Command.Logout;

                    byte[] byteData = msgToSend.ConvertToByte();

                    //Send it to the server
                    clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Send_Callback), null);
                    clientSocket.Close();
                    FormCloseFlag = true;
                    this.Close();

                } //below else block will run if server log out
                else if (msgReceived.Message == "ServerLogOut")
                {
                    clientSocket.Close();
                    this.Close();

                }


                //Accordingly process the message received
                switch (msgReceived.cmd)
                {
                    case Command.Login:
                        lstChatters.Items.Add(msgReceived.Name);
                        break;

                    case Command.Logout:
                        lstChatters.Items.Remove(msgReceived.Name);
                        break;

                    case Command.Message:
                        break;

                    case Command.List:
                        lstChatters.Items.AddRange(msgReceived.Message.Split('*'));
                        lstChatters.Items.RemoveAt(lstChatters.Items.Count - 1);
                        txtChatBox.Text += "<<<" + strName + " has joined the room>>>\r\n";
                        break;
                    case Command.Result:
                        try
                        {
                            string[] arr = new string[2];
                            arr = msgReceived.Message.Split(',');


                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                
                                txtChatBox.Text +=" Mobile Name: "+ arr[0] + " Client ID  " + arr[1];
                               
                                
                            });
                            int client_Id = Convert.ToInt16(arr[1]);

                          
                            sendPrice(product.FindMin(arr[0], client_Id).ToString());
                           
                        }
                        catch (Exception)
                        {

                            sendPrice("-1");
                        }
                        break;

                }

                if (msgReceived.Message != null && msgReceived.cmd != Command.List)
                    txtChatBox.Text += msgReceived.Message + "\r\n";

                byteData = new byte[1024];
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Receive_Callback), null);


            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client Ka error: " + strName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        void sendPrice(string Price)
        {

            try
            {

                //Fill the info for the message to be send
                Data msgToSend = new Data();

                msgToSend.Name = strName;
                msgToSend.Message = Price;
                msgToSend.cmd = Command.Message;

                byte[] byteData = msgToSend.ConvertToByte();

                //Send it to the server
                clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Send_Callback), null);


            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "CLient: " + strName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Logout_Click(object sender, EventArgs e)
        {
            try
            {
                //Fill the info for the message to be send
                Data msgToSend = new Data();

                msgToSend.Name = strName;
                msgToSend.Message = " ";
                msgToSend.cmd = Command.Logout;

                byte[] byteData = msgToSend.ConvertToByte();

                //Send it to the server
                clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(Send_Callback), null);
                clientSocket.Close();

                //clsoing form
                FormCloseFlag = true;
                this.Close();

            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "Client Error: " + strName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormCloseFlag)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}

