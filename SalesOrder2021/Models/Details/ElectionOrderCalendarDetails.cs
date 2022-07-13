namespace SalesOrder2021.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ElectionOrderCalendarDetails
    {
        public int EventRecordID { get; set; }
        public Nullable<int> ElectionOrderNo { get; set; }
        public Nullable<int> EventID { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EventDate { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EventTime { get; set; }
        public string EventLocation { get; set; }
        public string EventPerson { get; set; }
        public Nullable<int> ContactID { get; set; }
        public Nullable<int> ShipAddressID { get; set; }
        public Nullable<bool> PublishWSS { get; set; }
        public string ElectionEvent { get; set; }
    }
}