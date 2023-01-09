using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using TMD.Coinbase.PricePrediction.Services;
using TMDInvestment;
using TMDInvestment.Models.Helpers;
using TMDInvestment.Models;
using TMDInvestments.Models;

namespace TMD.Coinbase.PricePrediction
{
    class Program
    {
        public static DateTime time = DateTime.MinValue;
        public const decimal SELLFEE = .005m;
        public const decimal SELLMARGIN = .0095m;
        public const decimal MAXBALANCEPERCENT = .2m;
        public const decimal TICKERBUYRANGE = 1.1m;
        public const decimal TICKERSELLRANGE = .9m;
        public static decimal SellTicker = 0;
        public static decimal lowLow = 0m;
        public static decimal highHigh = 0m;
        public static List<string> positions = new List<string>();
        
        static void Main(string[] args)
        {
            Console.Title = "Coinbase Bot";
            while (true)
            {
                System.Threading.Thread.Sleep(10000);
                Stopwatch sw = new Stopwatch();
                RunProcess(ref sw);
                TimeSpan ts = sw.Elapsed;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Runtime " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10));
                System.Threading.Thread.Sleep(10000);
                //Console.ReadKey();
            }
        }

        private static void RunProcess(ref Stopwatch sw)
        {
            sw.Start();
            CoinBaseClientService service = new CoinBaseClientService();
            List<Coins> availCoins = new List<Coins>();
            time = DateTime.Now;
            Console.WriteLine("Start time of session: " + time);
            decimal AccountBalance = 0;
            decimal AvailiableForTrade = 0;
            decimal AmountPerTrade = 0;
            decimal TotalinEquities = 0;
            HistoricData LastTicker;
            int amtAvailForTrade = 0;

            TradeDirection direction;

            var settings = (AccountInfo)service.GetAccountSettings();
            //get current crypto holdings
            var accounts = service.GetAccountBalance();
            //get coins list for trading
            var coins = service.GetCoins();

            if (accounts != null && coins != null && settings != null)
            {
                positions = accounts.Where(x => x.currency != "USD" && x.currency != "USDC" && x.currentAmount > 5m).Select(x => x.currency + "-USD").ToList();
                amtAvailForTrade = (positions.Count >= 5) ? 0 : (int)settings.MaxRoundTradesPerDay - positions.Count;
                availCoins = coins.Where(x => !positions.Contains(x.Coin)).Take(amtAvailForTrade).ToList();
                Reflection<AccountInfo>.GetAllProperties(settings);
                //get balance/availiabe for trade and current crypto holdings
                var bal = ((List<Accounts>)accounts).Where(x => x.currency == "USD").FirstOrDefault();
                var avails = ((List<Accounts>)accounts).Where(x => (x.currency != "USD" && x.currency != "USDC"));

                AvailiableForTrade = Convert.ToDecimal(bal.balance);

                //list of coins that are not already trading
                int amt = amtAvailForTrade;

                //Get balance of all equities in account
                TotalinEquities = avails.Sum(x => Convert.ToDecimal(x.available) * Convert.ToDecimal(x.ticker.price));
                AccountBalance = TotalinEquities + AvailiableForTrade;
                var minReserve = AccountBalance * MAXBALANCEPERCENT;
                if (AvailiableForTrade > minReserve)
                {
                    //prevent divide by zero
                    if (amt != 0)
                        AmountPerTrade = decimal.Round((AvailiableForTrade - minReserve) / amt, 2);
                }

                DisplayBalanceandEquities(AccountBalance, TotalinEquities, AvailiableForTrade, minReserve, AmountPerTrade, amt);

                if (coins.Count > 0)
                    foreach (var coin in coins)
                    {
                        if (coin.ticker != null && coin.history != null && coin.history.Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            highHigh = decimal.Round(coin.history.Max(x => x.high), 2);
                            LastTicker = coin.history.OrderByDescending(x => x.time).FirstOrDefault();
                            var avail = accounts.Find(x => x.currency == coin.Coin.Replace("-USD", ""));
                            if (avail != null && avail.order != null && avail.currentAmount > 5)
                            {
                                //deal with existing trades
                                if (avail.order != null && avail.order.Count > 0 && avail.order[0].fills != null && avail.order[0].fills.Count > 0)
                                {
                                    if ((Convert.ToDecimal(avail.ticker.price) > avail.broughtTicker) && (Convert.ToDecimal(avail.ticker.price) >= highHigh * TICKERSELLRANGE) && (avail.currentAmount > (avail.broughtAmount + (avail.broughtAmount * SELLMARGIN) + (avail.broughtAmount * SELLFEE))))
                                    {
                                        direction = TradeDirection.sell;
                                        Console.WriteLine(direction.ToString() + " " + coin.Coin + " at " + Convert.ToDecimal(avail.ticker.price) + " for " + avail.currentAmount + " brought ticker " + avail.broughtTicker + " with MARGIN of " + SELLMARGIN + " Brought w FEEs for " + avail.broughtAmount);
                                        var order = service.Trade(coin.Coin, avail.currentAmount, direction);
                                        if (order != null)
                                        {
                                            Console.WriteLine("Order successfull for sale of " + order.product_id);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Order failed for " + coin.Coin);
                                        }

                                    }
                                    else
                                    {
                                        Console.WriteLine("Cannot Sell " + coin.Coin + " at " + Convert.ToDecimal(avail.ticker.price) + " for " + avail.currentAmount + " Brought w FEEs for " + avail.broughtAmount + " at ticker " + avail.broughtTicker + " current amount with MARGIN and FEE of " + (SELLMARGIN + SELLFEE) + " should be " + (avail.broughtAmount + (avail.broughtAmount * SELLMARGIN) + (avail.broughtAmount * SELLFEE)) + " or higher.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Order not found for account item " + avail.profile_id);
                                }
                            }
                            else
                            {
                                // only trade availCoin to trade. otherwise skip it.
                                var evalCoin = availCoins.Where(x => x.Coin == coin.Coin).FirstOrDefault();
                                //evaluate historic data
                                //do evaluation if true than trade
                                if (evalCoin != null)
                                {
                                    if (AnalyzeCoin(evalCoin))
                                    {
                                        //try a new trade
                                        if (AmountPerTrade >= 10)
                                        {
                                            direction = TradeDirection.buy;
                                            Console.WriteLine(direction.ToString() + " " + evalCoin.Coin + " at " + evalCoin.ticker.price + " for " + AmountPerTrade);
                                            var order = service.Trade(evalCoin.Coin, AmountPerTrade, direction);
                                            if (order != null)
                                            {
                                                //Console.WriteLine("Order successfull for buy of " + order.product_id);
                                                Console.WriteLine("Order successfull for buy of " + evalCoin.Coin);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Order failed for " + evalCoin.Coin);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Not enough fund to trade " + evalCoin.Coin + " at " + Convert.ToDecimal(avail.ticker.price));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Current Ticker for " + evalCoin.Coin + " at " + Convert.ToDecimal(avail.ticker.price) + ". Previous history open " + LastTicker.open + " on date " + LastTicker.time + ", Last Sale Price " + SellTicker + ". Last balance amount " + avail.currentAmount + ". Skip Trading. ");
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(coin.Coin + " Not availiable for trade due to rule");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ticker and/or History for " + coin.Coin + " Not found. Skipping.");
                        }

                    }

            }
            else
            {
                Console.WriteLine("Accounts or Coins has returned null");
            }
            Console.WriteLine(Environment.NewLine);
            sw.Stop();
        }
        //find low high range of price. check current with last, low, range since last sell
        private static bool AnalyzeCoin(Coins coin)
        {
            bool buy = false;
            decimal openAvg = 0m;
            decimal highAvg = 0m;
            decimal lowAvg = 0m;
            decimal lowHigh = 0m;
            decimal highLow = 0m;
            decimal closeAvg = 0m;
            decimal lastOpen = 0m;
            DateTime lastOpenDate = DateTime.MinValue;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            double numOfDays = 0f;
            decimal priceatLastFill = 0m;
            DateTime dateofLastOrder = DateTime.MinValue;

            decimal currentTicker = 0m;
            DateTime lastOrderDate = DateTime.MinValue;

            SellTicker = (coin.order != null && coin.order.Count > 0 && coin.order[0].fills != null) ? coin.order[0].fills.Where(x => x.side == "sell").Sum(x => Convert.ToDecimal(x.price)) : 0m;
            currentTicker = Convert.ToDecimal(coin.ticker.price);
            lastOrderDate = (coin.order != null && coin.order.Count > 0) ? DateTime.Parse(coin.order[0].done_at) : DateTime.MinValue;

            openAvg = decimal.Round(coin.history.Average(x => x.open), 2);
            highAvg = decimal.Round(coin.history.Average(x => x.high), 2);
            lowAvg = decimal.Round(coin.history.Average(x => x.low), 2);
            closeAvg = decimal.Round(coin.history.Average(x => x.close), 2);
            highHigh = decimal.Round(coin.history.Max(x => x.high), 2);
            highLow = decimal.Round(coin.history.Min(x => x.high), 2);
            lowHigh = decimal.Round(coin.history.Max(x => x.low), 2);
            lowLow = decimal.Round(coin.history.Min(x => x.low), 2);
            lastOpen = decimal.Round(coin.history.OrderByDescending(x => x.time).ToList()[0].open, 2);
            lastOpenDate = coin.history.OrderByDescending(x => x.time).ToList()[0].time;
            startDate = coin.history.Min(x => x.time);
            endDate = coin.history.Max(x => x.time);
            numOfDays = (endDate - startDate).TotalDays;

            //if (SellTicker > 0m && (currentTicker < SellTicker && range >= MAXBUYRANGE))
            //{
            //    buy = true;
            //}
            //else if ((daysSincelastOrder >= MAXDAYSSINCELASTORDER || SellTicker <= 0m) && (currentTicker > lowLow && currentTicker < highHigh))
            //{
            //    buy = true;
            //}
            //current ticker must be within 5% of avg low in last ten days.
            if (currentTicker >= lowLow && currentTicker <= (TICKERBUYRANGE * lowLow))
            {
                buy = true;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Avg for Symbol " + coin.Coin);
            Console.WriteLine("Start time " + startDate + " End time " + endDate);
            Console.WriteLine("Number of days calculated " + numOfDays);
            Console.WriteLine("Current " + coin.ticker.price);
            Console.WriteLine("Last Open " + lastOpen + " on " + lastOpenDate);
            Console.WriteLine("Open Avg " + openAvg);
            Console.WriteLine("High high " + highHigh);
            Console.WriteLine("High Avg " + highAvg);
            Console.WriteLine("High low " + highLow);
            Console.WriteLine("Low high " + lowHigh);
            Console.WriteLine("Low Avg " + lowAvg);
            Console.WriteLine("Low low " + lowLow);
            Console.WriteLine("Close Avg " + closeAvg);
            //Console.WriteLine("Days since last order " + daysSincelastOrder);
            Console.WriteLine("Last order date " + lastOrderDate);
            if (coin.order != null && coin.order.Count > 0 && coin.order[0].fills != null)
            {
                priceatLastFill = decimal.Round(coin.order.Average(x => x.fills.Average(y => Convert.ToDecimal(y.price))), 2);
                dateofLastOrder = DateTime.Parse(coin.order[0].done_at);
                Console.WriteLine("Avg Price at last Order " + priceatLastFill + " at Date Time " + dateofLastOrder);
            }
            else
            {
                Console.WriteLine("Skipping " + coin.Coin + " analysis as order not found");
            }
            Console.WriteLine(Environment.NewLine);
            return buy;
        }

        private static void DisplayBalanceandEquities(decimal AccountBalance, decimal TotalinEquities, decimal AvailiableForTrade, decimal minReserve, decimal AmountPerTrade, int amt)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Your total acount balance " + AccountBalance);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Your total in Equities " + TotalinEquities);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Your current cash balance availiable trade is " + AvailiableForTrade);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Minium reserve amt " + minReserve);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Amount per Trade " + AmountPerTrade);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Commodities availiable for trade " + amt);

            //Console.ForegroundColor = ConsoleColor.Magenta;
            //Console.WriteLine("Getting CoinBase Orders and Fills");
            //Console.WriteLine(Environment.NewLine);
        }
        //private static void EvaluateEligibility(Coins coins, Accounts accounts)
        //{

        //}
        private void DisplayOrdersandFills(List<Accounts> accounts)
        {
            foreach (var item in accounts)
            {
                if (item.currency != "USD" && item.currency != "USDC")
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Ticker for " + item.currency);
                    Console.WriteLine(Environment.NewLine);

                    if (item.order != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Getting CoinBase Account Orders for " + item.currency);
                        Console.WriteLine(Environment.NewLine);
                        if (item.order != null)
                            Reflection<Order>.GetAllPropertiesArray(item.order);
                        Console.WriteLine(Environment.NewLine);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Fills for " + item.currency);
                        foreach (var order in item.order)
                        {
                            if (order.fills != null)
                            {
                                foreach (var fill in order.fills)
                                {
                                    Reflection<Fills>.GetAllProperties(fill);
                                    Console.WriteLine(Environment.NewLine);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Fills missing for item " + item.order);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Item order missing for " + item.currency);
                    }
                }
            }
        }
    }
}
