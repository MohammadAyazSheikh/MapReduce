using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Data
    {
        public string Name;      
        public string Message;   
        public Command cmd;  //Command type (login, logout, send message, etcetera)

        //Default constructor
        public Data()
        {
            this.cmd = Command.Null;
            this.Message = null;
            this.Name = null;
        }

        //Converts the bytes into an object of type Data
        public Data(byte[] _data)
        {
            ConvertToObject(_data);
        }



        //Converts the bytes into an object of type Data
        void ConvertToObject(byte[] data)
        {
            //The first four bytes are for the Command
            this.cmd = (Command)BitConverter.ToInt32(data, 0);

            //The next four store the length of the name
            int nameLen = BitConverter.ToInt32(data, 4);

            //The next four store the length of the message
            int msgLen = BitConverter.ToInt32(data, 8);

            //This check makes sure that strName has been passed in the array of bytes
            if (nameLen > 0)
                this.Name = Encoding.UTF8.GetString(data, 12, nameLen);
            else
                this.Name = null;

            //This checks for a null message field
            if (msgLen > 0)
                this.Message = Encoding.UTF8.GetString(data, 12 + nameLen, msgLen);
            else
                this.Message = null;

        }


        //Converts the Data structure into an array of bytes
        public byte[] ConvertToByte()
        {
            List<byte> result = new List<byte>();

            //First four are for the Command
            result.AddRange(BitConverter.GetBytes((int)cmd));

            //Add the length of the name
            if (Name != null)
                result.AddRange(BitConverter.GetBytes(Name.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Length of the message
            if (Message != null)
                result.AddRange(BitConverter.GetBytes(Message.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Add the name
            if (Name != null)
                result.AddRange(Encoding.UTF8.GetBytes(Name));

            //And, lastly we add the message text to our array of bytes
            if (Message != null)
                result.AddRange(Encoding.UTF8.GetBytes(Message));

            return result.ToArray();
        }
  
    }
}
