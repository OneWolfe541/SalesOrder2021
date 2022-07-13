using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class SupplyItemDetails
    {
        public Nullable<int> TransactionID { get; set; }
        public Nullable<int> SupplyOrderID { get; set; }
        public Nullable<int> UnitsSold { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> TransactionDate { get; set; } 
        public string TransactionDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Cost { get; set; }
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> ItemStatusID { get; set; }
    }
}