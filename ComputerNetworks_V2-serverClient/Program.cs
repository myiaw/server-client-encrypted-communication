// See https://aka.ms/new-console-template for more information

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Server {
    class Program {
        //NEEDED CONSTANTS
        const int port = 1236;
        const string ip = "127.0.0.1";
        const int msg_size = 1024;
        const int head_size = 1;
        const string key = "key";

        static bool isRunning = true;

        static string? Receive(NetworkStream stream) {
            try {
                //WE RECIEVE MESSAGE IN BYTES (ARRAY OF BYTES WITH SIZE I SET EARLIER)
                byte[] recievedBytes = new byte[msg_size];
                int len = stream.Read(recievedBytes, 0, recievedBytes.Length);
                //IF Read() IS NULL IT TRIGGERS EXCEPTION
                return Encoding.UTF8.GetString(recievedBytes, 0, len);
            }
            catch (Exception e) {
                Console.WriteLine(value: e.Message + "\n");
                return null;
            }
        }
        static void Send(NetworkStream stream, string msg) {
            try {
                byte[] sendMsg = Encoding.UTF8.GetBytes(msg.ToCharArray(), 0, msg.ToCharArray().Length);
                stream.Write(sendMsg, 0, sendMsg.Length);
            }
            catch (Exception e) {
                Console.WriteLine(value: e.Message + "\n");
            }
        }


        static string Encrypt(string toEncrypt) {
            try {
                MD5 md5 = MD5.Create();
                TripleDES trDES = TripleDES.Create();
                UTF8Encoding utf8 = new UTF8Encoding();
                trDES.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                trDES.Mode = CipherMode.ECB;
                trDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = trDES.CreateEncryptor();
                byte[] data = Encoding.ASCII.GetBytes(toEncrypt);
                byte[] hidden = transform.TransformFinalBlock(data, 0, data.Length);

                return Convert.ToBase64String(hidden, 0, hidden.Length);



            }
            catch (Exception e) {
                Console.WriteLine(value: e.Message + "\n");
                return null;

            }

            }
    
    

        static void Main(string[] args) {
            TcpListener listener = new TcpListener(IPAddress.Parse(ip), port);
            listener.Start();

            Console.WriteLine("\nListening at: " + ip + ":" + port.ToString());
            while (isRunning) {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream()) {
                    Console.WriteLine("\nClient has connected with: " + client.Client.RemoteEndPoint.ToString());
                    string msg = Receive(stream);

                    if(msg == "1") {
                        isRunning = false;
                        break;
                    }

                    Console.WriteLine("\nRecieved:" + msg);
                    string answer = "";

                    string head = msg[0].ToString();
                    string body = " ";

                    if(msg.Length > 1) {
                        body = msg.Substring(head_size, msg.Length - 1);
                        if (body[0] == ' ') {
                            body.Remove(0, 1);
                        }

                    }

               


                    switch (head) {
                        case "A":
                            answer = "Hello, " + client.Client.RemoteEndPoint.ToString();    //IP AND PORT
                            break;
                        case "B":
                            answer = "Current time is: " + DateTime.Now.ToString("dd. MM. yyyy, H : mm"); //CURRENT DATE & TIME
                            break;
                        case "C":
                            answer = Environment.CurrentDirectory; // CURRENT DIRECTORY
                            break;
                        case "D": 
                            answer = body; //OUTPUT JUST THE BODY
                            break;
                        case "E":
                            answer =  Environment.MachineName + "\n" + Environment.OSVersion.ToString();    //PC NAME AND OS VERSION
                            break;
                        case "G":
                            answer = Encrypt(body); //ENCRYPT
                            break;
                        default: 
                            Console.WriteLine("NOT AN OPTION");
                            break;
                    }

                    Send(stream, answer);
                    Console.WriteLine("Answer: " + answer);
                }
                Console.WriteLine("\nClient disconnected.");
            }
            listener.Stop();
            Console.WriteLine("\nServer stopped.");
            Console.ReadKey();
        }


    }
}