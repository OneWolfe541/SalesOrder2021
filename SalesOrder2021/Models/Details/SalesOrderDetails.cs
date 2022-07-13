namespace SalesOrder2021.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SalesOrderDetails
    {
        public Nullable<int> SalesOrderNo { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string AccountCode { get; set; }
        public string CustomerPO { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ReqShipDate { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ActShipDate { get; set; }
        public string ContractNo { get; set; }
        public Nullable<int> Createdby { get; set; }
        public string CreatedbyName { get; set; }
        public Nullable<int> BillCustomerNo { get; set; }
        public string CustomerID { get; set; }
        public Nullable<int> CustomerContact { get; set; }
        //[DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> SalesOrderTotal { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> SalesOrderCost { get; set; }
        public string TaxStatus { get; set; }
        //[DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> TaxValue { get; set; }
        public Nullable<int> Billedby { get; set; }
        public Nullable<System.DateTime> BilledDate { get; set; }
        public string InvoiceNo { get; set; }
        public string ProjectNo { get; set; }
        public Nullable<bool> MiscSalesOrder { get; set; }

        public string CustLookupIndex { get; set; }
        public string ContactName { get; set; }        
        public string MainPhone { get; set; }
        public string MainFax { get; set; }

        public string CustomerName { get; set; }
        public string BillAddress1 { get; set; }
        public string BillAddress2 { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillZip { get; set; }

        public int? SOStatusID { get; set; }
        public string SOStatus { get; set; }

        public List<SalesItemDetails> SalesOrderItems { get; set; }

        public List<PurchaseOrderSummary> PurchaseOrders { get; set; }
        [DataType(DataType.Currency)]
        public Nullable<decimal> PurchaseTotal { get; set; }
    }
}