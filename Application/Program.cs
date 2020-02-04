using System;
using ClientLib;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to chat application");

            if (GetInputFromUser().Broadcast)
            {

            }
            else
            {

            }
        }

        //static void MessagePrinter(Message message)
        //{

        //}

        static Message GetInputFromUser()
        {
            Console.WriteLine("Input Sender ID");
            string sender = Console.ReadLine();
            
            Console.WriteLine("Input Receiver ID");
            string receiver = Console.ReadLine();
            
            Console.WriteLine("Type Messsage");
            string message = Console.ReadLine();
            
            Console.WriteLine("Is it Broadcast? Type yes or no)");
            string inputString = Console.ReadLine();
            bool broadcast = inputString.ToLower() == "yes" || inputString.ToLower() == "y";

            Message m1 = new Message
            {
                Broadcast = broadcast,
                SenderClientID = sender,
                ReceiverClientID = receiver,
                MessageBody = message
            };

            return m1;
        }

        

        //public void PrintInbox(Client c)
        //{
        //    foreach (message in c.Inbox)
        //    {
        //        MessagePrinter(message);
        //    }
        //}
    }
}
