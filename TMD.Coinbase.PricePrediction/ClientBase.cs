using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TMD.Coinbase.PricePrediction
{
    public abstract class ClientBase 
    {
        private IConfiguration TMDInvestments;
        private IConfiguration DBService;
        protected string _url = string.Empty;
        protected string _dbURL = string.Empty;
        //string _controller = string.Empty;
        protected dynamic error;
        public ClientBase()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            TMDInvestments = configuration.GetSection("TMDInvestment");
            DBService = configuration.GetSection("DBService");
            //_controller = controllerBase.ToString();
            if (TMDInvestments != null)
            {
                _url = TMDInvestments.GetSection("url").Value;
            }
            if (DBService != null)
            {
                _dbURL = DBService.GetSection("url").Value;
            }
        }
    }
}
