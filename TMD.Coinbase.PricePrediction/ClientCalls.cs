using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TMDInvestment;
using TMDInvestment.APIProxy;
using TMDInvestment.Models;
using TMDInvestments.Models;

namespace TMD.Coinbase.PricePrediction
{
    public class ClientCalls : ClientBase
    {
        public ClientCalls()
        {

        }
        protected dynamic GetSettings()
        {
            string url = _dbURL + "GetAccountSettings/1";
            //HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var results = APIProxy<AccountInfo>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }
        #region CoinBase
        protected dynamic GetHistoricData(string product, DateTime startDate, DateTime endTime)
        {
            string url = _url + "GetHistoricData/" + product + "/" + startDate.ToShortDateString().Replace("/","-") + "/" + endTime.ToShortDateString().Replace("/", "-");
            var results = APIProxy<List<HistoricData>>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }

        protected dynamic PlaceCoinBaseOrder(dynamic order)
        {
            string url = _url + "PlaceOrder";
            HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var results = APIProxy<dynamic>.Post(url, content, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }

        protected dynamic GetCoinBaseAccounts()
        {
            string url = _url + "GetAccounts";
            //HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var results = APIProxy<List<Accounts>>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }

        protected dynamic GetCoinBaseProducts()
        {
            string url = _url + "GetProducts";
            //HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var results = APIProxy<List<Product>>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }

        protected dynamic GetCoinBaseProductTicker(string productId)
        {
            string url = _url + "GetProduct/" + productId + "/ticker";
            //HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var results = APIProxy<Ticker>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }

        protected dynamic GetProductTicker(string productId)
        {
            string url = _url + "GetProductTicker/" + productId;
            //HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var results = APIProxy<dynamic>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }

        //protected List<Order> GetCoinBaseOrders()
        //{
        //    string url = _url + "GetOrders";
        //    var results = APIProxy<List<Order>>.Get(url, ref error);
        //    if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
        //    {
        //        return error;
        //    }
        //    return results;
        //}

        protected dynamic GetCoinBaseOrderandFill(string productId)
        {
            string url = _url + "GetOrderandFill/" + productId;
            var results = APIProxy<List<Order>>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }

        protected dynamic GetCoinBaseFills(string orderId)
        {
            string url = _url + "GetFills/" + orderId;
            //HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var results = APIProxy<List<Fills>>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }
        #endregion

        #region DBService
        //protected AccountInfo GetSettings(string userId)
        //{
        //    string url = _dbURL + "GetAccountSettings/" + userId;
        //    //HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
        //    var results = APIProxy<AccountInfo>.Get(url, ref error);
        //    if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
        //    {
        //        return error;
        //    }
        //    return results;
        //}

        protected dynamic GetAllowableCoins()
        {
            string url = _dbURL + "GetCoins";
            //HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var results = APIProxy<List<Coins>>.Get(url, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }
        //{
        //    "type": "market",
        //    "funds": 0.50,
        //    "side": "buy",
        //    "product_id": "BTC-USD"
        //}
        protected dynamic TradeCoinbase(string product, string amount, TradeDirection direction)
        {
            string url = _url + "PlaceOrder";
            dynamic model = new ExpandoObject();
            model.type = "market";
            model.funds = amount;
            model.side = direction.ToString();
            model.product_id = product;
            HttpContent content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var results = APIProxy<Order>.Post(url, content, ref error);
            if (Helpers.Errors.HasErrors(error) || Helpers.Errors.NotFound(error))
            {
                return error;
            }
            return results;
        }
        #endregion
    }
}
