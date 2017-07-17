using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin.OpenAsset;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NBitcoin.SPV;
using NBitcoin.Stealth;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace ConsoleApp1
{
    class Program
    {
        private static List<string> Nodes;
        static void Main(string[] args)
        {
            //  AppTest.Transaction1();
            Nodes = new List<string>();
            var app = new SearchNodes();
            app.OnChangeNodesCollection += OnChangeNodesCollection;
            app.Search();
            Console.Read();
        }

        private static void OnChangeNodesCollection(object sender, NodesCollection nodesCollection)
        {
           // Console.Clear();
            foreach (var item in nodesCollection)
            {
                if (Nodes.All(_ => _ != $"{item.RemoteSocketAddress.MapToIPv4()}:{item.RemoteSocketPort}"))
                {
                    Nodes.Add($"{item.RemoteSocketAddress.MapToIPv4()}:{item.RemoteSocketPort}");
                    Console.WriteLine("{0}:{1}",item.RemoteSocketAddress.MapToIPv4(), item.RemoteSocketPort);
                    AppTest.Transaction1($"{item.RemoteSocketAddress.MapToIPv4()}:{item.RemoteSocketPort}");
                    
                }
            }
        }
    }
}
