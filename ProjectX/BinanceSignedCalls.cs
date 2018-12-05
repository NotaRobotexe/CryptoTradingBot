using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectX
{
    class BinanceSignedCalls
    {
        static string SecKey = "";
        static string ApiKey = "";

        public static void Init(string _SecKey, string _ApiKey)
        {
            SecKey = _SecKey;
            ApiKey = _ApiKey;
        }

        public static void GetOpenOrder()
        {
            long date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string msg = "recvWindow=5000&timestamp=" + date;
            string HwacMsg = GetHash(msg);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("X-MBX-APIKEY:"+ApiKey);
                string orders  = client.DownloadString("https://api.binance.com/api/v3/openOrders?recvWindow=5000&timestamp=" + date+"&signature=" + HwacMsg);
                Console.WriteLine(orders);
            }
        }

        public static decimal GetFund(string pair) //this shit has weight 5
        {
            long date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string msg = "recvWindow=5000&timestamp=" + date;
            string HwacMsg = GetHash(msg);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("X-MBX-APIKEY:" + ApiKey);
                string orders = client.DownloadString("https://api.binance.com/api/v3/account?recvWindow=5000&timestamp=" + date + "&signature=" + HwacMsg);
                var json = orders;
                dynamic result = JsonConvert.DeserializeObject(json);

                foreach (var obj_asset in result.balances)
                {
                    if (obj_asset.asset == pair.ToUpper())
                    {
                        return obj_asset.free;
                    }
                }
            }

            return -1;
        }

        public static void NewOrder()
        {
            long date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string msg = "recvWindow=5000&timestamp=" + date;
            string HwacMsg = GetHash(msg);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("X-MBX-APIKEY:" + ApiKey);
                string orders = client.DownloadString("https://api.binance.com/api/v3/order/test?recvWindow=5000&timestamp=" + date + "&signature=" + HwacMsg);
                Console.WriteLine(orders);
            }
        }


        private static string GetHash(string msg)
        {
            var hash = HashHMAC(StringEncode(SecKey), StringEncode(msg));
            return HashEncode(hash);
        }

        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new System.Security.Cryptography.HMACSHA256(key);
            return hash.ComputeHash(message);
        }
    }
}
