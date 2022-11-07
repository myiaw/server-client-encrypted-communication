// See https://aka.ms/new-console-template for more information

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Client {
    class Program {
        //NEEDED CONSTANTS
        const int port = 1236;
        const string ip = "127.0.0.1";
        const int msg_size = 1024;
        const int head_size = 1;
        const string key = "key";

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



        static string Decrypt(string toDecrypt) {
            try {
                MD5 md5 = MD5.Create();
                TripleDES trDES = TripleDES.Create();
                trDES.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));     //kljuc sifre
                trDES.Mode = CipherMode.ECB;
                trDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = trDES.CreateDecryptor();
                byte[] data = Convert.FromBase64String(toDecrypt);
                byte[] decrypted = transform.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception e) {
                Console.WriteLine(value: e.Message + "\n");
                return null;
            }

        }


        static void Main(string[] args) {
            Console.WriteLine("\nClient:");
            while (true) {
                Console.WriteLine("\nEnter command: ");
                string input = Console.ReadLine();
                if (input == "q") {
                    break;
                }
                try {
                    TcpClient client = new TcpClient();
                    client.Connect(ip, port);

                    NetworkStream stream = client.GetStream();
                    Send(stream, input);
                    string answer = Receive(stream);

                    if (input[0] == 'G') {

                        answer += " --DECRYPTING--> " + Decrypt(answer);
                    }

                    Console.WriteLine("Client answer: " + answer);
                }
                catch (Exception e) {
                    Console.WriteLine(value: e.Message + "\n");
                }
            }
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();


        }
    }
}
