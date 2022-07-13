using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class ShippingOrderDetails
    {
        public string LadingID { get; set; }
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> ElectionOrderNo { get; set; }
        public Nullable<int> AESContactID { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string UpsTracking { get; set; }
        public string LadingTitle1 { get; set; }
        public string LadingTitle2 { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ShipmentDate { get; set; }
        public Nullable<int> CustomerNo { get; set; }
        public Nullable<int> ShipAddressID { get; set; }
        public Nullable<int> ContactID { get; set; }
        public Nullable<int> ShipperID { get; set; }
        public string ShippingMethod { get; set; }
        public string Instructions { get; set; }
        public Nullable<int> TotalCartons { get; set; }
        public bool CompleteShipment { get; set; }
        public bool PartialShipment { get; set; }
        public Nullable<int> Freight { get; set; }
        public bool NumberedForm { get; set; }
        public string NoFrom { get; set; }
        public string NoTo { get; set; }
        public string Destroyed { get; set; }
        public Nullable<bool> BLLocked { get; set; }
        public Nullable<bool> RecByCust { get; set; }
        public Nullable<bool> Misc { get; set; }

        public string ShipToName { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToZip { get; set; }
        public string ShipToContactName { get; set; }
        public string ShipToContactTitle { get; set; }
        public string ShipToPhone { get; set; }

        public string ShipperName { get; set; }

        public List<ShippingItemDetails> ShippingOrderItems { get; set; }
    }
}