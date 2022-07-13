namespace SalesOrder2021.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PurchaseContinuousDetails
    {
        public Nullable<int> ContPaperDetailID { get; set; }
        public Nullable<int> PODetailID { get; set; }
        public Nullable<int> StockNo { get; set; }
        public string StockColor { get; set; }
        public string StockGrade { get; set; }
        public string StockWeight { get; set; }
        public string StockSize { get; set; }
        public string Ink1st { get; set; }
        public string Ink2nd { get; set; }
        public string Ink3rd { get; set; }
        public string PolyPlate { get; set; }
        public string FCChg { get; set; }
        public string FCSame { get; set; }
        public string MargLeft { get; set; }
        public string MargRight { get; set; }
        public string SpecPerfs { get; set; }
        public string MargWords { get; set; }
    }
}