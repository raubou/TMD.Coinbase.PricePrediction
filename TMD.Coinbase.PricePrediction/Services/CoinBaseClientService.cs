using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using TMDInvestment.Models;
using TMDInvestment;
using TMDInvestments.Models;

namespace TMD.Coinbase.PricePrediction.Services
{
    class CoinBaseClientService : ClientCalls
    {
        //public static DateTime time = DateTime.MinValue;
        //public static List<List<HistoricData>> PriceHistory;
        //public static List<Coins> AllowableCoins;
        public CoinBaseClientService()
        {
            //time = DateTime.Now;
            //PriceHistory = new List<List<HistoricData>>();
            //AllowableCoins = new List<Coins>();
        }
        public dynamic GetAccountSettings()
        {
            //Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Getting DB Settings");
            var settings = GetSettings();
            //Console.WriteLine(settings);
            return settings;
        }
        public List<Accounts> GetAccountBalance()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Getting CoinBase Account Balances");
            Console.WriteLine(Environment.NewLine);
            var accounts = GetCoinBaseAccounts();
            if(accounts is List<Accounts> && accounts != null && accounts.Count > 0)
            {
                //Helpers.Reflection<Accounts>.GetAllPropertiesArray(accounts);
                //var usd = ((List<Accounts>)accounts).Find(x => x.currency == "USD");
                //if (usd != null)
                //{
                //    Console.ForegroundColor = ConsoleColor.White;
                //    Console.WriteLine("Your current cash balance to trade is " + usd.balance);
                //}
                //Console.WriteLine("Getting CoinBase Orders and Fills");
                //Console.WriteLine(Environment.NewLine);
                //foreach (var item in accounts)
                //{
                //    if (item.currency != "USD" && item.currency != "USDC")
                //    {
                //        Console.ForegroundColor = ConsoleColor.White;
                //        Console.WriteLine("Ticker for " + item.currency);
                //        Console.WriteLine(Environment.NewLine);
                //        var ticker = item.ticker;
                //        Helpers.Reflection<Ticker>.GetAllProperties(ticker);
                //        Console.WriteLine(Environment.NewLine);
                //        //Console.ForegroundColor = ConsoleColor.White;
                //        //Console.WriteLine("Getting CoinBase Product Ticker for " + item.currency);
                //        //item.ticker = GetCoinBaseProductTicker(item.currency + "-USD");
                //        //Helpers.Reflection<Ticker>.GetAllProperties(item.ticker);
                //        if(item.order != null)
                //        {
                //            Console.ForegroundColor = ConsoleColor.Magenta;
                //            Console.WriteLine("Getting CoinBase Account Orders for " + item.currency);
                //            Console.WriteLine(Environment.NewLine);
                //            //var ordersAndFills = GetCoinBaseOrderandFill(item.currency + "-USD");
                //            Helpers.Reflection<Order>.GetAllPropertiesArray(item.order);
                //            Console.WriteLine(Environment.NewLine);
                //            Console.ForegroundColor = ConsoleColor.Blue;
                //            Console.WriteLine("Fills for " + item.currency);
                //            foreach (var order in item.order)
                //            {
                //                //try
                //                //{
                //                if (order.fills != null)
                //                {
                //                    foreach (var fill in order.fills)
                //                    {
                //                        Helpers.Reflection<Fills>.GetAllProperties(fill);
                //                        Console.WriteLine(Environment.NewLine);
                //                    }
                //                }
                //                else
                //                {
                //                    Console.WriteLine("Fills missing for item " + item.order);
                //                }
                //            }
                //        }
                //        else
                //        {
                //            Console.WriteLine("Item order missing for " + item.currency);
                //        }
                //    }
                //}
                //Console.ReadLine();
                return accounts;
            }
            else
            {
                Console.WriteLine("Accounts returned empty");
            }
            return null;
        }

        //public void GetProducts()
        //{
        //    Console.ForegroundColor = ConsoleColor.Blue;
        //    Console.WriteLine("Getting CoinBase Availiable Products and Price History..");
        //    var products = GetCoinBaseProducts();
        //    //Helpers.Reflection<Product>.GetAllPropertiesArray(products);
        //    foreach(var item in products)
        //    {
        //        foreach(var coin in AllowableCoins)
        //        {
        //            if(coin.Coin.ToLower() == item.id)
        //                GetCoinBasePriceHistory(item.id);
        //        }

        //    }
        //    Console.WriteLine("Availiable Products and Price History Loaded.");
        //}

        //public void GetOrders()
        //{
        //    Console.ForegroundColor = ConsoleColor.Green;
        //    Console.WriteLine("Getting CoinBase Orders");
        //    var orders = GetCoinBaseOrders();
        //    //Console.WriteLine(orders);
        //    Helpers.Reflection<Order>.GetAllPropertiesArray(orders);
        //}

        //public void GetFills(string orderId)
        //{
        //    Console.ForegroundColor = ConsoleColor.DarkBlue;
        //    Console.WriteLine("Getting CoinBase fills for order " + orderId);
        //    var fills = GetCoinBaseFills(orderId);
        //    //Console.WriteLine(fills);
        //    Helpers.Reflection<List<Fills>>.GetAllProperties(fills);
        //}

        //public List<HistoricData> GetCoinBasePriceHistory(string product)
        //{
        //    bool productIsinList = false;
        //    dynamic historicData = null;
        //    List<HistoricData> currentProduct = new List<HistoricData>(); 
        //    //Console.ForegroundColor = ConsoleColor.DarkYellow;
        //    //Console.WriteLine("Getting CoinBase Price History for product " + product);
        //    //dynamic historicData = null;
        //    //if (PriceHistory.Count <= 0)
        //    //{
        //    foreach (var list in PriceHistory)
        //    {
        //        foreach(var priceList in list)
        //        {
        //            if (priceList.symbol == product)
        //            {
        //                productIsinList = true;
        //                currentProduct.AddRange(list);
        //                break;                        
        //            }
        //        }
        //    }
        //    if(!productIsinList)
        //    {
        //        historicData = GetHistoricData(product, DateTime.Now.AddDays(-10), DateTime.Now);
        //        currentProduct.AddRange(historicData);
        //        PriceHistory.Add(historicData);
        //    }
        //    //}

        //    Console.WriteLine("Historic Product Price: " + product);
        //    //Console.ForegroundColor = ConsoleColor.DarkCyan;
        //    Helpers.Reflection<HistoricData>.GetAllPropertiesArray(currentProduct);
        //    Console.WriteLine(" ");
        //    return historicData;
        //}

        public List<Coins> GetCoins()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Loading Allowable Coins from DB");
            Console.WriteLine(Environment.NewLine);
            var coins = GetAllowableCoins();
            if (coins is List<Coins> && coins != null && coins.Count > 0)
            {
                return coins;
            }
            else
            {
                Console.WriteLine("Coins returned empty");
            }
            return null;

            //if (AllowableCoins.Count <= 0)
            //{
            //    try
            //    {
            //        AllowableCoins.AddRange(GetAllowableCoins());
            //    }
            //    catch (Exception)
            //    {
            //        Console.WriteLine("Error on allowalbe coin retrieval");
            //    }                
            //}

            //foreach (var coin in AllowableCoins)
            //{
            //    Console.WriteLine("Getting ticker for " + coin.Coin);
            //    Console.WriteLine(Environment.NewLine);
            //    //if (coin.Coin.ToLower() == item.id)
            //    //GetCoinBasePriceHistory(coin.Coin);
            //    try
            //    {
            //        coin.ticker = GetCoinBaseProductTicker(coin.Coin.Trim());
            //        Helpers.Reflection<Ticker>.GetAllProperties(coin.ticker);
            //    }
            //    catch (Exception)
            //    {
            //        Console.WriteLine("Failed to get current ticker for " + coin.Coin);
            //    }                
            //    Console.WriteLine(Environment.NewLine);
            //}
            //if (AllowableCoins != null && AllowableCoins.Count > 0)
            //    Helpers.Reflection<Coins>.GetAllPropertiesArray(AllowableCoins);
            //else
            //    Console.WriteLine("Allowable coint failed to load");
            //return AllowableCoins;
        }

        public Ticker GetTicker(string product)
        {
            var ticker = GetCoinBaseProductTicker(product);
            if (ticker is Ticker && ticker != null)
            {
                return ticker;
            }
            else
            {
                Console.WriteLine("ticker failed to load for " + product);
            }
            return null;
        }
        public Order Trade(string product, decimal amount, TradeDirection direction)
        {
            //return new Order();
            var order = TradeCoinbase(product, amount.ToString(), direction);
            if (order is Order)
                return order;
            else
                return null;
        }
    }
}
