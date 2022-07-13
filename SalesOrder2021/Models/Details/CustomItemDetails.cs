namespace SalesOrder2021.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CustomItemDetails
    {
        public Nullable<int> CustomItemOrderID { get; set; }
        public string CustomOrderID { get; set; }
        public Nullable<int> CustomItemID { get; set; }
        public string pSize { get; set; }
        public string Sides { get; set; }
        public string Quantity { get; set; }
        public string Colors { get; set; }
        public string Parts { get; set; }
        public string Windows { get; set; }
        public bool Permit { get; set; }
        public bool Metered { get; set; }
        public bool AESIndicia { get; set; }
        public bool QBRM { get; set; }
        public bool Courtesy { get; set; }
        public bool Graphics { get; set; }
        public bool TickMarks { get; set; }
        public bool DrillMarks { get; set; }
        public Nullable<int> ItemStatusID { get; set; }
        public string CustomItem { get; set; }
    }
}