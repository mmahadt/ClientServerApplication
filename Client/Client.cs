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
        public Queue<Message> Inbox = new Queue<Message>();
        private NetworkStream serverStream;
        private TcpClient clientSocket;
        
        Client()
        {
            clientSocket = new TcpClient();
            
            clientSocket.Connect(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], 8888);
            
            serverStream = clientSocket.GetStream();

            Thread receiverThread = new Thread(() => MessageReceiverThreadFunction(serverStream));
            receiverThread.Start();

            //First message sent to each client is from server containing 
            //id assigned by the server
            Id = Inbox.Dequeue().MessageBody;
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
            SendToServerStream(serverStream, m);
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
            SendToServerStream(serverStream, m);
            return m;
        }

        private void MessageReceiverThreadFunction(NetworkStream stream)
        {
            while (true)
            {
                Message dataFromServer = ReceiveFromServerStream(stream);
                Inbox.Enqueue(dataFromServer);
            }
        }

        private static void SendToServerStream(NetworkStream serverStream, Message message)
        {
            var bin = new BinaryFormatter();
            bin.Serialize(serverStream, message);
            serverStream.Flush();
        }

        private Message ReceiveFromServerStream(NetworkStream networkStream)
        {
            // Client side
            var bin = new BinaryFormatter();
            Message m1 = (Message)bin.Deserialize(networkStream);
            return m1;
        }
    }
}
