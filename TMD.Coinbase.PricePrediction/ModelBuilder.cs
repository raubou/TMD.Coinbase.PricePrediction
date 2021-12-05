using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;
using static TMD.Coinbase.PricePrediction.Models;

namespace TMD.Coinbase.PricePrediction
{
    class ModelBuilder
    {
        // Create MLContext to be shared across the model creation workflow objects 
        // Set a random seed for repeatable/deterministic results across multiple trainings.
        private static MLContext mlContext = new MLContext(seed: 1);

        public void BuildModel()
        {
            // Load Data
            //var data = mlContext.Data.LoadFromEnumerable<CurrencyModel>(results);
        }
    }
}
