using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class CustomController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        // GET: Custom
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Order(int? eo)
        {
            if (Session["UserId"] != null)
            {
                var so = _salesDB.ElectionOrders.Where(e => e.ElectionOrderNo == eo).Select(s => s.SalesOrderNo).FirstOrDefault();
                ViewBag.SalesOrder = so;
                ViewBag.ElectionOrder = eo;

                var query = from customOrder in _salesDB.CustomOrders
                            join electionOrder in _salesDB.ElectionOrders
                                    on customOrder.ElectionOrderNo equals electionOrder.ElectionOrderNo into electiongroup
                            from election in electiongroup.DefaultIfEmpty()
                            join customSupply in _salesDB.CustomSupplies
                                    on customOrder.CustomSupplyID equals customSupply.CustomSupplyID into supplygroup
                            from supply in supplygroup.DefaultIfEmpty()
                            join customCustomers in _salesDB.Customers
                                    on election.CustomerNo equals customCustomers.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join electionType in _salesDB.ElectionTypes
                                    on election.ElectionType equals electionType.GovTypeID into eltypegroup
                            from el_type in eltypegroup.DefaultIfEmpty()
                            join votingEquipment in _salesDB.VotingEquipment
                                    on election.ElectionOrderNo equals votingEquipment.ElectionOrderNo into equipmentgroup
                            from equipment in equipmentgroup.DefaultIfEmpty()
                            select new
                            {
                                customOrder.CustomOrderID,
                                customOrder.ElectionOrderNo,
                                customOrder.AESContactID,
                                customOrder.EntryDate,
                                customOrder.CustomSupplyID,
                                customOrder.DueDate,
                                customOrder.COInstructions,
                                election.SalesOrderNo,
                                election.ElectionTitle,
                                election.ElectionDate,
                                election.EngProc,
                                election.SpnProc,
                                election.EngProcReceived,
                                election.SpnProcReceived,
                                election.Photo,
                                election.PhotoReceived,
                                election.Questions,
                                election.Districts,
                                supply.CustomSupply,
                                customer.CustomerName,
                                customer.BillAddress1,
                                customer.BillAddress2,
                                customer.BillCity,
                                customer.BillState,
                                customer.BillZip,
                                customer.MainPhone,
                                customer.MainExtension,
                                customer.MainFax,
                                customer.MainEmail,
                                el_type.GovType,
                                equipment.PollingPlace,
                                equipment.Absentee,
                                equipment.EarlyVoting
                            };

                int id = eo ?? 0;
                var results = query
                              .Where(d => d.ElectionOrderNo == id)
                              .Select(d => new CustomOrderDetails
                              {
                                  CustomOrderID = d.CustomOrderID,
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  AESContactID = d.AESContactID,
                                  EntryDate = d.EntryDate,
                                  CustomSupplyID = d.CustomSupplyID,
                                  DueDate = d.DueDate,
                                  COInstructions = d.COInstructions,
                                  SalesOrderNo = d.SalesOrderNo,
                                  ElectionTitle = d.ElectionTitle,
                                  ElectionDate = d.ElectionDate,
                                  EngProc = d.EngProc,
                                  SpnProc = d.SpnProc,
                                  EngProcReceived = d.EngProcReceived,
                                  SpnProcReceived = d.SpnProcReceived,
                                  Photo = d.Photo,
                                  PhotoReceived = d.PhotoReceived,
                                  Questions = d.Questions,
                                  Districts = d.Districts,
                                  CustomSupply = d.CustomSupply,
                                  CustomerName = d.CustomerName,
                                  BillAddress1 = d.BillAddress1,
                                  BillAddress2 = d.BillAddress2,
                                  BillCity = d.BillCity,
                                  BillState = d.BillState,
                                  BillZip = d.BillZip,
                                  MainPhone = d.MainPhone,
                                  MainExtension = d.MainExtension,
                                  MainFax = d.MainFax,
                                  MainEmail = d.MainEmail,
                                  GovType = d.GovType,
                                  PollingPlace = d.PollingPlace,
                                  Absentee = d.Absentee,
                                  EarlyVoting = d.EarlyVoting
                              })
                              .ToList();

                // Add Custom Order Line Items to results
                if (results != null)
                {
                    foreach (var custom in results)
                    {
                        var itemsQuery = from customDetails in _salesDB.CustomOrderDetails
                                         join items in _salesDB.CustomItems
                                                on customDetails.CustomItemID equals items.CustomItemID
                                         select new
                                         {
                                             customDetails.CustomItemOrderID,
                                             customDetails.CustomOrderID,
                                             customDetails.CustomItemID,
                                             customDetails.pSize,
                                             customDetails.Sides,
                                             customDetails.Quantity,
                                             customDetails.Colors,
                                             customDetails.Parts,
                                             customDetails.Windows,
                                             customDetails.Permit,
                                             customDetails.Metered,
                                             customDetails.AESIndicia,
                                             customDetails.QBRM,
                                             customDetails.Courtesy,
                                             customDetails.Graphics,
                                             customDetails.TickMarks,
                                             customDetails.DrillMarks,
                                             customDetails.ItemStatusID,
                                             items.CustomItem
                                         };

                        var customItems = itemsQuery
                                        .Where(i => i.CustomOrderID == custom.CustomOrderID)
                                        .Select(s => new CustomItemDetails
                                        {
                                            CustomItemOrderID = s.CustomItemOrderID,
                                            CustomOrderID = s.CustomOrderID,
                                            CustomItemID = s.CustomItemID,
                                            pSize = s.pSize,
                                            Sides = s.Sides,
                                            Quantity = s.Quantity,
                                            Colors = s.Colors,
                                            Parts = s.Parts,
                                            Windows = s.Windows,
                                            Permit = s.Permit??false,
                                            Metered = s.Metered??false,
                                            AESIndicia = s.AESIndicia??false,
                                            QBRM = s.QBRM??false,
                                            Courtesy = s.Courtesy??false,
                                            Graphics = s.Graphics??false,
                                            TickMarks = s.TickMarks??false,
                                            DrillMarks = s.DrillMarks??false,
                                            ItemStatusID = s.ItemStatusID,
                                            CustomItem = s.CustomItem
                                        })
                                        .ToList();

                        if (customItems != null)
                        {
                            custom.CustomItems = customItems;

                            customItems = null;
                        }
                    }
                }
                //else
                //{
                //    var so = _salesDB.ElectionOrders.Where(e => e.ElectionOrderNo == eo).Select(s => s.SalesOrderNo).FirstOrDefault();
                //    ViewBag.SalesOrder = so;
                //    ViewBag.ElectionOrder = eo;
                //}

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Create(int? eo)
        {
            var existingCustomOrders = _salesDB.CustomOrders.Where(c => c.ElectionOrderNo == eo).ToList();
            var newCustomOrderNo = existingCustomOrders.Count() + 1;

            tblCustomOrder customOrder = new tblCustomOrder()
            {
                ElectionOrderNo = eo,
                CustomOrderID = eo.ToString() + "-" + newCustomOrderNo,
                EntryDate = DateTime.Now,
                AESContactID = Int32.Parse(Session["UserId"].ToString()),
                COLocked = false
            };

            _salesDB.CustomOrders.Add(customOrder);

            try
            {
                _salesDB.SaveChanges();
            }
            catch (Exception e)
            {
                var test = e;
            }
            return RedirectToAction("Edit", "Custom", new { co = customOrder.CustomOrderID });
        }

        public ActionResult Edit(string co)
        {
            if (Session["UserId"] != null)
            {
                var results = _salesDB.CustomOrders
                              .Where(d => d.CustomOrderID == co)
                              .Select(d => new CustomOrderDetails
                              {
                                  CustomOrderID = d.CustomOrderID,
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  AESContactID = d.AESContactID,
                                  EntryDate = d.EntryDate,
                                  CustomSupplyID = d.CustomSupplyID,
                                  DueDate = d.DueDate,
                                  COInstructions = d.COInstructions
                              })
                              .FirstOrDefault();

                // Add Custom Order Line Items to results
                if (results != null)
                {
                    var customItems = _salesDB.CustomOrderDetails
                                    .Where(i => i.CustomOrderID == results.CustomOrderID)
                                    .Select(s => new CustomItemDetails
                                    {
                                        CustomItemOrderID = s.CustomItemOrderID,
                                        CustomOrderID = s.CustomOrderID,
                                        CustomItemID = s.CustomItemID,
                                        pSize = s.pSize,
                                        Sides = s.Sides,
                                        Quantity = s.Quantity,
                                        Colors = s.Colors,
                                        Parts = s.Parts,
                                        Windows = s.Windows,
                                        Permit = s.Permit??false,
                                        Metered = s.Metered??false,
                                        AESIndicia = s.AESIndicia??false,
                                        QBRM = s.QBRM??false,
                                        Courtesy = s.Courtesy??false,
                                        Graphics = s.Graphics??false,
                                        TickMarks = s.TickMarks??false,
                                        DrillMarks = s.DrillMarks??false,
                                        ItemStatusID = s.ItemStatusID
                                    })
                                    .ToList();

                    if (customItems != null)
                    {
                        results.CustomItems = customItems;

                        customItems = null;
                    }
                }

                ViewBag.EmployeeList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName");
                ViewBag.CustomSupplyList = new SelectList(_salesDB.CustomSupplies.ToList(), "CustomSupplyID", "CustomSupply");
                ViewBag.CustomItemList = new SelectList(_salesDB.CustomItems.ToList(), "CustomItemID", "CustomItem");
                ViewBag.ItemStatusList = new SelectList(_salesDB.ItemStatus.ToList(), "ItemStatusID", "ItemStatus");

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public bool UpdateCustomOrder(CustomOrderDetails OrderToUpdate)
        {
            if (OrderToUpdate != null)
            {
                var customOrder = _salesDB.CustomOrders.Where(s => s.CustomOrderID == OrderToUpdate.CustomOrderID).FirstOrDefault();

                if (customOrder != null)
                {
                    // Update all values
                    customOrder.EntryDate = OrderToUpdate.EntryDate;
                    customOrder.AESContactID = OrderToUpdate.AESContactID;
                    customOrder.CustomSupplyID = OrderToUpdate.CustomSupplyID;
                    customOrder.DueDate = OrderToUpdate.DueDate;
                    customOrder.COInstructions = OrderToUpdate.COInstructions;

                    try
                    {
                        _salesDB.SaveChanges();
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool UpdateCustomItem(CustomItemDetails ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var customItem = _salesDB.CustomOrderDetails.Where(s => s.CustomItemOrderID == ItemToUpdate.CustomItemOrderID).FirstOrDefault();

                if (customItem != null)
                {
                    customItem.CustomItemID = ItemToUpdate.CustomItemID;
                    customItem.Quantity = ItemToUpdate.Quantity;
                    customItem.pSize = ItemToUpdate.pSize;
                    customItem.Sides = ItemToUpdate.Sides;
                    customItem.Parts = ItemToUpdate.Parts;
                    customItem.Colors = ItemToUpdate.Colors;
                    customItem.Permit = ItemToUpdate.Permit;
                    customItem.AESIndicia = ItemToUpdate.AESIndicia;
                    customItem.Courtesy = ItemToUpdate.Courtesy;
                    customItem.ItemStatusID = ItemToUpdate.ItemStatusID;
                    customItem.TickMarks = ItemToUpdate.TickMarks;
                    customItem.ItemStatusID = ItemToUpdate.ItemStatusID;
                    customItem.Metered = ItemToUpdate.Metered;
                    customItem.QBRM = ItemToUpdate.QBRM;
                    customItem.Graphics = ItemToUpdate.Graphics;
                    customItem.DrillMarks = ItemToUpdate.DrillMarks;

                    try
                    {
                        _salesDB.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        var test = e;
                        return false;
                    }
                }

            }
            return true;
        }

        public ActionResult AddCustomItem(string co)
        {
            if (co != null)
            {
                var maxCustomIndex = _salesDB.CustomOrderDetails.Max(i => i.CustomItemOrderID);

                tblCustomOrderDetail customItem = new tblCustomOrderDetail()
                {
                    CustomItemOrderID = maxCustomIndex + 1,
                    CustomOrderID = co,
                    Permit = false,
                    Metered = false,
                    AESIndicia = false,
                    QBRM = false,
                    Courtesy = false,
                    Graphics = false,
                    TickMarks = false,
                    DrillMarks = false
                };

                _salesDB.CustomOrderDetails.Add(customItem);

                try
                {
                    _salesDB.SaveChanges();
                }
                catch (Exception e)
                {
                    var test = e;
                }
            }

            return RedirectToAction("Edit", "Custom", new { co = co });
        }
    }
}