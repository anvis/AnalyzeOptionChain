using System;
using System.Collections.Generic;
using System.Text;

namespace Analyze_Options_Chain.Models
{
   public class History
    {
        public chart chart { get; set; }
    }

   public class quote
   {
       public string[] high { get; set; }

       public string[] close { get; set; }

        public string[] low { get; set; }

        public string[] open { get; set; }

        public string[] volume { get; set; }
    }

   public class chart
   {
       public result[] Result { get; set; }
   }

   public class result
   {
       public string[] timestamp { get; set; }

       public Indicators Indicators { get; set; }
   }

   public class Indicators
   {
       public quote[] quote { get; set; }
   }
}
