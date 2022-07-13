using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class BallotStyleDetails
    {
        public Nullable<int> BallotDetailID { get; set; }
        public string BallotOrderNo { get; set; }
        public Nullable<int> BallotIndex { get; set; }
        public Nullable<int> BallotStyleID { get; set; }
        public Nullable<int> VotingTypeID { get; set; }
        public Nullable<int> BallotTypeID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> BallotSize { get; set; }
        public Nullable<int> LeftHeaderCode { get; set; }
        public Nullable<int> RightHeaderCode { get; set; }
        public Nullable<int> StartNo { get; set; }
        public Nullable<int> EndNo { get; set; }
        public bool AddBallot { get; set; }
        public string PDFPage { get; set; }
        public Nullable<int> ItemStatusID { get; set; }

        // Ballot Types
        public string BallotTypes { get; set; }

        // Ballot Styles
        public Nullable<int> StyleIndex { get; set; }
        public string StyleName1 { get; set; }
        public string StyleName2 { get; set; }
        public Nullable<bool> AddColors { get; set; }
        public string StyleColor { get; set; }
        public Nullable<bool> TwoSides { get; set; }

        // Ballot Sizes
        public string ColWidth { get; set; }
        public string SizeDesc { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Length { get; set; }

        // Voting Types
        public string VotingType { get; set; }
    }
}