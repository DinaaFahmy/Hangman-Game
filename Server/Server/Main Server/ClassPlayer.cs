using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Main_Server
{
    class ClassPlayer
    {
        public string playerName { set; get; }
        private bool flag { set; get; }
        public int counter = 0;
        //public int port;
      
        public ClassPlayer(string name)
        {
            this.playerName = name;
            counter++;
            // this.playerName = name;

        }
    }
}
