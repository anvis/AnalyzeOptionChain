using System;
using System.Collections.Generic;
using System.Text;

namespace Analyze_Options_Chain.Models
{
    class PE
    {
        public string AskPrice { get; set; }
        public string AskQty { get; set; }
        public string BidPrice { get; set; }
        public string BidQty { get; set; }
        public string Change { get; set; }
        public string ChangeinOpenInterest { get; set; }
        public string ExpiryDate { get; set; }
        public string Identifier { get; set; }
        public string ImpliedVolatility { get; set; }
        public string LastPrice { get; set; }
        public string OpenInterest { get; set; }
        public string PChange { get; set; }
        public string PChangeinOpenInterest { get; set; }
        public string StrikePrice { get; set; }
        public string TotalBuyQuantity { get; set; }
        public string TotalSellQuantity { get; set; }
        public string TotalTradedVolume { get; set; }
        public string Underlying { get; set; }
        public string UnderlyingValue { get; set; }
    }
}
