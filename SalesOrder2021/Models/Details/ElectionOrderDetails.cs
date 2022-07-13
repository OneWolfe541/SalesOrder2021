using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrder2021.Models
{
    public class ElectionOrderDetails
    {
        public int ElectionOrderNo { get; set; }
        public Nullable<int> SalesOrderNo { get; set; }
        public Nullable<int> AESContactID { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<int> CustomerNo { get; set; }
        public Nullable<int> ContactID { get; set; }
        public string CustomerPO { get; set; }
        public Nullable<int> ElectionType { get; set; }
        public string ElectionTitle { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> ElectionDate { get; set; }
        public bool EngProc { get; set; }
        public bool SpnProc { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EngProcReceived { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> SpnProcReceived { get; set; }
        public bool Photo { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> PhotoReceived { get; set; }
        public Nullable<int> Questions { get; set; }
        public Nullable<int> Districts { get; set; }
        public bool Imaging { get; set; }
        public bool NoScheduleRpt { get; set; }
        public bool BOD { get; set; }
        public string BODDescription { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> BODData { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> BODSetup { get; set; }
        public Nullable<int> BODSystems { get; set; }
        public string BODNetwork { get; set; }
        public string BODInstructions { get; set; }
        public Nullable<bool> BOLocked { get; set; }
        //[DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> LastUpdate { get; set; }

        // Customers
        public string CustomerName { get; set; }
        public string BillAddress1 { get; set; }
        public string BillAddress2 { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillZip { get; set; }
        public string MainPhone { get; set; }
        public string MainExtension { get; set; }
        public string MainFax { get; set; }
        public string MainEmail { get; set; }

        // Customer Contacts
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        // Alternate Contacts
        public string AltContactName { get; set; }
        public string AltContactEmail { get; set; }
        public string AltContactPhone { get; set; }

        // Election Types
        public string GovType { get; set; }

        // Voting Equipment
        public string PollingPlace { get; set; }
        public string Absentee { get; set; }
        public string EarlyVoting { get; set; }

        public string SpecialInstructions { get; set; }

        public List<ElectionOrderCalendarDetails> ElectionEventsCalendar { get; set; }

        public string BallotPath { get; set; }
        public List<BallotFile> Ballots { get; set; }

        public List<tblWriteInDetail> WriteInCandidates { get; set; }
        public List<tblVotingEquipment> VotingEquipment { get; set; }
        public List<tblInstructions> Instructions { get; set; }
        public List<tblElectionContacts> Contacts { get; set; }
    }
}