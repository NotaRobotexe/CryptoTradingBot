using System;
using WebSocketSharp;

namespace ProjectX
{
    internal class Naive_
    {
        private static Conditions conditions; //universal buying condition for all currency if you dont like it gtfo

        private string pair;
        private int PriceHistory = 10;
        private decimal Price;                                          //Price is how much will be bought
        private string interval = "1m";
        private int paid = 0;

        private Candlestick[] LastPrice;
        private WebSocket WebSckStream;

        public Naive_(string pair, int MaxOpenOrder, decimal Price)
        {
            this.pair = pair;
            this.Price = Price;

            LastPrice = new Candlestick[MaxOpenOrder];
            Candlestick[] candleHistory = BinanceSimpleCall.GetCandleStickts(pair, PriceHistory, interval);
            for (int i = 0; i < PriceHistory; i++)
            {
                LastPrice[i] = candleHistory[9 - i];
            }
        }

        public void AnalyzeNewData(dynamic DynData)
        {
            Candlestick NewCandle = new Candlestick();

            NewCandle.open = DynData.data.k.o;
            NewCandle.close = DynData.data.k.c;
            NewCandle.high = DynData.data.k.h;
            NewCandle.low = DynData.data.k.l;
            NewCandle.name = DynData.data.s;

            NewCandle.percentage = ((decimal)DynData.data.k.c / (decimal)DynData.data.k.o) - 1;

            UpdateCandleHistory(NewCandle);
        }

        private void UpdateCandleHistory(Candlestick NewCandle)
        {
            var temporaryArray = new Candlestick[PriceHistory];
            temporaryArray[0] = NewCandle;

            Array.Copy(LastPrice, 0, temporaryArray, 1, PriceHistory - 1);

            LastPrice = temporaryArray;

            AnalyseMarket();

        }

        private void AnalyseMarket() //decide if buy
        {
            int MetConditions = 0;
            for (int i = 0; i < conditions.AmountCondition; i++)
            {
                if (LastPrice[i].percentage <= conditions.ToPercentage[i] && LastPrice[i].percentage >= conditions.FromPercentage[i])
                    MetConditions++;
                else if (LastPrice[i].percentage >= conditions.ToPercentage[i] && LastPrice[i].percentage <= conditions.FromPercentage[i])
                    MetConditions++;
                else
                    break;
            }

            if (MetConditions == conditions.AmountCondition)
            {
                BuyCrypto();
            }
        }

        private void BuyCrypto()
        {
            BuyLog(LastPrice[0].close);
        }

        public void BuyLog(decimal _price)
        {
            DateTime time = DateTime.UtcNow;
            time = time.AddHours(2);                   //Nooo treba si dat pozor ked sa zmeni cas na zimny lebo to nebude fungovat dobre :D

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Bought: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(pair);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" Price: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(_price);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" Paid: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(paid);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" Time: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(time + "\n");

            PlayAlert();

        }

        public static void SetBuyConditions(String From, String To, int amount)
        {
            string[] from_ = From.Split(',');
            string[] to_ = To.Split(',');

            decimal[] fromPr = new decimal[amount];
            decimal[] toPr = new decimal[amount];

            for (int i = 0; i < amount; i++)
            {
                fromPr[i] = Convert.ToDecimal(from_[i]);
                toPr[i] = Convert.ToDecimal(to_[i]);
            }

            conditions.AmountCondition = amount;
            conditions.FromPercentage = fromPr;
            conditions.ToPercentage = toPr;
        }

        private static void PlayAlert()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = "C:\\Users\\mt2si\\Desktop\\projekty\\FilthyPaycheck\\ProjectX\\alert.wav";
            player.Play();
        }
    }

    internal struct OpenOrders
    {
        public int id;
        public decimal StartPrice;
        public decimal TargerSellPrice;
        public DateTime OpenTime;
    }

    internal struct Conditions
    {
        public int AmountCondition;
        public decimal[] FromPercentage;
        public decimal[] ToPercentage;
    }
}