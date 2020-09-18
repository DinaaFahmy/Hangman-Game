using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Main_Server
{
    class Server
    {
        public static List<ClassPlayer> players = new List<ClassPlayer>();
        public static List<ClassRoom> rooms = new List<ClassRoom>();
        public static List<int> roomPorts = new List<int>();
        public static NetworkStream n;
        public static string s = "";
        public static IPAddress ip;
        public static Int32 port;
        public static TcpClient client;
        public static string cat;//category
        public static List<TcpClient> mClients;
        static void Main(string[] args)
        {
            mClients = new List<TcpClient>();
            TcpListener server = null;
            try
            {
                //Set the TcpListener on port 4000.
                port = 4000;
                ip = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(ip, port);

                //Start listening for client requests.
                server.Start();

                Console.WriteLine("Waiting for a connection... ");

                //Enter the listening loop
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    mClients.Add(client);
                    
                    string name = "";
                    NetworkStream n = client.GetStream();

                    StreamReader reader = new StreamReader(n);
                    name = reader.ReadLine();
                    ClassPlayer p = new ClassPlayer(name);
                    players.Add(p);
                    Console.WriteLine(p.playerName);
                   
                    SendToAll(s);

                    cat = reader.ReadLine();
                    Console.WriteLine(name + ":" + cat);

                    if (cat == "food" || cat == "colors" || cat == "animals") 
                    {
                        
                        ClassRoom r= new ClassRoom(cat, ++port);
                        rooms.Add(r);
                        n=client.GetStream();
                        StreamWriter ww = new StreamWriter(n);
                        ww.WriteLine(port+"");
                        ww.Flush();

                        roomPorts.Add(port);
                        s += port.ToString()+","+cat+","+name+ "*";
                        Console.WriteLine(r.roomport + " category: "+cat);
                    }
                    foreach(var room in rooms)
                    { 
                    Console.WriteLine(room.category+" room:has "+ room.playersList.Count + " of players");
                    }
                   
                }

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            //finally
            //{
               
            //    server.Stop();
            //}

            //Console.WriteLine("\nHit enter to continue...");
            //Console.Read();
        }

        public static async void SendToAll(string message)
        {
          
            
                try
                {
                    
                    foreach (TcpClient c in mClients)
                    {
                       NetworkStream s= c.GetStream();
                        StreamWriter w = new StreamWriter(s);
                        w.WriteLine(message);
                        w.Flush();
                    }
                }
                catch (Exception exception)
                {
                    
                  Console.WriteLine(exception.ToString());

                }

        }


    }
}



