namespace SalesOrder2021.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SalesItemDetails
    {
        public Nullable<int> ItemRecordID { get; set; }
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> ItemNo { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string GenDescription { get; set; }
        public Nullable<int> PricePerUnit { get; set; }
        //[DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> CustPrice { get; set; }
        //[DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Extension { get; set; }
    }
}