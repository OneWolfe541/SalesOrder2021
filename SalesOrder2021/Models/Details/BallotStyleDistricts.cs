using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class BallotStyleDistricts
    {
        public Nullable<int> BallotStyleID { get; set; }
        public string BallotOrderNo { get; set; }
        public Nullable<int> StyleIndex { get; set; }
        public string StyleName1 { get; set; }
        public string StyleName2 { get; set; }
        public bool AddColors { get; set; }
        public string StyleColor { get; set; }
        public bool TwoSides { get; set; }
        public Nullable<int> AddBallotStyleID { get; set; }

        public List<BallotStyleDetails> StyleDetails { get; set; }
    }
}