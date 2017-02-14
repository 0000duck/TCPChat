using DA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP
{
    class Program
    {
        private const int BUFFER_SIZE = 1024;
        private const int PORT_NUMBER = 9090;

        static ASCIIEncoding encoding = new ASCIIEncoding();

        static void Main(string[] args)
        {
            List<Student> lst = new List<Student>()
            { 
                new Student { Name= "Hai", Class="TH12A", Marks = new float[] { 2.5f, 3.1f, 3.0f } },
                new Student { Name = "Anh", Class = "TH12B", Marks = new float[] { 2.1f, 3.1f, 3.2f } },
                new Student { Name = "Binh", Class = "TH12B", Marks = new float[] { 2.4f, 3.3f, 3.6f } }
            };

            try
            {
                IPAddress address = IPAddress.Parse("127.0.0.1");

                TcpListener listener = new TcpListener(address, PORT_NUMBER);

                // 1. listen
                listener.Start();

                Console.WriteLine("Server started on " + listener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection...");

                Socket socket = listener.AcceptSocket();
                Console.WriteLine("Connection received from " + socket.RemoteEndPoint);

                NetworkStream stream = new NetworkStream(socket);
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                writer.AutoFlush = true;

                while (true)
                {
                    // 2. receive
                    string str = reader.ReadLine();
                    string ret = "";
                    if (str.ToLower() == ":q")
                    {
                        writer.WriteLine(":ack:q");
                        break;
                    }

                    string[] words = str.Split(' ');

                    if (words.Length >= 2 && words[0].ToLower() == "student")
                    {
                        switch (words[1].ToLower())
                        {
                            case "--l":
                            case "--a":
                            case "-list":
                            case "-all":
                                ret = JsonConvert.SerializeObject(lst).ToString();
                                break;

                            case "--n":
                            case "-name":
                                if(words.Length >= 3)
                                {
                                    List<Student> tmp = new List<Student>();
                                    lst.ForEach(delegate (Student s)
                                    {
                                        if (s.Name.ToLower().Contains(words[2].ToLower()))
                                        {
                                            tmp.Add(s);
                                        }
                                    });
                                    ret = JsonConvert.SerializeObject(tmp).ToString();
                                }
                                else
                                {
                                    ret = "Sorry! We cannot found this command.\n";
                                }
                                break;

                            default:
                                ret = "Sorry! We cannot found this command.\n";
                                break;
                        }
                    }
                    else
                    {
                        ret = "Sorry! We cannot found this command.\n";
                    }

                    // 3. send
                    writer.WriteLine(ret);
                }
                // 4. close
                stream.Close();
                socket.Close();
                listener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
            Console.Read();
        }
    }
}
