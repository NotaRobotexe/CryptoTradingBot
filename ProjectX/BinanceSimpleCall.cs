using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX
{
    class BinanceSimpleCall
    {
        public static Candlestick[] GetCandleStickts(string symbol,int quantity,string interval)
        {
            Candlestick[] candlestick = new Candlestick[quantity];
            using (WebClient client = new WebClient())
            {
                var json = client.DownloadString("https://api.binance.com/api/v1/klines?symbol="+symbol+"&interval="+interval+"&limit="+quantity);
                dynamic result = JsonConvert.DeserializeObject(json);

                for (int i = 0; i < quantity; i++)
                {
                    candlestick[i].open = result[i][1];
                    candlestick[i].close = result[i][4];
                    candlestick[i].high = result[i][2];
                    candlestick[i].low = result[i][3];
                    candlestick[i].name = symbol;
                    candlestick[i].percentage = ((decimal)result[i][4] / (decimal)result[i][1]) - 1;
                }

            }

            return candlestick;
        }

        public static decimal GetCurrentPrice(string symbol)
        {
            using (WebClient client = new WebClient())
            {
                var json = client.DownloadString("https://api.binance.com/api/v1/ticker/price?symbol="+symbol);
                dynamic result = JsonConvert.DeserializeObject(json);
                var pr = result.price;
                return pr;
            }
        }
    }
}
