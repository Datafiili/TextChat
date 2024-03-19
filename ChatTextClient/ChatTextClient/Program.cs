﻿using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ChatTextClient
{
    internal class Program
    {
        static TcpClient client;
        static NetworkStream stream;
        static bool Connection;
        static string Username = "";
        static void Main(string[] args)
        {
            Console.Write("Write your name: ");
            Username = Console.ReadLine();
            Console.Write("\n" + "Write ip adress you want to connect to: ");
            string ipAdress = Console.ReadLine();
            if(ipAdress == "")
            {
                Connection = Connect();
            }
            else
            {
                Connection = Connect(ipAdress);
            }

            Start(args);
            while (Connection)
            {
                if(SendMessage() == false)
                {
                    return;
                }
            }
        }

        static async void Start(string[] args)
        {
            if (Connection)
            {
                await ReceiveMessage();
            }
        }

        static bool Connect(string ip = "127.0.0.1")
        {
            try
            {
                client = new TcpClient(ip, 1302);
                stream = client.GetStream();
                Console.WriteLine("Connected to the server " + ip);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to connect!");
                return false;
            }
        }

        static async Task ReceiveMessage()
        {
            char[] buffer = new char[1024];
            StreamReader sr = new StreamReader(stream);
            await sr.ReadAsync(buffer,0,1024);
            Console.WriteLine(buffer);
            await ReceiveMessage();
        }

        static bool SendMessage()
        {
            
            string messageToSend = Console.ReadLine();

            //Disconnecting
            if(messageToSend.ToLower() == "e" || messageToSend.ToLower() == "exit")
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
