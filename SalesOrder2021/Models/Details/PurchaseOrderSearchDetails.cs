using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class PurchaseOrderSearchDetails
    {
        public string PONumber { get; set; }
        public Nullable<int> SupplyOrderID { get; set; }
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> AESContactID { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string AccountCode { get; set; }
        public string QuoteNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> ReqShipDate { get; set; }
        public Nullable<int> ShipperID { get; set; }
        public string ShippingMethod { get; set; }
        public string OrderStatus { get; set; }
        public string Composition { get; set; }
        public string PrevJobNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> PrevDate { get; set; }
        public Nullable<int> VendorNo { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string ForCustomer { get; set; }
        public Nullable<int> CustAddID { get; set; }
        public Nullable<int> ContactID { get; set; }
        public string SpecInst { get; set; }
        public Nullable<int> POType { get; set; }

        public string VendorName { get; set; }

        public string CustomerName { get; set; }

        public string ItemDescription { get; set; }
    }
}