using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class ElectionCalendarDetails
    {
        
        public Nullable<int> ElectionOrderNo { get; set; }
        public Nullable<int> SalesOrderNo { get; set; }
        public string ElectionTitle { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> ElectionDate { get; set; }
        public Nullable<int> Questions { get; set; }
        public Nullable<int> Districts { get; set; }
        public Nullable<bool> BOD { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> BODData { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> BODSetup { get; set; }
        public Nullable<int> BODSystems { get; set; }
        public Nullable<bool> Imaging { get; set; }
        public string CustomerName { get; set; }
        public string GovType { get; set; }

        public List<EventCalendarDetails> EventsCalendar { get; set; }
    }
}