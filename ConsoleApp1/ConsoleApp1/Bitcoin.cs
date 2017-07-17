using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.SPV;
using QBitNinja.Client;

namespace ConsoleApp1
{
    class Bitcoin
    {
        /// <summary>
        /// Создания кошелька
        /// </summary>
        /// <param name="network">Тип сети</param>
        /// <returns></returns>
        public DataWallet CreateWallet(Network network)
        {
            var wallet = new DataWallet();
            byte[] seed = RandomUtils.GetBytes(64);
            wallet.SeedHex = Encoders.Hex.EncodeData(seed);
            
            //Create Master Private Key with a seed
            ExtKey privateKey = new ExtKey(wallet.SeedHex);
            //create master public key from this privateKey
            ExtPubKey pubKey = privateKey.Neuter();
            //save it's wifStr as key to the next server to use and generate all child keys 
            string wifStr = pubKey.ToString(network);
            

            
            ExtPubKey key = ExtPubKey.Parse(wifStr);
            //The payment server receive an order, note the server does not need the private key to generate the address
            uint orderID = 1001;
            BitcoinAddress address = key.Derive(orderID).PubKey.GetAddress(network);
            wallet.Address = address.ToString();
            


            
            //Now on the server that have access to the private key, you get the private key from the orderID
            ExtKey mPrivateKey = new ExtKey(wallet.SeedHex);
            Key key1 = mPrivateKey.Derive(orderID).PrivateKey;
            BitcoinSecret secret = key1.GetBitcoinSecret(Network.TestNet);
            wallet.Secret = secret.ToString(); //Print a nice secret key string
            
            return wallet;
        }

        /// <summary>
        /// Получения баланса с по
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static decimal GetBalance(string walletAddress)
        {
            QBitNinjaClient client = new QBitNinjaClient(Network.TestNet);
            var address = BitcoinAddress.Create(walletAddress, Network.TestNet);
            var balanceModel = client.GetBalance(address, true).Result;
            if (balanceModel.Operations.Count == 0)
                return 0;
            var unspentCoins = new List<Coin>();
            foreach (var operation in balanceModel.Operations)
                unspentCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
            var balance = unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));

            return balance;
        }
    }

    public class DataWallet
    {
        /// <summary>
        /// Случайнное число 64 байта
        /// </summary>
        public string SeedHex { get; set; }
        /// <summary>
        /// Биткоин адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Биткоин секрет
        /// </summary>
        public string Secret { get; set; }
    }
}
