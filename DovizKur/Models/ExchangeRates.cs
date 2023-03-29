using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DovizKur.Models
{
    public class ExchangeRates
    {
        public DateTime date { get; set; }
 
        public decimal? usd { get; set; }

        public decimal? eur { get; set; }
  
        public decimal? chf { get; set; }
   
        public decimal? gbp { get; set; }
   
        public decimal? jpy { get; set; }
    }
}
