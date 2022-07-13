using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class ShippingController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        // GET: Shipping
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Order(int? so)
        {
            if (Session["UserId"] != null)
            {
                ViewBag.SalesOrder = so;

                var query = from shippingOrders in _salesDB.BillOfLading
                            join shippingCustomer in _salesDB.Customers
                                    on shippingOrders.CustomerNo equals shippingCustomer.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join shippingContact in _salesDB.CustomerContacts
                                    on shippingOrders.ContactID equals shippingContact.ContactID into customercontactgroup
                            from custContact in customercontactgroup.DefaultIfEmpty()
                            join shippingAddress in _salesDB.CustomerAddresses
                                    on shippingOrders.ShipAddressID equals shippingAddress.ShipAddressID into shippingaddressgroup
                            from shipAddress in shippingaddressgroup.DefaultIfEmpty()
                            join shippingShipper in _salesDB.Shippers
                                    on shippingOrders.ShipperID equals shippingShipper.ShipperID into shippergroup
                            from shipper in shippergroup.DefaultIfEmpty()
                            select new
                            {
                                shippingOrders.LadingID,
                                shippingOrders.SalesOrderNo,
                                shippingOrders.ElectionOrderNo,
                                shippingOrders.EntryDate,
                                shippingOrders.UpsTracking,
                                shippingOrders.LadingTitle1,
                                shippingOrders.LadingTitle2,
                                shippingOrders.ShipmentDate,
                                shippingOrders.CustomerNo,
                                shippingOrders.ShipAddressID,
                                shippingOrders.ContactID,
                                shippingOrders.ShipperID,
                                shippingOrders.ShippingMethod,
                                shippingOrders.Instructions,
                                shippingOrders.TotalCartons,
                                shippingOrders.CompleteShipment,
                                shippingOrders.PartialShipment,
                                shippingOrders.Freight,
                                shippingOrders.NumberedForm,
                                shippingOrders.NoFrom,
                                shippingOrders.NoTo,
                                shippingOrders.Destroyed,
                                shipAddress.ShipAddress1,
                                shipAddress.ShipAddress2,
                                shipAddress.ShipCity,
                                shipAddress.ShipState,
                                shipAddress.ShipZip,
                                customer.CustomerName,
                                custContact.ContactName,
                                custContact.ContactTitle,
                                custContact.ContactPhone,
                                shipper.ShipperName
                            };

                int id = so ?? 0;
                var results = query
                              .Where(d => d.SalesOrderNo == id)
                              .Select(d => new ShippingOrderDetails
                              {
                                  LadingID = d.LadingID,
                                  SalesOrderNo = d.SalesOrderNo,
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  EntryDate = d.EntryDate,
                                  UpsTracking = d.UpsTracking,
                                  LadingTitle1 = d.LadingTitle1,
                                  LadingTitle2 = d.LadingTitle2,
                                  ShipmentDate = d.ShipmentDate,
                                  CustomerNo = d.CustomerNo,
                                  ShipAddressID = d.ShipAddressID,
                                  ContactID = d.ContactID,
                                  ShipperID = d.ShipperID,
                                  ShippingMethod = d.ShippingMethod,
                                  Instructions = d.Instructions,
                                  TotalCartons = d.TotalCartons,
                                  CompleteShipment = d.CompleteShipment??false,
                                  PartialShipment = d.PartialShipment??false,
                                  Freight = d.Freight,
                                  NumberedForm = d.NumberedForm??false,
                                  NoFrom = d.NoFrom,
                                  NoTo = d.NoTo,
                                  Destroyed = d.Destroyed,
                                  ShipToName = d.CustomerName,
                                  ShipToAddress1 = d.ShipAddress1,
                                  ShipToAddress2 = d.ShipAddress2,
                                  ShipToCity = d.ShipCity,
                                  ShipToState = d.ShipState,
                                  ShipToZip = d.ShipZip,
                                  ShipToContactName = d.ContactName,
                                  ShipToContactTitle = d.ContactTitle,
                                  ShipToPhone = d.ContactPhone,
                                  ShipperName = d.ShipperName
                              })
                              .ToList();

                //https://www.c-sharpcorner.com/article/how-to-generate-barcode-and-read-the-barcode-in-mvc/
                //ViewBag.BarcodeImage

                // Add Shipping Order Line Items to results
                if (results != null)
                {
                    foreach (var ship in results)
                    {
                        var query2 = from details in _salesDB.BillOfLadingDetails
                                     join orderStatus in _salesDB.OrderStatus
                                        on details.StatusID equals orderStatus.StatusID into statusgroup
                                     from status in statusgroup.DefaultIfEmpty()
                                     select new
                                     {
                                         details.LadingDetailID,
                                         details.LadingID,
                                         details.Description,
                                         details.QtyOrdered,
                                         details.QtyShipped,
                                         details.StartNumber,
                                         details.EndNumber,
                                         details.StatusID,
                                         details.BallotDetailID,
                                         status.Status
                                     };

                        var shippingItems = query2
                                        .Where(i => i.LadingID == ship.LadingID)
                                        .Select(s => new ShippingItemDetails
                                        {
                                            LadingDetailID = s.LadingDetailID,
                                            LadingID = s.LadingID,
                                            Description = s.Description,
                                            QtyOrdered = s.QtyOrdered,
                                            QtyShipped = s.QtyShipped,
                                            StartNumber = s.StartNumber,
                                            EndNumber = s.EndNumber,
                                            StatusID = s.StatusID,
                                            Status = s.Status,
                                            BallotDetailID = s.BallotDetailID
                                        })
                                        .ToList();

                        if (shippingItems != null)
                        {
                            ship.ShippingOrderItems = shippingItems;

                            shippingItems = null;
                        }
                    }
                }
                else
                {
                    ViewBag.SalesOrder = so;
                }

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Daily()
        {
            if (Session["UserId"] != null)
            {
                var query = from shippingOrders in _salesDB.BillOfLading
                            join shippingCustomer in _salesDB.Customers
                                    on shippingOrders.CustomerNo equals shippingCustomer.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join shippingContact in _salesDB.CustomerContacts
                                    on shippingOrders.ContactID equals shippingContact.ContactID into customercontactgroup
                            from custContact in customercontactgroup.DefaultIfEmpty()
                            join shippingAddress in _salesDB.CustomerAddresses
                                    on shippingOrders.ShipAddressID equals shippingAddress.ShipAddressID into shippingaddressgroup
                            from shipAddress in shippingaddressgroup.DefaultIfEmpty()
                            join shippingShipper in _salesDB.Shippers
                                    on shippingOrders.ShipperID equals shippingShipper.ShipperID into shippergroup
                            from shipper in shippergroup.DefaultIfEmpty()
                            select new
                            {
                                shippingOrders.LadingID,
                                shippingOrders.SalesOrderNo,
                                shippingOrders.ElectionOrderNo,
                                shippingOrders.EntryDate,
                                shippingOrders.UpsTracking,
                                shippingOrders.LadingTitle1,
                                shippingOrders.LadingTitle2,
                                shippingOrders.ShipmentDate,
                                shippingOrders.CustomerNo,
                                shippingOrders.ShipAddressID,
                                shippingOrders.ContactID,
                                shippingOrders.ShipperID,
                                shippingOrders.ShippingMethod,
                                shippingOrders.Instructions,
                                shippingOrders.TotalCartons,
                                shippingOrders.CompleteShipment,
                                shippingOrders.PartialShipment,
                                shippingOrders.Freight,
                                shippingOrders.NumberedForm,
                                shippingOrders.NoFrom,
                                shippingOrders.NoTo,
                                shippingOrders.Destroyed,
                                shipAddress.ShipAddress1,
                                shipAddress.ShipAddress2,
                                shipAddress.ShipCity,
                                shipAddress.ShipState,
                                shipAddress.ShipZip,
                                customer.CustomerName,
                                custContact.ContactName,
                                custContact.ContactTitle,
                                custContact.ContactPhone,
                                shipper.ShipperName
                            };

                //var currentDate = DateTime.Now.AddDays(-2);
                var currentDate = DateTime.Now;
                var results = query
                              .Where(d => DbFunctions.TruncateTime(d.EntryDate.Value) == DbFunctions.TruncateTime(currentDate))
                              .Select(d => new ShippingOrderDetails
                              {
                                  LadingID = d.LadingID,
                                  SalesOrderNo = d.SalesOrderNo,
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  EntryDate = d.EntryDate,
                                  UpsTracking = d.UpsTracking,
                                  LadingTitle1 = d.LadingTitle1,
                                  LadingTitle2 = d.LadingTitle2,
                                  ShipmentDate = d.ShipmentDate,
                                  CustomerNo = d.CustomerNo,
                                  ShipAddressID = d.ShipAddressID,
                                  ContactID = d.ContactID,
                                  ShipperID = d.ShipperID,
                                  ShippingMethod = d.ShippingMethod,
                                  Instructions = d.Instructions,
                                  TotalCartons = d.TotalCartons,
                                  CompleteShipment = d.CompleteShipment??false,
                                  PartialShipment = d.PartialShipment??false,
                                  Freight = d.Freight,
                                  NumberedForm = d.NumberedForm??false,
                                  NoFrom = d.NoFrom,
                                  NoTo = d.NoTo,
                                  Destroyed = d.Destroyed,
                                  ShipToName = d.CustomerName,
                                  ShipToAddress1 = d.ShipAddress1,
                                  ShipToAddress2 = d.ShipAddress2,
                                  ShipToCity = d.ShipCity,
                                  ShipToState = d.ShipState,
                                  ShipToZip = d.ShipZip,
                                  ShipToContactName = d.ContactName,
                                  ShipToContactTitle = d.ContactTitle,
                                  ShipToPhone = d.ContactPhone,
                                  ShipperName = d.ShipperName
                              })
                              .ToList();

                //https://www.c-sharpcorner.com/article/how-to-generate-barcode-and-read-the-barcode-in-mvc/
                //ViewBag.BarcodeImage

                // Add Shipping Order Line Items to results
                if (results != null)
                {
                    foreach (var ship in results)
                    {
                        var query2 = from details in _salesDB.BillOfLadingDetails
                                     join orderStatus in _salesDB.OrderStatus
                                        on details.StatusID equals orderStatus.StatusID into statusgroup
                                     from status in statusgroup.DefaultIfEmpty()
                                     select new
                                     {
                                         details.LadingDetailID,
                                         details.LadingID,
                                         details.Description,
                                         details.QtyOrdered,
                                         details.QtyShipped,
                                         details.StartNumber,
                                         details.EndNumber,
                                         details.StatusID,
                                         details.BallotDetailID,
                                         status.Status
                                     };

                        var shippingItems = query2
                                        .Where(i => i.LadingID == ship.LadingID)
                                        .Select(s => new ShippingItemDetails
                                        {
                                            LadingDetailID = s.LadingDetailID,
                                            LadingID = s.LadingID,
                                            Description = s.Description,
                                            QtyOrdered = s.QtyOrdered,
                                            QtyShipped = s.QtyShipped,
                                            StartNumber = s.StartNumber,
                                            EndNumber = s.EndNumber,
                                            StatusID = s.StatusID,
                                            Status = s.Status,
                                            BallotDetailID = s.BallotDetailID
                                        })
                                        .ToList();

                        if (shippingItems != null)
                        {
                            ship.ShippingOrderItems = shippingItems;

                            shippingItems = null;
                        }
                    }
                }

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Create(int? so, int? eo)
        {
            //var maxShippingOrderNo = _salesDB.BillOfLading.Max(i => i.SalesOrderNo);
            var salesOrder = _salesDB.SalesOrders.Where(s => s.SalesOrderNo == so).FirstOrDefault();
            var customerNumber = _salesDB.Customers.Where(c => c.CustomerID == salesOrder.CustomerID).Select(c => c.CustomerNo).FirstOrDefault();
            var customerAddress = _salesDB.CustomerAddresses.Where(c => c.CustomerNo == customerNumber).Select(a => a.ShipAddressID).FirstOrDefault();

            var existingShippingOrders = _salesDB.BillOfLading.Where(p => p.SalesOrderNo == so).ToList();
            var newShippingOrderNo = existingShippingOrders.Count() + 1;

            tblBillofLading shippingOrder = new tblBillofLading()
            {
                SalesOrderNo = so,
                ElectionOrderNo = eo,
                LadingID = so.ToString() + "-" + newShippingOrderNo,
                EntryDate = DateTime.Now,
                AESContactID = Int32.Parse(Session["UserId"].ToString()),
                CustomerNo = customerNumber,
                ShipAddressID = customerAddress,
                CompleteShipment = false,
                PartialShipment = false,
                NumberedForm = false,
                BLLocked = false,
                RecByCust = false,
                Misc = false
            };

            _salesDB.BillOfLading.Add(shippingOrder);

            try
            {
                _salesDB.SaveChanges();
            }
            catch (Exception e)
            {
                var test = e;
            }

            return RedirectToAction("Edit", "Shipping", new { so = so.ToString() + "-" + newShippingOrderNo });
        }

        public ActionResult Edit(string so)
        {
            if (Session["UserId"] != null)
            {
                var query = from shippingOrders in _salesDB.BillOfLading
                            join shippingCustomer in _salesDB.Customers
                                    on shippingOrders.CustomerNo equals shippingCustomer.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join shippingContact in _salesDB.CustomerContacts
                                    on shippingOrders.ContactID equals shippingContact.ContactID into customercontactgroup
                            from custContact in customercontactgroup.DefaultIfEmpty()
                            join shippingAddress in _salesDB.CustomerAddresses
                                    on shippingOrders.ShipAddressID equals shippingAddress.ShipAddressID into shippingaddressgroup
                            from shipAddress in shippingaddressgroup.DefaultIfEmpty()
                            join shippingShipper in _salesDB.Shippers
                                    on shippingOrders.ShipperID equals shippingShipper.ShipperID into shippergroup
                            from shipper in shippergroup.DefaultIfEmpty()
                            select new
                            {
                                shippingOrders.LadingID,
                                shippingOrders.SalesOrderNo,
                                shippingOrders.ElectionOrderNo,
                                shippingOrders.AESContactID,
                                shippingOrders.EntryDate,
                                shippingOrders.UpsTracking,
                                shippingOrders.LadingTitle1,
                                shippingOrders.LadingTitle2,
                                shippingOrders.ShipmentDate,
                                shippingOrders.CustomerNo,
                                shippingOrders.ShipAddressID,
                                shippingOrders.ContactID,
                                shippingOrders.ShipperID,
                                shippingOrders.ShippingMethod,
                                shippingOrders.Instructions,
                                shippingOrders.TotalCartons,
                                shippingOrders.CompleteShipment,
                                shippingOrders.PartialShipment,
                                shippingOrders.Freight,
                                shippingOrders.NumberedForm,
                                shippingOrders.NoFrom,
                                shippingOrders.NoTo,
                                shippingOrders.Destroyed,
                                shippingOrders.BLLocked,
                                shippingOrders.RecByCust,
                                shippingOrders.Misc,
                                shipAddress.ShipAddress1,
                                shipAddress.ShipAddress2,
                                shipAddress.ShipCity,
                                shipAddress.ShipState,
                                shipAddress.ShipZip,
                                customer.CustomerName,
                                custContact.ContactName,
                                custContact.ContactTitle,
                                custContact.ContactPhone,
                                shipper.ShipperName
                            };

                var results = query
                              .Where(d => d.LadingID == so)
                              .Select(d => new ShippingOrderDetails
                              {
                                  LadingID = d.LadingID,
                                  SalesOrderNo = d.SalesOrderNo,
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  AESContactID = d.AESContactID,
                                  EntryDate = d.EntryDate,
                                  UpsTracking = d.UpsTracking,
                                  LadingTitle1 = d.LadingTitle1,
                                  LadingTitle2 = d.LadingTitle2,
                                  ShipmentDate = d.ShipmentDate,
                                  CustomerNo = d.CustomerNo,
                                  ShipAddressID = d.ShipAddressID,
                                  ContactID = d.ContactID,
                                  ShipperID = d.ShipperID,
                                  ShippingMethod = d.ShippingMethod,
                                  Instructions = d.Instructions,
                                  TotalCartons = d.TotalCartons,
                                  CompleteShipment = d.CompleteShipment??false,
                                  PartialShipment = d.PartialShipment??false,
                                  Freight = d.Freight,
                                  NumberedForm = d.NumberedForm??false,
                                  NoFrom = d.NoFrom,
                                  NoTo = d.NoTo,
                                  Destroyed = d.Destroyed,
                                  ShipToName = d.CustomerName,
                                  ShipToAddress1 = d.ShipAddress1,
                                  ShipToAddress2 = d.ShipAddress2,
                                  ShipToCity = d.ShipCity,
                                  ShipToState = d.ShipState,
                                  ShipToZip = d.ShipZip,
                                  ShipToContactName = d.ContactName,
                                  ShipToContactTitle = d.ContactTitle,
                                  ShipToPhone = d.ContactPhone,
                                  ShipperName = d.ShipperName
                              })
                              .FirstOrDefault();

                // Add Shipping Order Line Items to results
                if (results != null)
                {
                    var query2 = from details in _salesDB.BillOfLadingDetails
                                 join orderStatus in _salesDB.OrderStatus
                                    on details.StatusID equals orderStatus.StatusID into statusgroup
                                 from status in statusgroup.DefaultIfEmpty()
                                 select new
                                 {
                                     details.LadingDetailID,
                                     details.LadingID,
                                     details.Description,
                                     details.QtyOrdered,
                                     details.QtyShipped,
                                     details.StartNumber,
                                     details.EndNumber,
                                     details.StatusID,
                                     details.BallotDetailID,
                                     status.Status
                                 };

                    var shippingItems = query2
                                    .Where(i => i.LadingID == results.LadingID)
                                    .Select(s => new ShippingItemDetails
                                    {
                                        LadingDetailID = s.LadingDetailID,
                                        LadingID = s.LadingID,
                                        Description = s.Description,
                                        QtyOrdered = s.QtyOrdered,
                                        QtyShipped = s.QtyShipped,
                                        StartNumber = s.StartNumber,
                                        EndNumber = s.EndNumber,
                                        StatusID = s.StatusID,
                                        Status = s.Status,
                                        BallotDetailID = s.BallotDetailID
                                    })
                                    .ToList();

                    if (shippingItems != null)
                    {
                        results.ShippingOrderItems = shippingItems;

                        shippingItems = null;
                    }
                }

                ViewBag.EmployeeList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName");
                ViewBag.CustomerList = new SelectList(_salesDB.Customers.ToList(), "CustomerNo", "CustomerName");
                ViewBag.CustomerAddressList = new SelectList(_salesDB.CustomerAddresses.Where(c => c.CustomerNo == results.CustomerNo).ToList(), "ShipAddressID", "ShipAddress1");
                ViewBag.CustomerContactList = new SelectList(_salesDB.CustomerContacts.Where(c => c.CustomerNo == results.CustomerNo).ToList(), "ContactID", "ContactName");
                ViewBag.ShipperList = new SelectList(_salesDB.Shippers.ToList(), "ShipperID", "ShipperName", results.ShipperID);
                ViewBag.ShippingMethodList = new SelectList(_salesDB.ShippingMethods.Where(m => m.ShipperID == results.ShipperID).ToList(), "ShippingMethod", "ShippingMethod");
                ViewBag.FreightList = FreightList();
                ViewBag.ShippingItemStatus = new SelectList(_salesDB.OrderStatus.ToList(), "StatusID", "Status");

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        private IEnumerable<SelectListItem> FreightList()
        {
            IEnumerable<SelectListItem> Freight = new List<SelectListItem>()
                {
                new SelectListItem() { Text = "PRE-PAID", Value = "PRE-PAID" },
                new SelectListItem() { Text = "P/P & ADD", Value = "P/P & ADD" },
                new SelectListItem() { Text = "COLLECT", Value = "COLLECT" }
                };

            return Freight;
        }

        public bool UpdateShippingOrder(ShippingOrderDetails OrderToUpdate)
        {
            if (OrderToUpdate != null)
            {
                var shippingOrder = _salesDB.BillOfLading.Where(p => p.LadingID == OrderToUpdate.LadingID).FirstOrDefault();

                if(shippingOrder != null)
                {
                    shippingOrder.EntryDate = OrderToUpdate.EntryDate;
                    shippingOrder.AESContactID = OrderToUpdate.AESContactID;
                    shippingOrder.CustomerNo = OrderToUpdate.CustomerNo;
                    shippingOrder.ShipAddressID = OrderToUpdate.ShipAddressID;
                    shippingOrder.ContactID = OrderToUpdate.ContactID;
                    shippingOrder.ShipperID = OrderToUpdate.ShipperID;
                    shippingOrder.ShippingMethod = OrderToUpdate.ShippingMethod;
                    shippingOrder.UpsTracking = OrderToUpdate.UpsTracking;
                    shippingOrder.Freight = OrderToUpdate.Freight;
                    shippingOrder.NumberedForm = OrderToUpdate.NumberedForm;
                    shippingOrder.NoFrom = OrderToUpdate.NoFrom;
                    shippingOrder.NoTo = OrderToUpdate.NoTo;
                    shippingOrder.Destroyed = OrderToUpdate.Destroyed;
                    shippingOrder.TotalCartons = OrderToUpdate.TotalCartons;
                    shippingOrder.CompleteShipment = OrderToUpdate.CompleteShipment;
                    shippingOrder.PartialShipment = OrderToUpdate.PartialShipment;
                    shippingOrder.Instructions = OrderToUpdate.Instructions;
                    shippingOrder.LadingTitle1 = OrderToUpdate.LadingTitle1;
                    shippingOrder.LadingTitle2 = OrderToUpdate.LadingTitle2;
                    shippingOrder.BLLocked = OrderToUpdate.BLLocked??false;
                    shippingOrder.RecByCust = OrderToUpdate.RecByCust??false;
                    shippingOrder.Misc = OrderToUpdate.Misc??false;

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

        public virtual JsonResult CustomerAddressList(int? id)
        {
            var addressList = _salesDB.CustomerAddresses
                .Where(c => c.CustomerNo == id)
                .Select(x => new SelectListItem { Value = x.ShipAddressID.ToString(), Text = x.ShipAddress1 })
                .ToList();

            // Convert list object to Json object
            //string jsonList = new JavaScriptSerializer().Serialize(DayList);
            return Json(addressList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddressDetails(int? id)
        {
            var address = _salesDB.CustomerAddresses.Where(v => v.ShipAddressID == id).FirstOrDefault();

            return Json(address, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerContact(int? id)
        {
            var vendor = _salesDB.CustomerContacts.Where(v => v.ContactID == id).FirstOrDefault();

            return Json(vendor, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddShippingItem(string so)
        {
            var maxRecordId = _salesDB.BillOfLadingDetails.Max(i => i.LadingDetailID);

            tblBillofLadingDetail orderItem = new tblBillofLadingDetail()
            {
                LadingDetailID = maxRecordId + 1,
                LadingID = so
            };

            _salesDB.BillOfLadingDetails.Add(orderItem);

            try
            {
                _salesDB.SaveChanges();
            }
            catch
            {

            }

            return RedirectToAction("Edit", "Shipping", new { so = so });
        }

        public bool UpdateShippingItem(ShippingItemDetails ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var shippingItem = _salesDB.BillOfLadingDetails.Where(s => s.LadingDetailID == ItemToUpdate.LadingDetailID).FirstOrDefault();

                if (shippingItem != null)
                {
                    shippingItem.Description = ItemToUpdate.Description;
                    shippingItem.QtyOrdered = ItemToUpdate.QtyOrdered;
                    shippingItem.QtyShipped = ItemToUpdate.QtyShipped;
                    shippingItem.StartNumber = ItemToUpdate.StartNumber;
                    shippingItem.EndNumber = ItemToUpdate.EndNumber;
                    shippingItem.StatusID = ItemToUpdate.StatusID;

                    try
                    {
                        _salesDB.SaveChanges();
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}