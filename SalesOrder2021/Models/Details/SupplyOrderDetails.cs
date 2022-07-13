using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class SupplyOrderDetails
    {
        public string ElecSupplyOrderNo { get; set; }
        public Nullable<int> SupplyOrderID { get; set; }
        public Nullable<int> ElectionOrderNo { get; set; }
        public Nullable<int> AESContactID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<int> OrderTypeID { get; set; }
        public string ESOrderName { get; set; }
        public string ESInstructions { get; set; }
        
        // Election Order
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> ElectionType { get; set; }
        public string ElectionTitle { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ElectionDate { get; set; }
        public Nullable<int> Districts { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }

        // Customers
        public string CustomerName { get; set; }

        // Customer Contacts
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        // Election Types
        public string GovType { get; set; }

        // Order Types
        public string OrderType { get; set; }

        // Supply Item Details
        public List<SupplyItemDetails> SupplyItems { get; set; }

        [DataType(DataType.Currency)]
        public Nullable<decimal> Total { get; set; }
    }
}