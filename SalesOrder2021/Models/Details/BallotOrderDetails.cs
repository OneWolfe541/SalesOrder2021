using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class BallotOrderDetails
    {
        public string BallotOrderNo { get; set; }
        public Nullable<int> ElectionOrderNo { get; set; }
        public Nullable<int> AESContactID { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<int> OrderTypeID { get; set; }
        public string BallotOrderName { get; set; }
        public string BOInstructions { get; set; }
        
        // Election Order
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> ElectionType { get; set; }
        public string ElectionTitle { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
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

        // Ballot Style Details
        public List<BallotStyleDetails> BallotStyles { get; set; }
        public List<BallotStyleDistricts> BallotDistricts { get; set; }
    }
}