using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class ElectionOrderSearchDetails
    {
        public int ElectionOrderNo { get; set; }
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> AESContactID { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<int> CustomerNo { get; set; }
        public Nullable<int> ContactID { get; set; }
        public string CustomerPO { get; set; }
        public Nullable<int> ElectionType { get; set; }
        public string ElectionTitle { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> ElectionDate { get; set; }

        // Customers
        public string CustomerName { get; set; }
    }
}