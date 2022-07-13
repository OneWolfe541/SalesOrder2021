using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class CustomOrderDetails
    {
        public string CustomOrderID { get; set; }
        public Nullable<int> ElectionOrderNo { get; set; }
        public Nullable<int> AESContactID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<int> CustomSupplyID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> DueDate { get; set; }
        public string COInstructions { get; set; }

        // Election Order
        public Nullable<int> SalesOrderNo { get; set; }
        public string ElectionTitle { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ElectionDate { get; set; }
        public Nullable<bool> EngProc { get; set; }
        public Nullable<bool> SpnProc { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EngProcReceived { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> SpnProcReceived { get; set; }
        public Nullable<bool> Photo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> PhotoReceived { get; set; }
        public Nullable<int> Questions { get; set; }
        public Nullable<int> Districts { get; set; }

        // Custom Supplies
        public string CustomSupply { get; set; }

        // Customers
        public string CustomerName { get; set; }
        public string BillAddress1 { get; set; }
        public string BillAddress2 { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillZip { get; set; }
        public string MainPhone { get; set; }
        public string MainExtension { get; set; }
        public string MainFax { get; set; }
        public string MainEmail { get; set; }

        // Election Types
        public string GovType { get; set; }

        // Voting Equipment
        public string PollingPlace { get; set; }
        public string Absentee { get; set; }
        public string EarlyVoting { get; set; }

        public List<CustomItemDetails> CustomItems { get; set; }
    }
}