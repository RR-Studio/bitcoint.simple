using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Protocol;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace ConsoleApp1
{
    /// <summary>
    /// Тестовые примеры транзакий
    /// </summary>
    public class AppTest
    {
        public static void Transaction1()
        {
            var secret = new BitcoinSecret("cQMQ1QMAv7yrt5EAW4Noc7B13wZDtu16iqJLpunmHfLdJb6u6F8N", Network.TestNet);
            var key = secret.PrivateKey;


            var tx = new Transaction();

            var input = new TxIn();
            input.PrevOut = new OutPoint(uint256.Parse("61072b6b5bff32419237a1789ba53edace6bf5d781efdbed821a3a1f4ce307a1"), 1);
            input.ScriptSig = secret.GetAddress().ScriptPubKey;
            tx.AddInput(input);

            var output = new TxOut();
            var destination = BitcoinAddress.Create("mvcAQXvzhi3UXBq2VRMMGP4SGEgejUVEkh", Network.TestNet);
            var fee = Money.Coins(0.1m);
            output.Value = Money.Coins(0.3m) - fee;
            output.ScriptPubKey = destination.ScriptPubKey;
            tx.AddOutput(output);

            tx.Sign(secret, false);
            Console.WriteLine("GetHash {0}", tx.GetHash());

            var node = Node.Connect(Network.TestNet, "107.180.70.52:18333");
            node.MessageReceived += NodeOnMessageReceived;
            node.VersionHandshake();
            node.SendMessage(new InvPayload(tx));
            node.SendMessage(new TxPayload(tx));
            Console.Read();
            Console.WriteLine("Disconet");
            node.Disconnect();
        }

        private static void NodeOnMessageReceived(Node node1, IncomingMessage message)
        {
            Console.WriteLine(message.Message);
        }

        /// <summary>
        /// транзакция с использованием библиотеки QBitNinjaClient
        /// </summary>
        public static void Transaction2()
        {
            var secret = new BitcoinSecret("cQMQ1QMAv7yrt5EAW4Noc7B13wZDtu16iqJLpunmHfLdJb6u6F8N", Network.TestNet);
            var key = secret.PrivateKey;


            var tx = new Transaction();

            var input = new TxIn();
            input.PrevOut = new OutPoint(uint256.Parse("61072b6b5bff32419237a1789ba53edace6bf5d781efdbed821a3a1f4ce307a1"), 1);
            input.ScriptSig = secret.GetAddress().ScriptPubKey;
            tx.AddInput(input);

            var output = new TxOut();
            var destination = BitcoinAddress.Create("mvcAQXvzhi3UXBq2VRMMGP4SGEgejUVEkh", Network.TestNet);
            var fee = Money.Coins(0.1m);
            output.Value = Money.Coins(0.3m) - fee;
            output.ScriptPubKey = destination.ScriptPubKey;
            tx.AddOutput(output);

            tx.Sign(secret, false);
            Console.WriteLine("GetHash {0}", tx.GetHash());

            var client = new QBitNinjaClient(Network.TestNet);
            BroadcastResponse broadcastResponse = client.Broadcast(tx).Result;

            if (!broadcastResponse.Success)
            {
                Console.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success!");
                var transactionId = uint256.Parse("61072b6b5bff32419237a1789ba53edace6bf5d781efdbed821a3a1f4ce307a1");
                // Query the transaction
                GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;

                Console.WriteLine(transactionResponse.TransactionId); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
                Console.WriteLine(tx.GetHash());
            }
        }
    }
}
