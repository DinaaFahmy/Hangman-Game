using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string category;
        string player_name;
        string randomword;
        string dashline;
        //-----------------------------------------------------------------------------------------------------------
        public string Catval;
        NetworkStream substream;
        //member of client when he join to existing room
        TcpClient client;//connect to main server
        public int room_port;
        NetworkStream net;
        TcpClient clientroom; //connect to room server
        string playertype;
        int port = 4000;
        string ipAddress = "127.0.0.1";
        List<string> p1SelectedChars = new List<string>();
        List<string> p2SelectedChars = new List<string>();
        bool correctValue = true;
        bool correctValuep2 = false;
      


        public Form1()
        {
            InitializeComponent();
          
        }


        private void login_Click(object sender, EventArgs e)
        {
            login.Visible = false;
            name.Visible = true;
            nn.Visible = true;
            enter_name.Visible = true;
        }

        private void nn_Click(object sender, EventArgs e)
        {
            if(enter_name.Text=="")
            {
                MessageBox.Show("enter your name please");
            }
            else
            {
                player_name = enter_name.Text;
                //connect to server
                client = new TcpClient();
                client.Connect(ipAddress, port);
                // MessageBox.Show("Welcome " + player_name);
                hangman.Text += " "+player_name;
                //*************************************************
                
                //send name to server
                net = client.GetStream();
                StreamWriter w = new StreamWriter(net);
                w.WriteLine(player_name);
                w.Flush();

                //receive avaliable rooms
                StreamReader r = new StreamReader(net);
                string st = r.ReadLine();
                string[] se = st.Split('*');

                enter_name.Visible = false;
                name.Visible = false;
                nn.Visible = false;
                rooms_avaliable.Visible = true;
                listBox1.Visible = true;
                create_room.Visible = true;
                refresh.Visible = true;
                listBox1.Items.AddRange(se);

            }
        }

        private void create_room_Click(object sender, EventArgs e)
        {
            rooms_avaliable.Visible = false;
            listBox1.Visible = false;
            create_room.Visible = false;
            refresh.Visible = false;

            food.Visible = true;
            colors.Visible = true;
            animals.Visible = true;
            choose_cat.Visible = true;
            choose_cat_ok.Visible = true;
            //***************************************************************************       
        }

        private void choose_cat_ok_Click(object sender, EventArgs e)
        {
            
            if (food.Checked == true)
            {
                category= food.Text;
            }
            else if (colors.Checked == true)
            {
                category = colors.Text;
            }
            else if (animals.Checked == true)
            {
                category = animals.Text;
            }
            choose_cat_ok.Visible = false;
            food.Visible = false;
            colors.Visible = false;
            animals.Visible = false;
            choose_cat.Visible = false;
            waiting.Visible = true;
            

           // MessageBox.Show(category);

            //send category to server
            substream = client.GetStream();
            StreamWriter wr = new StreamWriter(substream);
            wr.WriteLine(category);
            wr.AutoFlush = true;

            //receive room port
            net = client.GetStream();
            StreamReader rr = new StreamReader(net);
            room_port = int.Parse(rr.ReadLine());
         //   MessageBox.Show(room_port+"");

            //connect to the room
            clientroom = new TcpClient();
            clientroom.Connect("127.0.0.1", room_port);

            //read from the room when player2 connected
            NetworkStream ss = clientroom.GetStream();
            StreamReader r = new StreamReader(ss);

            MessageBox.Show(r.ReadLine());
            
             if(r.ReadLine()=="yes")
             { 
              enter_game.Visible = true;
                waiting.Visible = false;
            }

        }

        private void enter_game_Click(object sender, EventArgs e)
        {
           
            enter_game.Visible = false;
            Button btn;
            for (char ch = 'a'; ch <= 'z'; ch++)
            {
               btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                btn.Visible = true;
                btn.Enabled = true;
            }
            cat_lable.Visible = true;
            word_lable.Visible = true;
            dashline = "";
            cat_lable.Text = "Guess from " + category + " category";
            string[] words = File.ReadAllLines(category + ".txt");
            var ra = new Random();
            int randomLineNumber = ra.Next(0, words.Length);//random index
            randomword = words[randomLineNumber];
            for (int i = 0; i < randomword.Length; i++)
            {
                dashline += "_ ";
            }
            word_lable.Text = dashline;

            //send random word to the room server
            net = clientroom.GetStream();
            StreamWriter wr = new StreamWriter(net);
            wr.WriteLine(randomword);
            wr.AutoFlush = true;

            MessageBox.Show(randomword);
        }



        //check if win
        private Boolean win(char[] a)
        {
            int flag = 0;
            for (int i = 0; i < randomword.Length; i++)
            {
                if (a[i * 2] == randomword[i])
                {
                    flag++;
                }
            }
            //check if lable is full with letters
            if (flag == randomword.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //any button clicked in the game keyboard
        private void a_Click(object sender, EventArgs e)
        {
            //player1
            if (playertype != "player2" && playertype != "watcher")
            {
                Button pressedbutton = sender as Button;
                try
                {
                    pressedbutton.Enabled = false;
                    //to know when he choose wrong char
                    int flag = 0;
                  char []  arr = word_lable.Text.ToCharArray();
                    for (int i = 0; i < randomword.Length; i++)
                    {
                        if (pressedbutton.Text[0] == randomword[i])
                        {
                            arr[i * 2] = pressedbutton.Text[0];
                            word_lable.Text = new string(arr);
                            correctValue = true;
                            correctValuep2 = false;
                            flag = 1;
                        }
                        
                    }
                   
                    if (flag==0)//choose wrong char
                    {
                        correctValue = false;
                        correctValuep2 = true;
                        Button btn;
                        correctValuep2 = true;
                        for (char ch = 'a'; ch <= 'z'; ch++)
                        {
                            btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                            btn.Enabled = false;
                        }
                    }
                    //send the letter and the label's new word
                    NetworkStream network = clientroom.GetStream();
                    StreamWriter stream = new StreamWriter(network);
                    //1
                    p1SelectedChars.Add(pressedbutton.Text);
                    stream.WriteLine(pressedbutton.Text[0] + "*" + word_lable.Text + "*" + correctValue);
                    stream.Flush();
                    // MessageBox.Show(pressedbutton.Text[0] + "*" + word_lable.Text + "*" + correctValue );


                    
                    while (correctValue == false)//player2 played
                    {
                       
                        //receive player2 answers
                        NetworkStream networkStream = clientroom.GetStream();
                        StreamReader streamReader = new StreamReader(networkStream);
                        string p1Response = streamReader.ReadLine();
                        if (p1Response.Split('*')[1] == "PA")
                        {
                            MessageBox.Show("You lose! start anthor game");
                            dashline = "";
                            randomword = p1Response.Split('*')[0];
                         //   Array.Clear(arr, 0, arr.Length);
                            for (int i = 0; i < randomword.Length; i++)
                            {
                                dashline += "_ ";
                            }
                            word_lable.Text = dashline;
                            word_lable.Refresh();
                            p1SelectedChars.Clear();
                            MessageBox.Show(randomword);
                        }
                        else
                        {
                            string[] newLabel = p1Response.Split('*');
                            p1SelectedChars.Add(newLabel[0]);

                            if (word_lable.Text != newLabel[1])
                            {
                                word_lable.Text = newLabel[1];
                                //MessageBox.Show(p1Response);
                                word_lable.Refresh();
                            }
                            correctValue = Convert.ToBoolean(newLabel[2]);
                            if (correctValue == true)//player2 choose wrong char
                            {
                                Button btn;

                                for (char ch = 'a'; ch <= 'z'; ch++)
                                {
                                    btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                                    btn.Enabled = true;
                                    for (int i = 0; i < p1SelectedChars.Count; i++)
                                    {
                                        if (ch.ToString() == p1SelectedChars[i])
                                        {
                                            btn.Enabled = false;
                                        }

                                    }
                                }
                                correctValuep2 = false;

                            }
                        } 

                    }
                    if (win(arr))
                    {
                      
                         MessageBox.Show("You Win"); 
                        play_again_lable.Visible = true;
                        accept_playing_again.Visible = true;
                        refuse_playing_again.Visible = true;
                        Button btn;
                        for (char ch = 'a'; ch <= 'z'; ch++)
                        {
                            btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                            btn.Visible = false;
                        }
                        cat_lable.Visible = false;
                        word_lable.Visible = false;
                       // Array.Clear(arr, 0, arr.Length);

                    }



                }


                catch (Exception u)
                {
                    MessageBox.Show(u.ToString());

                }
            }
            //player2
            else if (playertype == "player2")
            {

                Button pressedbutton = sender as Button;
                try
                {
                    pressedbutton.Enabled = false;
                    int flag = 0;
                   
                   char [] arr = word_lable.Text.ToCharArray();
                    for (int i = 0; i < randomword.Length; i++)
                    {
                        if (pressedbutton.Text[0] == randomword[i])
                        {
                            
                            arr[i * 2] = pressedbutton.Text[0];
                            word_lable.Text = new string(arr);
                            correctValue = false;
                            correctValuep2 = true;
                            flag = 1;
                        }

                    }
                   
                    if (flag == 0)//player2 choose wrong char
                    {
                        correctValue = true;
                        correctValuep2 = false;
                        Button btn;
                        for (char ch = 'a'; ch <= 'z'; ch++)
                        {
                            btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                            btn.Enabled = false;
                        }
                    }
                    //send the letter and the label's new word
                    NetworkStream network = clientroom.GetStream();
                    StreamWriter stream = new StreamWriter(network);

                    p2SelectedChars.Add(pressedbutton.Text);
                    stream.WriteLine(pressedbutton.Text[0] + "*" + word_lable.Text + "*" + correctValue);
                    stream.Flush();
                    //MessageBox.Show(pressedbutton.Text[0] + "*" + word_lable.Text + "*" + correctValue);


                   
                    while (correctValuep2 == false)
                    {
                        //receive player1 answers
                        NetworkStream networkStream = clientroom.GetStream();
                        StreamReader streamReader = new StreamReader(networkStream);
                        string p1Response = streamReader.ReadLine();
                        if (p1Response.Split('*')[1] == "PA")
                        {
                            MessageBox.Show("You lose! start anthor game");
                            dashline = ""; 
                            randomword = p1Response.Split('*')[0];
                           // Array.Clear(arr, 0, arr.Length);
                            for (int i = 0; i < randomword.Length; i++)
                            {
                                dashline += "_ ";
                            }
                            word_lable.Text = dashline;
                            word_lable.Refresh();
                            p2SelectedChars.Clear();
                            MessageBox.Show(randomword);
                        }
                        else
                        {
                            string[] newLabel = p1Response.Split('*');
                            p2SelectedChars.Add(newLabel[0]);
                            if (word_lable.Text != newLabel[1])
                            {
                                word_lable.Text = newLabel[1];
                                word_lable.Refresh();
                                //MessageBox.Show(p1Response);
                            }
                            correctValue = Convert.ToBoolean(newLabel[2]);//player1 choose wrong char
                            if (correctValue == false)
                            {
                                Button btn;
                                for (char ch = 'a'; ch <= 'z'; ch++)
                                {
                                    btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                                    btn.Enabled = true;
                                    for (int i = 0; i < p2SelectedChars.Count; i++)
                                    {
                                        if (ch.ToString() == p2SelectedChars[i])
                                        {
                                            // MessageBox.Show(ch + " inside if");
                                            btn.Enabled = false;
                                        }

                                    }
                                }
                                correctValuep2 = true;
                            }

                        }
                    }

                    if (win(arr))
                    {
                       
                         MessageBox.Show("You Win"); 
                        
                        play_again_lable.Visible = true;
                        accept_playing_again.Visible = true;
                        refuse_playing_again.Visible = true;
                        Button btn;
                        for (char ch = 'a'; ch <= 'z'; ch++)
                        {
                            btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                            btn.Visible = false;
                        }
                        cat_lable.Visible = false;
                        word_lable.Visible = false;
                       // Array.Clear(arr, 0, arr.Length);

                    }

                }
                catch (Exception u)
                {
                    MessageBox.Show(u.ToString());
                   

                }


            }
        }

            //play again 

            private void accept_playing_again_Click(object sender, EventArgs e)
            {
            play_again_lable.Visible = false;
            accept_playing_again.Visible = false;
            refuse_playing_again.Visible = false;

           


            Button btn;
            for (char ch = 'a'; ch <= 'z'; ch++)
            {
                btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                btn.Visible = true;
                btn.Enabled = true;
            }
            cat_lable.Visible = true;
            word_lable.Visible = true;
            dashline = "";
            cat_lable.Text = "Guess from " + category + " category";
            string[] words = File.ReadAllLines(category + ".txt");
            var ra = new Random();
            int randomLineNumber = ra.Next(0, words.Length);
            randomword = words[randomLineNumber];
            for (int i = 0; i < randomword.Length; i++)
            {
                dashline += "_ ";
            }
            word_lable.Text = dashline;

            net = clientroom.GetStream();
            StreamWriter wr = new StreamWriter(net);
            wr.WriteLine("PA"+"*"+randomword);
            wr.AutoFlush = true;

            MessageBox.Show(randomword);


        }





        //cancle
        private void refuse_playing_again_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string text = listBox1.GetItemText(listBox1.SelectedItem);
            string num = text.Substring(0, 4);
           
            clientroom = new TcpClient();
            clientroom.Connect("127.0.0.1", int.Parse(num));
            substream = clientroom.GetStream();
            StreamReader rr = new StreamReader(substream);

            MessageBox.Show(rr.ReadLine());
            net=client.GetStream();
            StreamWriter w = new StreamWriter(net);
            w.WriteLine("connected in roomport " + num);
            w.Flush();



            rooms_avaliable.Visible = false;
            listBox1.Visible = false;
            create_room.Visible = false;
            refresh.Visible = false;

            substream = clientroom.GetStream();
            StreamReader rrr = new StreamReader(substream);
            
              string [] recived = rrr.ReadLine().Split('*');

            randomword = recived[0];
            category = recived[1];
            playertype = recived[2];


            waiting.Visible = false;
            enter_game.Visible = false;
            if (playertype == "player2")
            {
                Button btn;
                for (char ch = 'a'; ch <= 'z'; ch++)
                {
                    btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                    btn.Visible = true;
                    btn.Enabled = false;
                    
                }

                cat_lable.Visible = true;
                word_lable.Visible = true;
                cat_lable.Text = "Guess from " + category + " category";
                dashline = "";
                for (int i = 0; i < randomword.Length; i++)
                {
                    dashline += "_ ";
                }
                word_lable.Text = dashline;

                MessageBox.Show(randomword);

                while (correctValue == true)
                {
                    //receive p1 answers
                    NetworkStream networkStream = clientroom.GetStream();
                    StreamReader streamReader = new StreamReader(networkStream);
                    string p1Response = streamReader.ReadLine();

                    string[] newLabel = p1Response.Split('*');
                    p2SelectedChars.Add(newLabel[0]);
                    //if p1's word label is not the same as p2's then apply it
                    //if p1's word label is the same as p2's label then p1 probably didn't choose the correct letter
                    //so we have to switch players
                    if (word_lable.Text != newLabel[1])
                    {
                        word_lable.Text = newLabel[1];
                        word_lable.Refresh();
                     //   MessageBox.Show(p1Response);
                    }
                    correctValue = Convert.ToBoolean(newLabel[2]);
                    if (correctValue == false)
                    {

                        for (char ch = 'a'; ch <= 'z'; ch++)
                        {
                            btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                            btn.Enabled = true;
                            for (int i = 0; i < p2SelectedChars.Count; i++)
                            {
                                if (ch.ToString() == p2SelectedChars[i])
                                {
                                   
                                    btn.Enabled = false;
                                }

                            }
                        }
                        correctValuep2 = true;
                    }
                }

         


            }
            if(playertype=="watcher")
            {
                Button btn;
                for (char ch = 'a'; ch <= 'z'; ch++)
                {
                    btn = this.Controls.Find("" + ch, true).FirstOrDefault() as Button;
                    btn.Visible = true;
                    btn.Enabled = false;
                }

                cat_lable.Visible = true;
                word_lable.Visible = true;
                cat_lable.Text = "Guess from " + category + " category";
                dashline = "";
                for (int i = 0; i < randomword.Length; i++)
                {
                    dashline += "_ ";
                }
                word_lable.Text = dashline;

                cat_lable.Enabled = false;
                word_lable.Enabled = false;
                MessageBox.Show(randomword);
            }

        }
    }
    }

