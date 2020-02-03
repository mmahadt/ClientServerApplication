using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientLib
{
    public class Client
    {
        private string Id;
        public Queue<string> Inbox = new Queue<string>();
        Client()
        {
            TcpClient clientSocket = new TcpClient();
            NetworkStream serverStream;

            clientSocket.Connect(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], 8888);
            
            serverStream = clientSocket.GetStream();

            Id = ReceiveFromServerStream(serverStream);
            
            Thread receiverThread = new Thread(() => MessageReceiverThreadFunction(serverStream));
            receiverThread.Start();

            while (true)
            {
                SendToServerStream(serverStream,"Assalamu Alaikum!");
            }
        }

        public Message Broadcast(string message)
        {
            Message m = new Message
            {
                Broadcast = true,
                SenderClientID = Id,
                MessageBody = message,
                ReceiverClientID = null
            };
            return m;
        }

        public Message Unicast(string message, string receiverId)
        {
            Message m = new Message
            {
                Broadcast = false,
                SenderClientID = Id,
                MessageBody = message,
                ReceiverClientID = receiverId
            };
            return m;
        }

        private static string GetMessageText(string msg)
        {
            string[] words = msg.Split('_');
            return words[0];
        }

        private static string GetSenderId(string msg)
        {
            string[] words = msg.Split('_');
            return words[1];
        }
        private static string GetReceiverId(string msg)
        {
            string[] words = msg.Split('_');
            return words[2];
        }
        private static bool IsBroadcast(string msg)
        {
            string[] words = msg.Split('_');
            return (words[3] == "1");
        }

        //public static void Unicast(string msg, string receiverId)
        //{
        //    //TODO: 
        //}

        //public static void Broadcast(string msg)
        //{
        //    //TODO: Turn on boolean braodcast flag in message object
        //}

        private void MessageReceiverThreadFunction(NetworkStream stream)
        {
            while (true)
            {
                string dataFromServer = ReceiveFromServerStream(stream);
                Inbox.Enqueue(dataFromServer);
            }
        }

        //https://stackoverflow.com/questions/7099875/sending-messages-and-files-over-networkstream
        private static string ReceiveFromServerStream(NetworkStream serverStream)
        {
            //// Client side
            
            //var bin = new BinaryFormatter();
            //listOfClients = (List<string>)bin.Deserialize(serverStream);

            //Read the length of incoming message from the server stream
            byte[] msgLengthBytes1 = new byte[sizeof(int)];
            serverStream.Read(msgLengthBytes1, 0, msgLengthBytes1.Length);
            //store the length of message as an integer
            int msgLength1 = BitConverter.ToInt32(msgLengthBytes1, 0);

            //create a buffer for incoming data of size equal to length of message
            byte[] inStream = new byte[msgLength1];
            //read that number of bytes from the server stream
            serverStream.Read(inStream, 0, msgLength1);
            //convert the byte array to message string
            string dataFromServer = Encoding.ASCII.GetString(inStream);

            return dataFromServer;
        }

        private static void SendToServerStream(NetworkStream serverStream, string message)
        {
            //Get the length of message in terms of number of bytes
            int messageLength = Encoding.ASCII.GetByteCount(message);

            //lengthBytes are first 4 bytes in stream that contain
            //message length as integer
            byte[] lengthBytes = BitConverter.GetBytes(messageLength);
            serverStream.Write(lengthBytes, 0, lengthBytes.Length);

            //Write the message to the server stream
            byte[] outStream = Encoding.ASCII.GetBytes(message);
            serverStream.Write(outStream, 0, outStream.Length);

            //ReceiveFromServerStream(serverStream);
            serverStream.Flush();
        }

        private static void SendToServerStream(NetworkStream serverStream, Message message)
        {
            var bin = new BinaryFormatter();
            //Sending bytes / actual object; which one is better?
            bin.Serialize(serverStream, message);

            //ReceiveFromServerStream(serverStream);
            serverStream.Flush();
        }
    }
}
