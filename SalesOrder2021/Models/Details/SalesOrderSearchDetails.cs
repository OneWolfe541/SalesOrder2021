namespace SalesOrder2021.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SalesOrderSearchDetails
    {
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> ElectionOrderNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string CustomerID { get; set; }
        public string CustLookupIndex { get; set; }
        public string CustomerName { get; set; }        
        public Nullable<int> ItemRecordID { get; set; }
        public Nullable<int> ItemNo { get; set; }
        public string GenDescription { get; set; }
        public int SOStatusID { get; set; }
        public string SOStatus { get; set; }
    }
}