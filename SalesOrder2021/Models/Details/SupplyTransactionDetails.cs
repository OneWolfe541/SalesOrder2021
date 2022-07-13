namespace SalesOrder2021.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SupplyTransactionDetails
    {
        public Nullable<int> TransactionID { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> ItemNo { get; set; }
        public Nullable<int> ProductCategoryID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> SupplyOrderID { get; set; }
        public string TransactionDescription { get; set; }
        public Nullable<int> UnitsOrdered { get; set; }
        public Nullable<int> UnitsReceived { get; set; }
        public Nullable<int> UnitsSold { get; set; }
        public Nullable<int> UnitsShrinkage { get; set; }
        public Nullable<int> UnitsShipped { get; set; }
        public Nullable<bool> AddTax { get; set; }
        public Nullable<float> TaxValue { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> Freight { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> Credit { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> Extensionq { get; set; }
        public Nullable<bool> Received { get; set; }
        public Nullable<bool> PartialReceived { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> DateReceived { get; set; }
        public Nullable<int> QtyReceived { get; set; }
        public Nullable<int> ReceivedBy { get; set; }
        public string CheckedBy { get; set; }
        public string InvoiceNum { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> Extension { get; set; }
        public Nullable<bool> PublishWSS { get; set; }
        public Nullable<bool> ItemComplete { get; set; }
        public Nullable<bool> Vouchered { get; set; }
        public string ProductName { get; set; }
    }
}