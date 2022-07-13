using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using SalesOrder2021.Models;

namespace SalesOrder2021.Context
{
    public partial class SalesContext : DbContext
    {
        public SalesContext() : base("name=SalesOrder")
        {
        }

        public virtual DbSet<tblBallotOrder> BallotOrders { get; set; }
        public virtual DbSet<tblBallotDetail> BallotOrderDetails { get; set; }
        public virtual DbSet<tblBallotSizes> BallotSizes { get; set; }
        public virtual DbSet<tblBallotStyles> BallotStyles { get; set; }
        public virtual DbSet<tblBallotTypes> BallotTypes { get; set; }
        public virtual DbSet<tblBillofLading> BillOfLading { get; set; }
        public virtual DbSet<tblBillofLadingDetail> BillOfLadingDetails { get; set; }
        public virtual DbSet<tblCalendar> Calendar { get; set; }
        public virtual DbSet<tblCustomer> Customers { get; set; }
        public virtual DbSet<tblCustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<tblCustomerContact> CustomerContacts { get; set; }
        public virtual DbSet<tblCustomItems> CustomItems { get; set; }
        public virtual DbSet<tblCustomOrder> CustomOrders { get; set; }
        public virtual DbSet<tblCustomOrderDetail> CustomOrderDetails { get; set; }
        public virtual DbSet<tblCustomSupplies> CustomSupplies { get; set; }
        public virtual DbSet<tblElectionContacts> ElectionContacts { get; set; }
        public virtual DbSet<tblElectionOrder> ElectionOrders { get; set; }
        public virtual DbSet<tblElectionSupplies> ElectionSupplies { get; set; }
        public virtual DbSet<tblElectionType> ElectionTypes { get; set; }
        public virtual DbSet<tblEmployee> Employees { get; set; }
        public virtual DbSet<tblEquipStatus> EquipmentStatus { get; set; }
        public virtual DbSet<tblESOrder> ElectionSupplyOrders { get; set; }
        public virtual DbSet<tblESTransactions> ElectionSupplyTransactions { get; set; }
        public virtual DbSet<tblEvent> Events { get; set; }
        public virtual DbSet<tblInstructions> SpecialInstructions { get; set; }
        public virtual DbSet<tblItemStatus> ItemStatus { get; set; }
        public virtual DbSet<tblLedger> Ledger { get; set; }
        public virtual DbSet<tblPO> PurchaseOrders { get; set; }
        public virtual DbSet<tblPODetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<tblPODetailCont> PurchaseOrderDetailsCont { get; set; }
        public virtual DbSet<tblPOType> PurchaseOrderTypes { get; set; }
        public virtual DbSet<tblPricingUnits> PricingUnits { get; set; }
        public virtual DbSet<tblOrderStatus> OrderStatus { get; set; }
        public virtual DbSet<tblOrderType> OrderTypes { get; set; }
        public virtual DbSet<tblSalesOrder> SalesOrders { get; set; }
        public virtual DbSet<tblSalesOrderItems> SalesOrderItems { get; set; }
        public virtual DbSet<tblSalesOrderStatus> SalesOrderStatus { get; set; }
        public virtual DbSet<tblShipping> Shippers { get; set; }
        public virtual DbSet<tblShippingMethods> ShippingMethods { get; set; }
        public virtual DbSet<tblSupplies> Supplies { get; set; }
        public virtual DbSet<tblSupplyTransactions> SupplyTransactions { get; set; }
        public virtual DbSet<tblTaxIndex> TaxIndex { get; set; }
        public virtual DbSet<tblVendor> Vendors { get; set; }
        public virtual DbSet<tblVotingEquipment> VotingEquipment { get; set; }
        public virtual DbSet<tblVotingMachineList> VotingMachines { get; set; }
        public virtual DbSet<tblVotingType> VotingTypes { get; set; }
        public virtual DbSet<tblWriteInDetail> WriteInDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // https://stackoverflow.com/questions/12130059/how-turn-off-pluralize-table-creation-for-entity-framework-5
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<tblBallotOrder>().HasKey(c => new { c.BallotOrderNo });
            modelBuilder.Entity<tblBallotDetail>().HasKey(c => new { c.BallotDetailID });
            modelBuilder.Entity<tblBallotSizes>().HasKey(c => new { c.OpSizeID });
            modelBuilder.Entity<tblBallotStyles>().HasKey(c => new { c.BallotStyleID });
            modelBuilder.Entity<tblBallotTypes>().HasKey(c => new { c.BallotTypeID });
            modelBuilder.Entity<tblBillofLading>().HasKey(c => new { c.LadingID });
            modelBuilder.Entity<tblBillofLadingDetail>().HasKey(c => new { c.LadingDetailID });
            modelBuilder.Entity<tblCalendar>().HasKey(c => new { c.EventRecordID });
            modelBuilder.Entity<tblCustomer>().HasKey(c => new { c.CustomerID });
            modelBuilder.Entity<tblCustomerAddress>().HasKey(c => new { c.ShipAddressID });
            modelBuilder.Entity<tblCustomerContact>().HasKey(c => new { c.ContactID });
            modelBuilder.Entity<tblCustomItems>().HasKey(c => new { c.CustomItemID });
            modelBuilder.Entity<tblCustomOrder>().HasKey(c => new { c.CustomOrderID });
            modelBuilder.Entity<tblCustomOrderDetail>().HasKey(c => new { c.CustomItemOrderID });
            modelBuilder.Entity<tblCustomSupplies>().HasKey(c => new { c.CustomSupplyID });
            modelBuilder.Entity<tblElectionContacts>().HasKey(c => new { c.ElectionContactID });
            modelBuilder.Entity<tblElectionOrder>().HasKey(c => new { c.ElectionOrderNo });
            modelBuilder.Entity<tblElectionSupplies>().HasKey(c => new { c.ProductID });
            modelBuilder.Entity<tblElectionType>().HasKey(c => new { c.GovTypeID });
            modelBuilder.Entity<tblEmployee>().HasKey(c => new { c.ContactID });
            modelBuilder.Entity<tblEquipStatus>().HasKey(c => new { c.EquipStatusID });
            modelBuilder.Entity<tblESOrder>().HasKey(c => new { c.ElecSupplyOrderNo });
            modelBuilder.Entity<tblESTransactions>().HasKey(c => new { c.TransactionID });
            modelBuilder.Entity<tblEvent>().HasKey(c => new { c.EventID });
            modelBuilder.Entity<tblInstructions>().HasKey(c => new { c.InstructionID });
            modelBuilder.Entity<tblItemStatus>().HasKey(c => new { c.ItemStatusID });
            modelBuilder.Entity<tblLedger>().HasKey(c => new { c.GLCodeID });
            modelBuilder.Entity<tblPO>().HasKey(c => new { c.PONumber });
            modelBuilder.Entity<tblPODetail>().HasKey(c => new { c.PODetailID });
            modelBuilder.Entity<tblPODetailCont>().HasKey(c => new { c.ContPaperDetailID });
            modelBuilder.Entity<tblPOType>().HasKey(c => new { c.POTypeID });
            modelBuilder.Entity<tblPricingUnits>().HasKey(c => new { c.Unit });
            modelBuilder.Entity<tblOrderStatus>().HasKey(c => new { c.StatusID });
            modelBuilder.Entity<tblOrderType>().HasKey(c => new { c.OrderTypeID });
            modelBuilder.Entity<tblSalesOrder>().HasKey(c => new { c.SalesOrderNo });
            modelBuilder.Entity<tblSalesOrderItems>().HasKey(c => new { c.ItemRecordID });
            modelBuilder.Entity<tblSalesOrderStatus>().HasKey(c => new { c.SOStatusID });
            modelBuilder.Entity<tblShipping>().HasKey(c => new { c.ShipperID });
            modelBuilder.Entity<tblShippingMethods>().HasKey(c => new { c.ShipMethodID });
            modelBuilder.Entity<tblSupplies>().HasKey(c => new { c.ProductID });
            modelBuilder.Entity<tblSupplyTransactions>().HasKey(c => new { c.TransactionID });
            modelBuilder.Entity<tblTaxIndex>().HasKey(c => new { c.TaxIndexID });
            modelBuilder.Entity<tblVendor>().HasKey(c => new { c.VendorNo });
            modelBuilder.Entity<tblVotingEquipment>().HasKey(c => new { c.VMEquipID });
            modelBuilder.Entity<tblVotingMachineList>().HasKey(c => new { c.VotingMachineID });
            modelBuilder.Entity<tblVotingType>().HasKey(c => new { c.VotingTypeID });
            modelBuilder.Entity<tblWriteInDetail>().HasKey(c => new { c.WriteinID });
        }
    }
}