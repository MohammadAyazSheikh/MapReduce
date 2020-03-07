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
       
        public Socket clientSocket; //The main client socket
        public string strName;      //Name by which the user logs into the room
        Product product = new Product();
      

        private byte[] byteData = new byte[1024];
        public MainForm()
        {
           
            InitializeComponent();
            btnSend.Enabled = true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                //Fill the info for the message to be send
                Data msgToSend = new Data();

                msgToSend.Name = strName;
                msgToSend.Message = txtMessage.Text;
                msgToSend.cmd = Command.Message;

                byte[] byteData = msgToSend.ConvertToByte();

                //Send it to the server
                clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

                txtMessage.Text = null;
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "SGSclientTCP: " + strName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclientTCP: " + strName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndReceive(ar);
               
                Data msgReceived = new Data(byteData);
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
                            // MessageBox.Show("index zero = " +arr[0]+"index 1 = "+arr[1]);

                            this.BeginInvoke((MethodInvoker)delegate () {
                                label1.Text = "";
                                label1.Text += arr[0] + "  " + arr[1];
                                //MessageBox.Show(label1.Text);
                                ;
                            });
                            int cid = Convert.ToInt16(arr[1]);

                            //sendPrice(product.FindMin("iphone 9", 1).ToString());
                            sendPrice(product.FindMin(arr[0], cid).ToString());
                            //sendPrice(product.Get_Min_Price(arr[0]).ToString());
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
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);


            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client Ka error: " + strName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

            byteData = new byte[1024];
            //Start listening to the data asynchronously
            clientSocket.BeginReceive(byteData,  0, byteData.Length,     SocketFlags.None, new AsyncCallback(OnReceive), null);
        }

        private void txtChatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, null);
            }
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                //Fill the info for the message to be send
                Data msgSnd = new Data();

                msgSnd.cmd = Command.Message;
                msgSnd.Message = txtMessage.Text;
                byte[] byteData = msgSnd.ConvertToByte();

                //Send it to the all clients


                //Send the message to all users
                clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None,  new AsyncCallback(OnSend), clientSocket);

                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);


            }
            catch (Exception)
            {
                //MessageBox.Show("Unable to send message to the server.", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
               
                
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "CLient: " + strName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
