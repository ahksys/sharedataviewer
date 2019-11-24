using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    // Defines the data structure of each row of the share data input.
    public class SharePrice
    {
        public string unitID { get; set; }
        public DateTime date { get; set; }
        public float unitPrice { get; set; }

        public SharePrice( string unitID, DateTime date, float unitPrice ) 
        {
            this.unitID = unitID;
            this.date = date;
            this.unitPrice = unitPrice;
        }

        // Defined mappings for each of the columns in the CSV file.
        public sealed class SharePriceMap : ClassMap<SharePrice>
        {
            public SharePriceMap()
            {
                // unitID,date,unitPrice
                Map(m => m.unitID).Name("unitID");
                Map(m => m.date).Name("date");
                Map(m => m.unitPrice).Name("unitPrice");
            }
        }
    }
}
