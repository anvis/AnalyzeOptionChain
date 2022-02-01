using System;
using System.Collections.Generic;
using System.Text;

namespace Analyze_Options_Chain.Models
{
    class Records
    {
        public List<string> ExpiryDates { get; set; }

        public List<Data> Data { get; set; }

        public string underlyingValue { get; set; }

        public string timestamp { get; set; }
    }
}
