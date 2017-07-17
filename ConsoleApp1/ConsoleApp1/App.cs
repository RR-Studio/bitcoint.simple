using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace ConsoleApp1
{
   
    public partial class App
    {
        public static Network Network
        {
            get
            {
                return Network.TestNet;
            }
        }

        public static string AppDir
        {
            get
            {
                return Directory.GetParent(typeof(App).Assembly.Location).FullName;
            }
        }

        public static object Saving = new object();
       
    }
}
