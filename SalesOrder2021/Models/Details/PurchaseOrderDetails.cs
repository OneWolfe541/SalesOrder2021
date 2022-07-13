using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class PurchaseOrderDetails
    {
        public string PONumber { get; set; }
        public Nullable<int> SupplyOrderID { get; set; }
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> AESContactID { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string QuoteNo { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> QuoteAmount { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ReqShipDate { get; set; }
        public Nullable<int> ShipperID { get; set; }
        public string ShippingMethod { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
        public string Composition { get; set; }
        public string PrevJobNo { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> PrevDate { get; set; }
        public Nullable<int> VendorNo { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string ForCustomer { get; set; }
        public Nullable<int> CustAddID { get; set; }
        public Nullable<int> ContactID { get; set; }
        public string SpecInst { get; set; }
        public string QtyInst { get; set; }
        public Nullable<int> POType { get; set; }
        public Nullable<int> POStatus { get; set; }

        public string Status { get; set; }

        public string ShipperName { get; set; }

        public string VendorName { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorCity { get; set; }
        public string VendorState { get; set; }
        public string VendorZip { get; set; }
        public string VendorPhone { get; set; }

        public string CustomerName { get; set; }
        public Nullable<int> CustomerAddressID { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerZip { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerContactName { get; set; }

        public List<PurchaseItemDetails> PurchaseOrderItems { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> Total { get; set; }

        public List<SupplyTransactionDetails> SupplyItems { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> SupplyTotal { get; set; }
    }
}