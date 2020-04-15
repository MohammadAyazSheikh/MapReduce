using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Map_Reduce
{
    class Client_Info
    {
      
            public Socket socket;   //Socket of the client
            public string strName;  //Name by which the user logged into the chat room
            public int id;
            public bool flag;
    }
}
