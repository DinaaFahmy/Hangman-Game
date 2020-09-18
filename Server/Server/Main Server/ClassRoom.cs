using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Main_Server
{
    class ClassRoom
    {
       public  List<TcpClient> playersList = new List<TcpClient>(); 
        //public bool flag = true;
        public TcpListener server;
        public NetworkStream n;
        public int roomport;
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        public string category;
       // public List<string> usedLetters;
       // public string status="waiting";
        bool p1AnswerIsCorrect = true; //p2AnswerIsCorrect=false;

        public ClassRoom(string category,int port)
        {
           // usedLetters = new List<string>();
            this.category = category;
            
            roomport = port;
                       
            server = new TcpListener(ip, port);
            
            server.Start();
            Task.Run(() => {
                while (true)
                { 
                    TcpClient player = server.AcceptTcpClient();
                    if (player != null)
                    {
                        n = player.GetStream();
                        StreamWriter w = new StreamWriter(n);
                        w.WriteLine("you are connected");
                        w.Flush();
                        playersList.Add(player);
                    }
                    if(playersList.Count>=2&& playersList.Count<4)
                    {
                        //send to player1 that player2 connected
                        n= playersList[0].GetStream();
                        StreamWriter w = new StreamWriter(n);
                        w.WriteLine("yes");
                        w.Flush();

                       // status = "playing";
                        
                        //read random word
                        StreamReader rr = new StreamReader(n);
                        string word=rr.ReadLine();
                        Console.WriteLine(word);

                        //send random word to player2
                        n = playersList[1].GetStream();
                        StreamWriter ww = new StreamWriter(n);
                        ww.WriteLine(word + "*" + category+"*"+"player2");
                        ww.Flush();

                        //watcher
                        if (playersList.Count == 3)
                        {
                            n = playersList[2].GetStream();
                            StreamWriter www = new StreamWriter(n);
                            www.WriteLine(word + "*" + category + "watcher");
                            www.Flush();
                        }

                        while (true)
                        {
                            try
                            {

                                while (p1AnswerIsCorrect == true)
                            {
                                

                                    //receive correct letters from player1
                                    n = playersList[0].GetStream();
                                    StreamReader streamReader = new StreamReader(n);
                                    string answer = streamReader.ReadLine();
                                    // usedLetters.Add(answer);
                                    // Console.WriteLine(answer);

                                    //play again
                                    if (answer.Split('*')[0] == "PA")
                                    {
                                        word = answer.Split('*')[1];
                                        n = playersList[1].GetStream();
                                        StreamWriter www = new StreamWriter(n);
                                        www.WriteLine(word + "*" + "PA");
                                        www.Flush();
                                    }
                                    else
                                    {
                                        //send correct letters to player 2
                                        n = playersList[1].GetStream();
                                        StreamWriter ww2 = new StreamWriter(n);
                                        ww2.WriteLine(answer);
                                        ww2.Flush();
                                        //answer after split
                                        string[] answerAfter = answer.Split('*');
                                        p1AnswerIsCorrect = Convert.ToBoolean(answerAfter[2]);
                                        Console.WriteLine(p1AnswerIsCorrect);
                                    }
                                }
                               
                            }
                             catch (Exception e)
                            {
                                Console.WriteLine("player1 disconnected");

                            }

                            try
                         {
                        while (p1AnswerIsCorrect == false)
                        {
                                
                                    //receive correct letters from player2
                                    n = playersList[1].GetStream();
                                    StreamReader streamReader = new StreamReader(n);

                                    string answer = streamReader.ReadLine();
                                    //usedLetters.Add(answer);
                                    //Console.WriteLine(answer);
                                    if (answer.Split('*')[0] == "PA")
                                    {
                                        word = answer.Split('*')[1];
                                        n = playersList[0].GetStream();
                                        StreamWriter www = new StreamWriter(n);
                                        www.WriteLine(word + "*" + "PA");
                                        www.Flush();
                                    }
                                    else
                                    {
                                        //send correct letters to player1
                                        n = playersList[0].GetStream();
                                        StreamWriter ww2 = new StreamWriter(n);
                                        ww2.WriteLine(answer);
                                        ww2.Flush();

                                        string[] answerAfter = answer.Split('*');
                                        p1AnswerIsCorrect = bool.Parse(answerAfter[2]);
                                        Console.WriteLine(p1AnswerIsCorrect);
                                    }
                                }
                                
                        }
                            catch (Exception e)
                            {
                                Console.WriteLine("player2 disconnected");


                            }


                        }


                       

                    }
                }
            });
           
        }
       
    }
}
