using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class SupplyController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        // GET: Supply
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

                var query = from supply in _salesDB.ElectionSupplyOrders
                            join electionOrder in _salesDB.ElectionOrders
                                    on supply.ElectionOrderNo equals electionOrder.ElectionOrderNo into electiongroup
                            from election in electiongroup.DefaultIfEmpty()
                            join supplyCustomers in _salesDB.Customers
                                    on election.CustomerNo equals supplyCustomers.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join customerContacts in _salesDB.CustomerContacts
                                    on election.ContactID equals customerContacts.ContactID into contactgroup
                            from contact in contactgroup.DefaultIfEmpty()
                            join electionType in _salesDB.ElectionTypes
                                    on election.ElectionType equals electionType.GovTypeID into eltypegroup
                            from el_type in eltypegroup.DefaultIfEmpty()
                            join orderType in _salesDB.OrderTypes
                                    on supply.OrderTypeID equals orderType.OrderTypeID into otypegroup
                            from o_type in otypegroup.DefaultIfEmpty()
                            select new
                            {
                                supply.ElecSupplyOrderNo,
                                supply.ElectionOrderNo,
                                supply.AESContactID,
                                supply.SupplyOrderID,
                                supply.EntryDate,
                                supply.OrderTypeID,
                                supply.ESOrderName,
                                supply.ESInstructions,
                                election.SalesOrderNo,
                                election.ElectionType,
                                election.ElectionTitle,
                                election.ElectionDate,
                                election.Districts,
                                election.LastUpdate,
                                customer.CustomerName,
                                contact.ContactName,
                                contact.ContactEmail,
                                contact.ContactPhone,
                                el_type.GovType,
                                o_type.OrderType
                            };

                int id = eo ?? 0;
                var results = query
                              .Where(d => d.ElectionOrderNo == id)
                              .Select(d => new SupplyOrderDetails
                              {
                                  ElecSupplyOrderNo = d.ElecSupplyOrderNo,
                                  SupplyOrderID = d.SupplyOrderID,
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  AESContactID = d.AESContactID,
                                  EntryDate = d.EntryDate,
                                  OrderTypeID = d.OrderTypeID,
                                  ESOrderName = d.ESOrderName,
                                  ESInstructions = d.ESInstructions,
                                  SalesOrderNo = d.SalesOrderNo,
                                  ElectionType = d.ElectionType,
                                  ElectionTitle = d.ElectionTitle,
                                  ElectionDate = d.ElectionDate,
                                  Districts = d.Districts,
                                  LastUpdate = d.LastUpdate,
                                  CustomerName = d.CustomerName,
                                  ContactName = d.ContactName,
                                  ContactEmail = d.ContactEmail,
                                  ContactPhone = d.ContactPhone,
                                  GovType = d.GovType,
                                  OrderType = d.OrderType
                              })
                              .ToList();

                // Add Supply Order Line Items to results
                if (results != null)
                {
                    foreach (var supply in results)
                    {
                        var itemsQuery = from transactions in _salesDB.ElectionSupplyTransactions
                                         join supplies in _salesDB.ElectionSupplies 
                                                on transactions.ProductID equals supplies.ProductID
                                         select new
                                         {
                                             transactions.TransactionID,
                                             transactions.SupplyOrderID,
                                             transactions.UnitsSold,
                                             supplies.ProductName,
                                             supplies.UnitPrice,
                                             supplies.ListPrice,
                                         };

                        var supplyItems = itemsQuery
                                        .Where(i => i.SupplyOrderID == supply.SupplyOrderID)
                                        .Select(s => new SupplyItemDetails
                                        {
                                            TransactionID = s.TransactionID,
                                            SupplyOrderID = s.SupplyOrderID,
                                            UnitsSold = s.UnitsSold,
                                            ProductName = s.ProductName,
                                            Cost = s.UnitsSold * s.UnitPrice,
                                            Price = s.UnitsSold * s.ListPrice
                                        })
                                        .ToList();

                        if (supplyItems != null)
                        {
                            supply.SupplyItems = supplyItems;
                            supply.Total = supplyItems.Sum(s => s.Price);

                            supplyItems = null;
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
            var existingSupplyOrders = _salesDB.ElectionSupplyOrders.Where(s => s.ElectionOrderNo == eo).ToList();
            var newSupplyOrderNo = existingSupplyOrders.Count() + 1;

            int supplyID = 0;
            if (int.TryParse(eo.ToString() + newSupplyOrderNo, out supplyID))
            {
                tblESOrder supplyOrder = new tblESOrder()
                {
                    ElectionOrderNo = eo,
                    SupplyOrderID = supplyID,
                    ElecSupplyOrderNo = eo.ToString() + "-" + newSupplyOrderNo,
                    EntryDate = DateTime.Now,
                    AESContactID = Int32.Parse(Session["UserId"].ToString()),
                    ESOLocked = false
                };

                _salesDB.ElectionSupplyOrders.Add(supplyOrder);

                try
                {
                    _salesDB.SaveChanges();
                }
                catch (Exception e)
                {
                    var test = e;
                }
                return RedirectToAction("Edit", "Supply", new { so = supplyOrder.ElecSupplyOrderNo });
            }
            else
            {
                return RedirectToAction("Order", "Supply", new { eo = eo });
            }
        }

        public ActionResult Edit(string so)
        {
            if (Session["UserId"] != null)
            {
                var results = _salesDB.ElectionSupplyOrders
                             .Where(d => d.ElecSupplyOrderNo == so)
                             .Select(d => new SupplyOrderDetails
                             {
                                 ElecSupplyOrderNo = d.ElecSupplyOrderNo,
                                 SupplyOrderID = d.SupplyOrderID,
                                 ElectionOrderNo = d.ElectionOrderNo,
                                 AESContactID = d.AESContactID,
                                 EntryDate = d.EntryDate,
                                 OrderTypeID = d.OrderTypeID,
                                 ESOrderName = d.ESOrderName,
                                 ESInstructions = d.ESInstructions
                             })
                             .FirstOrDefault();

                if (results != null)
                {
                    results.SupplyItems = _salesDB.ElectionSupplyTransactions
                                           .Where(d => d.SupplyOrderID == results.SupplyOrderID)
                                           .Select(d => new SupplyItemDetails
                                           {
                                               TransactionID = d.TransactionID,
                                               SupplyOrderID = d.SupplyOrderID,
                                               UnitsSold = d.UnitsSold,
                                               ProductID = d.ProductID,
                                               TransactionDate = d.TransactionDate,
                                               TransactionDescription = d.TransactionDescription,
                                               ItemStatusID = d.ItemStatusID
                                           })
                                           .ToList();
                }

                ViewBag.EmployeeList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName");
                ViewBag.OrderTypesList = new SelectList(_salesDB.OrderTypes.ToList(), "OrderTypeID", "OrderType");
                ViewBag.ProductsList = new SelectList(_salesDB.ElectionSupplies.ToList(), "ProductID", "ProductName");
                ViewBag.ItemStatusList = new SelectList(_salesDB.ItemStatus.ToList(), "ItemStatusID", "ItemStatus");

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public bool UpdateSupplyOrder(SupplyOrderDetails OrderToUpdate)
        {
            if (OrderToUpdate != null)
            {
                var supplyOrder = _salesDB.ElectionSupplyOrders.Where(s => s.ElecSupplyOrderNo == OrderToUpdate.ElecSupplyOrderNo).FirstOrDefault();

                if (supplyOrder != null)
                {
                    // Update all values
                    supplyOrder.EntryDate = OrderToUpdate.EntryDate;
                    supplyOrder.AESContactID = OrderToUpdate.AESContactID;
                    supplyOrder.OrderTypeID = OrderToUpdate.OrderTypeID;
                    supplyOrder.ESOrderName = OrderToUpdate.ESOrderName;
                    supplyOrder.ESInstructions = OrderToUpdate.ESInstructions;

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

        public bool UpdateSupplyItem(SupplyItemDetails ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var supplyItem = _salesDB.ElectionSupplyTransactions.Where(s => s.TransactionID == ItemToUpdate.TransactionID).FirstOrDefault();

                if (supplyItem != null)
                {
                    supplyItem.UnitsSold = ItemToUpdate.UnitsSold;
                    supplyItem.ProductID = ItemToUpdate.ProductID;
                    supplyItem.TransactionDate = ItemToUpdate.TransactionDate;
                    supplyItem.TransactionDescription = ItemToUpdate.TransactionDescription;
                    supplyItem.ItemStatusID = ItemToUpdate.ItemStatusID;

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

        public ActionResult AddSupplyItem(int? so, string eso)
        {
            if (so != null)
            {
                var maxSupplyIndex = _salesDB.ElectionSupplyTransactions.Max(i => i.TransactionID);

                tblESTransactions transation = new tblESTransactions()
                {
                    TransactionID = maxSupplyIndex + 1,
                    TransactionDate = DateTime.Now,
                    SupplyOrderID = so
                };

                _salesDB.ElectionSupplyTransactions.Add(transation);

                try
                {
                    _salesDB.SaveChanges();
                }
                catch (Exception e)
                {
                    var test = e;
                }
            }

            return RedirectToAction("Edit", "Supply", new { so = eso });
        }
    }
}