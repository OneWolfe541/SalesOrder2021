namespace SalesOrder2021.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ShippingItemDetails
    {
        public int LadingDetailID { get; set; }
        public string LadingID { get; set; }
        public string Description { get; set; }
        public Nullable<int> QtyOrdered { get; set; }
        public Nullable<int> QtyShipped { get; set; }
        public string StartNumber { get; set; }
        public string EndNumber { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string Status { get; set; }
        public Nullable<int> BallotDetailID { get; set; }
    }
}