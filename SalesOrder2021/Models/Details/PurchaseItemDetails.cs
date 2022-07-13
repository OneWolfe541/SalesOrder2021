namespace SalesOrder2021.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PurchaseItemDetails
    {
        public Nullable<int> PODetailID { get; set; }
        public string PONumber { get; set; }
        public Nullable<int> ItemNo { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string ItemDescription { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> UnitCost { get; set; }
        public Nullable<int> PricePerUnit { get; set; }        
        [DataType(DataType.Currency)]
        public Nullable<decimal> Extension { get; set; }
        public string StockWeight { get; set; }
        public string StockColor { get; set; }
        public string StockGrade { get; set; }
        public string StockSize { get; set; }
        public string Ink1st { get; set; }
        public string Ink2nd { get; set; }
        public string Ink3rd { get; set; }
        public string Ink4th { get; set; }
        public Nullable<bool> Numbering { get; set; }
        public string StartNumber { get; set; }
        public string NumberColor { get; set; }
        public string NumberStyle { get; set; }
        public Nullable<bool> NoMissing { get; set; }
        public string MICR { get; set; }
        public string Construction { get; set; }
        public Nullable<bool> Punching { get; set; }
        public string PunchSize { get; set; }
        public string PunchNo { get; set; }
        public string CtoC { get; set; }
        public string PunchLoc { get; set; }
        public string FromEdge { get; set; }
        public string Press { get; set; }
        public string Bind { get; set; }
        public string Parts { get; set; }
        public string Carbon { get; set; }
        public string StubSize { get; set; }
        public string SnapoutLoc { get; set; }
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
        [DataType(DataType.Currency)]
        public Nullable<decimal> Extensionq { get; set; }
        public Nullable<bool> ItemComplete { get; set; }
        public Nullable<bool> PublishWSS { get; set; }
        public Nullable<bool> Vouchered { get; set; }

        public List<PurchaseContinuousDetails> ContinuousItems { get; set; }
    }
}