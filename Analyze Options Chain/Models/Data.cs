using System;
using System.Collections.Generic;
using System.Text;

namespace Analyze_Options_Chain.Models
{
    class Data
    {
        public string ExpiryDate { get; set; }

        public string StrikePrice { get; set; }

        public PE PE { get; set; }

        public Ce CE { get; set; }
    }
}
