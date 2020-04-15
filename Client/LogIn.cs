using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class LogIn : Form
    {
        public Socket clientSocket;
        public string strName;
        public LogIn()
        {
            InitializeComponent();
            txtName.Text = "Client 1";
            txtServerIP.Text = "127.0.0.1";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress ipAddress = IPAddress.Parse(txtServerIP.Text);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1000);

                //Connect to the server
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(Connect_Callback), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "client error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void OnSend(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
                strName = txtName.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Connect_Callback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

                //We are connected so we login into the server
                Data msgToSend = new Data();
                msgToSend.cmd = Command.Login;
                msgToSend.Name = txtName.Text;
                msgToSend.Message = null;

                byte[] b = msgToSend.ConvertToByte();

                //Send the message to the server
                clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LogIn_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtName.Text.Length > 0 && txtServerIP.Text.Length > 0)
                btnOK.Enabled = true;
            else
                btnOK.Enabled = false;
        }

        private void txtServerIP_TextChanged(object sender, EventArgs e)
        {
            if (txtName.Text.Length > 0 && txtServerIP.Text.Length > 0)
                btnOK.Enabled = true;
            else
                btnOK.Enabled = false;
        }
    }
}
