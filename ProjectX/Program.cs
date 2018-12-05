using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using WebSocketSharp;

namespace ProjectX
{
    internal class Program
    {
        private static string ApiKey = "";
        private static string SecretKey = "";
        private static WebSocket stream;
        static Naive_[] ActiveTrading;

        private static Dictionary<string, int> PairsID = new Dictionary<string, int>();

        private static void Main(string[] args)
        {
            Console.Write("ApiKey: ");
            ApiKey = Console.ReadLine();
            Console.Write("SecretKey: ");
            SecretKey = Console.ReadLine();

            double a = 100;
            int b = 0;
            while (true)
            {
                b++;
                a *= 1.05;
                Console.WriteLine(b + " " +a);
                if (a>2800)
                {
                    Console.WriteLine(b);
                    break;
                }
            }
            Console.ReadLine();

            BinanceSignedCalls.Init(SecretKey,ApiKey);
            Console.WriteLine( BinanceSignedCalls.GetFund("xrp"));
            BinanceSignedCalls.GetOpenOrder();


            string[] TradingPairs = new string[] {"ADABTC", "BATBTC", "ETHBTC", "XRPBTC", "ICXBTC", "IOTABTC", "LTCBTC", "NEOBTC", "POABTC", "LINKBTC", "EOSBTC", "ONTBTC"};

            Naive_.SetBuyConditions("-0.002,-0.001,0.0005", "-0.02,-0.02,0.03", 3);
            ActiveTrading = new Naive_[TradingPairs.Length];

            for (int i = 0; i < TradingPairs.Length; i++)
            {
                ActiveTrading[i] = new Naive_(TradingPairs[i], 10, 1);
                PairsID.Add(TradingPairs[i], i);
            }

            LaunchStream(TradingPairs, "1m");

            Console.ReadLine();
        }

        private static void LaunchStream(string[] TradingPairs, string interval)
        {
            string pairs = "";
            foreach (var pair in TradingPairs)
            {
                pairs += pair.ToLower() + "@kline_" + interval + "/";
            }

            stream = new WebSocket("wss://stream.binance.com:9443/stream?streams=" + pairs);
            stream.OnMessage += StreamNewData;
            stream.Connect();
        }

        private static void StreamNewData(object sender, MessageEventArgs e)
        {
            dynamic DynData = JsonConvert.DeserializeObject(e.Data);
            if (DynData.data.k.x == true)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(ActiveTrading[PairsID[Convert.ToString(DynData.data.s)]].AnalyzeNewData));
                thread.Start(DynData);
            }
        }
    }
}