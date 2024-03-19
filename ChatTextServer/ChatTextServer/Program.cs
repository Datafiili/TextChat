using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatTextServer
{
    internal class Program
    {
        static TcpListener listener;
        static TcpClient client;
        static NetworkStream stream;
        static string Username = "";

        static void Main(string[] args)
        {
            Console.Write("Write your name: ");
            Username = Console.ReadLine();

            Setup();

            while (true)
            {
                if(client != null)
                {
                    if(SendMessage() == false)
                    {
                        return;
                    }

                }
            }
        }

        static async void Setup()
        {
            CreateServer();
            await RecieveClient();
        }

        static void CreateServer()
        {
            listener = new TcpListener(System.Net.IPAddress.Any, 1302);
            listener.Start();
            Console.WriteLine("Waiting for a connection.");
        }

        static async Task RecieveClient()
        {
            client = await listener.AcceptTcpClientAsync();
            
            Console.WriteLine("Client accepted.");
            stream = client.GetStream();
            await ReceiveMessage();
        }

        static async Task ReceiveMessage()
        {
            char[] buffer = new char[1024];
            StreamReader sr = new StreamReader(stream);
            await sr.ReadAsync(buffer, 0, 1024);
            string Message = "";

            //Checks for disconenct
            if (buffer[0] == 0)
            {
                Console.WriteLine("Client has disconnected!");
                return;
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != 0)
                {
                    Message += buffer[i];
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine(Message);
            await ReceiveMessage();
        }

        static bool SendMessage()
        {
            string messageToSend = Console.ReadLine();
            //Disconnecting
            if (messageToSend.ToLower() == "e" || messageToSend.ToLower() == "exit")
            {
                CloseStream();
                return false;
            }
            messageToSend = Username + ": " + messageToSend;
            int byteCount = Encoding.UTF8.GetByteCount(messageToSend + 1);
            byte[] sendData = Encoding.UTF8.GetBytes(messageToSend);
            stream.Write(sendData, 0, sendData.Length);
            return true;
        }

        static void CloseStream()
        {
            stream.Close();
            client.Close();
        }
    }
}
