using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class PurchaseController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        // GET: Purchase
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string pono, int? vendid, string vendname)
        {
            if (Session["UserId"] != null)
            {
                var query = from purchaseOrders in _salesDB.PurchaseOrders
                            join purchaseVendor in _salesDB.Vendors
                                on purchaseOrders.VendorNo equals purchaseVendor.VendorNo into vendorgroup
                            from vendor in vendorgroup.DefaultIfEmpty()
                            join purchaseCustomer in _salesDB.Customers
                                    on purchaseOrders.CustomerID equals purchaseCustomer.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            select new 
                            {
                                purchaseOrders.PONumber,
                                purchaseOrders.SupplyOrderID,
                                purchaseOrders.SalesOrderNo,
                                purchaseOrders.AESContactID,
                                purchaseOrders.OrderDate,
                                purchaseOrders.VendorNo,
                                purchaseOrders.CustomerID,
                                vendor.VendorName,
                                customer.CustomerName
                            };

                if (pono == "") pono = null;

                var searchResults = query
                                .Where(d => (d.PONumber == pono || pono == null) && (d.VendorNo == vendid || vendid == null))
                                .Select(d => new PurchaseOrderSearchDetails
                                {
                                    PONumber = d.PONumber,
                                    SupplyOrderID = d.SupplyOrderID,
                                    SalesOrderNo = d.SalesOrderNo,
                                    AESContactID = d.AESContactID,
                                    OrderDate = d.OrderDate,
                                    VendorNo = d.VendorNo,
                                    CustomerID = d.CustomerID,
                                    VendorName = d.VendorName,
                                    CustomerName = d.CustomerName
                                })
                                .OrderByDescending(o => o.OrderDate)
                                .Take(1000)
                                .ToList();

                ViewBag.PONumber = pono;
                ViewBag.VendorList = new SelectList(_salesDB.Vendors.ToList(), "VendorNo", "VendorName", vendid);
                ViewBag.VendorName = vendname;

                return View(searchResults);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Order(int? so)
        {
            if (Session["UserId"] != null)
            {
                ViewBag.SalesOrder = so;

                var statusList = POStatusList(null).ToList();

                var query = from purchaseOrders in _salesDB.PurchaseOrders
                            join purchaseVendor in _salesDB.Vendors
                                    on purchaseOrders.VendorNo equals purchaseVendor.VendorNo into vendorgroup
                            from vendor in vendorgroup.DefaultIfEmpty()
                            join purchaseCustomer in _salesDB.Customers
                                    on purchaseOrders.CustomerID equals purchaseCustomer.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join customerAddress in _salesDB.CustomerAddresses
                                    on purchaseOrders.CustAddID equals customerAddress.CustomerNo into customeraddressgroup
                            from custAddress in customeraddressgroup.DefaultIfEmpty()
                            join customerContact in _salesDB.CustomerContacts
                                    on purchaseOrders.ContactID equals customerContact.ContactID into customercontactgroup
                            from custContact in customercontactgroup.DefaultIfEmpty()
                            join purchaseShipper in _salesDB.Shippers
                                    on purchaseOrders.ShipperID equals purchaseShipper.ShipperID into shippergroup
                            from shipper in shippergroup.DefaultIfEmpty()                            
                            join purchaseOrderTypes in _salesDB.PurchaseOrderTypes
                                    on purchaseOrders.POTypeID equals purchaseOrderTypes.POTypeID into typesgroup
                            from types in typesgroup.DefaultIfEmpty()
                            select new
                            {
                                purchaseOrders.PONumber,
                                purchaseOrders.SupplyOrderID,
                                purchaseOrders.SalesOrderNo,
                                purchaseOrders.AESContactID,
                                purchaseOrders.OrderDate,
                                purchaseOrders.OrderStatus,
                                purchaseOrders.AccountCode,
                                purchaseOrders.QuoteNo,
                                purchaseOrders.QuoteAmount,
                                purchaseOrders.ReqShipDate,
                                purchaseOrders.ShipperID,
                                purchaseOrders.ShippingMethod,
                                purchaseOrders.Composition,
                                purchaseOrders.PrevJobNo,
                                purchaseOrders.PrevDate,
                                purchaseOrders.VendorNo,
                                purchaseOrders.CustomerID,
                                purchaseOrders.forCustomer,
                                purchaseOrders.CustAddID,
                                purchaseOrders.ContactID,
                                purchaseOrders.SpecInst,
                                purchaseOrders.POTypeID,
                                purchaseOrders.POStatus,
                                vendor.VendorName,
                                vendor.VendorAddress1,
                                vendor.VendorAddress2,
                                vendor.VendorCity,
                                vendor.VendorState,
                                vendor.VendorZip,
                                vendor.VendorPhone,
                                customer.CustomerName,
                                CustomerContact = custContact.ContactName,
                                CustomerPhone = custContact.ContactPhone,
                                custAddress.ShipAddress1,
                                custAddress.ShipAddress2,
                                custAddress.ShipCity,
                                custAddress.ShipState,
                                custAddress.ShipZip,
                                shipper.ShipperName,
                                types.POType
                            };

                int id = so ?? 0;
                var results = query
                              .Where(d => d.SalesOrderNo == id)
                              .Select(d => new PurchaseOrderDetails
                              {
                                  PONumber = d.PONumber,
                                  SupplyOrderID = d.SupplyOrderID,
                                  SalesOrderNo = d.SalesOrderNo,
                                  AESContactID = d.AESContactID,
                                  OrderDate = d.OrderDate,
                                  AccountCode = d.AccountCode,
                                  QuoteNo = d.QuoteNo,
                                  QuoteAmount = d.QuoteAmount,
                                  ReqShipDate = d.ReqShipDate,
                                  ShipperID = d.ShipperID,
                                  ShippingMethod = d.ShippingMethod,
                                  OrderStatus = d.OrderStatus,
                                  Composition = d.Composition,
                                  PrevJobNo = d.PrevJobNo,
                                  PrevDate = d.PrevDate,
                                  VendorNo = d.VendorNo,
                                  CustomerID = d.CustomerID,
                                  ForCustomer = d.forCustomer,
                                  CustAddID = d.CustAddID,
                                  ContactID = d.ContactID,
                                  SpecInst = d.SpecInst,
                                  POType = d.POTypeID,
                                  VendorName = d.VendorName,
                                  VendorAddress1 = d.VendorAddress1,
                                  VendorAddress2 = d.VendorAddress2,
                                  VendorCity = d.VendorCity,
                                  VendorState = d.VendorState,
                                  VendorZip = d.VendorZip,
                                  VendorPhone = d.VendorPhone,
                                  CustomerName = d.CustomerName,
                                  CustomerAddress1 = d.ShipAddress1,
                                  CustomerAddress2 = d.ShipAddress2,
                                  CustomerCity = d.ShipCity,
                                  CustomerState = d.ShipState,
                                  CustomerZip = d.ShipZip,
                                  CustomerContactName = d.CustomerContact,
                                  CustomerPhone = d.CustomerPhone,
                                  ShipperName = d.ShipperName,
                                  OrderType = d.POType,
                                  POStatus = d.POStatus
                              })
                              .ToList();

                // Add Purchase Order Line Items to results
                if (results != null)
                {                    
                    foreach (var po in results)
                    {
                        po.Status = statusList.Where(s => s.Value == po.POStatus.ToString()).Select(s => s.Text).FirstOrDefault();

                        if (po.POType == 3)
                        {
                            if (po.SupplyOrderID != null)
                            {
                                var supplyQuery = from transaction in _salesDB.SupplyTransactions
                                                  join supplies in _salesDB.Supplies
                                                    on transaction.ProductID equals supplies.ProductID into supplygroup
                                                  from supply in supplygroup.DefaultIfEmpty()
                                                  join employees in _salesDB.Employees
                                                    on transaction.ReceivedBy equals employees.ContactID into employeegroup
                                                  from employee in employeegroup.DefaultIfEmpty()
                                                  select new
                                                  {
                                                      transaction.TransactionID,
                                                      transaction.TransactionDate,
                                                      transaction.ItemNo,
                                                      transaction.ProductCategoryID,
                                                      transaction.ProductID,
                                                      transaction.SupplyOrderID,
                                                      transaction.TransactionDescription,
                                                      transaction.UnitsOrdered,
                                                      transaction.UnitsReceived,
                                                      transaction.UnitsSold,
                                                      transaction.UnitsShrinkage,
                                                      transaction.UnitsShipped,
                                                      transaction.AddTax,
                                                      transaction.TaxValue,
                                                      transaction.Freight,
                                                      transaction.Credit,
                                                      transaction.Extensionq,
                                                      transaction.Received,
                                                      transaction.PartialReceived,
                                                      transaction.DateReceived,
                                                      transaction.QtyReceived,
                                                      transaction.ReceivedBy,
                                                      transaction.InvoiceNum,
                                                      transaction.InvoiceDate,
                                                      transaction.Extension,
                                                      transaction.ItemComplete,
                                                      transaction.Vouchered,
                                                      supply.ProductName,
                                                      employee.ContactName
                                                  };

                                var supplyItems = supplyQuery
                                                .Where(i => i.SupplyOrderID == po.SupplyOrderID)
                                                .Select(s => new SupplyTransactionDetails
                                                {
                                                    TransactionID = s.TransactionID,
                                                    TransactionDate = s.TransactionDate,
                                                    ItemNo = s.ItemNo,
                                                    ProductCategoryID = s.ProductCategoryID,
                                                    ProductID = s.ProductID,
                                                    SupplyOrderID = s.SupplyOrderID,
                                                    TransactionDescription = s.TransactionDescription,
                                                    UnitsOrdered = s.UnitsOrdered,
                                                    UnitsReceived = s.UnitsReceived,
                                                    UnitsSold = s.UnitsSold,
                                                    UnitsShrinkage = s.UnitsShrinkage,
                                                    UnitsShipped = s.UnitsShipped,
                                                    TaxValue = s.TaxValue,
                                                    Freight = s.Freight,
                                                    Credit = s.Credit,
                                                    Extensionq = s.Extensionq,
                                                    Received = s.Received,
                                                    PartialReceived = s.PartialReceived,
                                                    DateReceived = s.DateReceived,
                                                    QtyReceived = s.QtyReceived,
                                                    ReceivedBy = s.ReceivedBy,
                                                    InvoiceNum = s.InvoiceNum,
                                                    InvoiceDate = s.InvoiceDate,
                                                    Extension = s.Extension,
                                                    ItemComplete = s.ItemComplete,
                                                    Vouchered = s.Vouchered,
                                                    ProductName = s.ProductName,
                                                    CheckedBy = s.ContactName
                                                })
                                                .ToList();

                                if (supplyItems != null)
                                {
                                    po.SupplyItems = supplyItems;
                                    po.SupplyTotal = supplyItems.Sum(s => s.Extension);

                                    supplyItems = null;
                                }
                            }
                        }
                        else
                        {
                            var poQuery = from purchase in _salesDB.PurchaseOrderDetails
                                              join employees in _salesDB.Employees
                                                on purchase.ReceivedBy equals employees.ContactID into employeegroup
                                              from employee in employeegroup.DefaultIfEmpty()
                                              select new
                                              {
                                                  purchase.PODetailID,
                                                  purchase.PONumber,
                                                  purchase.ItemNo,
                                                  purchase.QtyOrdered,
                                                  purchase.ItemDescription,
                                                  purchase.PerUnit,
                                                  purchase.UnitCost,
                                                  purchase.Extension,
                                                  purchase.StockWeight,
                                                  purchase.StockColor,
                                                  purchase.StockGrade,
                                                  purchase.StockSize,
                                                  purchase.Ink1st,
                                                  purchase.Ink2nd,
                                                  purchase.Ink3rd,
                                                  purchase.Ink4th,
                                                  purchase.Numbering,
                                                  purchase.StartNumber,
                                                  purchase.NumberColor,
                                                  purchase.NumberStyle,
                                                  purchase.NoMissing,
                                                  purchase.MICR,
                                                  purchase.Construction,
                                                  purchase.Punching,
                                                  purchase.PunchSize,
                                                  purchase.PunchNo,
                                                  purchase.CtoC,
                                                  purchase.PunchLoc,
                                                  purchase.FromEdge,
                                                  purchase.Press,
                                                  purchase.Bind,
                                                  purchase.Parts,
                                                  purchase.Carbon,
                                                  purchase.StubSize,
                                                  purchase.SnapoutLoc,
                                                  purchase.Received,
                                                  purchase.PartialReceived,
                                                  purchase.DateReceived,
                                                  purchase.QtyReceived,
                                                  purchase.ReceivedBy,
                                                  purchase.InvoiceNum,
                                                  purchase.InvoiceDate,
                                                  purchase.Extensionq,
                                                  purchase.ItemComplete,
                                                  purchase.PublishWSS,
                                                  purchase.Vouchered,
                                                  employee.ContactName
                                              };

                            var purchaseItems = poQuery
                                            .Where(i => i.PONumber == po.PONumber)
                                            .Select(s => new PurchaseItemDetails
                                            {
                                                PODetailID = s.PODetailID,
                                                PONumber = s.PONumber,
                                                ItemNo = s.ItemNo,
                                                Quantity = s.QtyOrdered,
                                                ItemDescription = s.ItemDescription,
                                                PricePerUnit = s.PerUnit,
                                                UnitCost = s.UnitCost,
                                                Extension = s.Extension,
                                                StockWeight = s.StockWeight,
                                                StockColor = s.StockColor,
                                                StockGrade = s.StockGrade,
                                                StockSize = s.StockSize,
                                                Ink1st = s.Ink1st,
                                                Ink2nd = s.Ink2nd,
                                                Ink3rd = s.Ink3rd,
                                                Ink4th = s.Ink4th,
                                                Numbering = s.Numbering,
                                                StartNumber = s.StartNumber,
                                                NumberColor = s.NumberColor,
                                                NumberStyle = s.NumberStyle,
                                                NoMissing = s.NoMissing,
                                                MICR = s.MICR,
                                                Construction = s.Construction,
                                                Punching = s.Punching,
                                                PunchSize = s.PunchSize,
                                                PunchNo = s.PunchNo,
                                                CtoC = s.CtoC,
                                                PunchLoc = s.PunchLoc,
                                                FromEdge = s.FromEdge,
                                                Press = s.Press,
                                                Bind = s.Bind,
                                                Parts = s.Parts,
                                                Carbon = s.Carbon,
                                                StubSize = s.StubSize,
                                                SnapoutLoc = s.SnapoutLoc,
                                                Received = s.Received,
                                                PartialReceived = s.PartialReceived,
                                                DateReceived = s.DateReceived,
                                                QtyReceived = s.QtyReceived,
                                                ReceivedBy = s.ReceivedBy,
                                                InvoiceNum = s.InvoiceNum,
                                                InvoiceDate = s.InvoiceDate,
                                                Extensionq = s.Extensionq,
                                                ItemComplete = s.ItemComplete,
                                                PublishWSS = s.PublishWSS,
                                                Vouchered = s.Vouchered,
                                                CheckedBy = s.ContactName
                                            })
                                            .ToList();

                            if (purchaseItems != null)
                            {
                                if (po.POType == 2)
                                {
                                    foreach (var poItem in purchaseItems)
                                    {
                                        var contItems = _salesDB.PurchaseOrderDetailsCont
                                            .Where(i => i.PODetailID == poItem.PODetailID)
                                            .Select(s => new PurchaseContinuousDetails
                                            {
                                                ContPaperDetailID = s.ContPaperDetailID,
                                                PODetailID = s.PODetailID,
                                                StockNo = s.StockNo,
                                                StockColor = s.StockColor,
                                                StockGrade = s.StockGrade,
                                                StockWeight = s.StockWeight,
                                                StockSize = s.StockSize,
                                                Ink1st = s.Ink1st,
                                                Ink2nd = s.Ink2nd,
                                                Ink3rd = s.Ink3rd,
                                                PolyPlate = s.PolyPlate,
                                                FCChg = s.FCChg,
                                                FCSame = s.FCSame,
                                                MargLeft = s.MargLeft,
                                                MargRight = s.MargRight,
                                                SpecPerfs = s.SpecPerfs,
                                                MargWords = s.MargWords
                                            })
                                            .ToList();

                                        if(contItems != null)
                                        {
                                            poItem.ContinuousItems = contItems;

                                            contItems = null;
                                        }
                                    }
                                }

                                po.PurchaseOrderItems = purchaseItems;
                                po.Total = purchaseItems.Sum(s => s.Extensionq);

                                purchaseItems = null;
                            }
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

        public ActionResult Add(int? so)
        {
            if (Session["UserId"] != null)
            {
                ViewBag.SalesOrder = so;

                ViewBag.POTypeList = new SelectList(_salesDB.PurchaseOrderTypes.Where(p => p.POTypeID <= 4).ToList(), "POTypeID", "POType");

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Create(int? so, int? type)
        {
            var existingPurchaseOrders = _salesDB.PurchaseOrders.Where(p => p.SalesOrderNo == so).ToList();
            
            var newPurchaseOrderNo = existingPurchaseOrders.Count() + 1;
            int newSupplyOrderId = int.TryParse(so.ToString() + newPurchaseOrderNo.ToString(), out newSupplyOrderId) ? newSupplyOrderId : 0;

            int? customerId = _salesDB.SalesOrders.Where(s => s.SalesOrderNo == so).Select(s => s.BillCustomerNo).FirstOrDefault();
            string customerName = _salesDB.Customers.Where(c => c.CustomerNo == customerId).Select(c => c.CustomerName).FirstOrDefault();

            tblPO purchaseOrder = new tblPO()
            {
                SalesOrderNo = so,
                PONumber = so.ToString() + "-" + newPurchaseOrderNo,
                SupplyOrderID = newSupplyOrderId,
                POTypeID = type,
                OrderDate = DateTime.Now,
                AESContactID = Int32.Parse(Session["UserId"].ToString()),
                forCustomer = customerName,
                Misc = false
            };

            _salesDB.PurchaseOrders.Add(purchaseOrder);

            try
            {
                _salesDB.SaveChanges();
            }
            catch (Exception e)
            {
                var test = e;
            }

            return RedirectToAction("Edit", "Purchase", new { po = purchaseOrder.PONumber });
        }

        public virtual JsonResult FilteredVendors(string vendname)
        {
            var vendList = _salesDB.Vendors
                .Where(c => c.VendorName.StartsWith(vendname))
                .Select(x => new SelectListItem { Value = x.VendorNo.ToString(), Text = x.VendorName })
                .ToList();

            // Convert list object to Json object
            return Json(vendList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItem(string po) 
        {
            var purchaseType = _salesDB.PurchaseOrders.Where(p => p.PONumber == po).Select(p => p.POTypeID).FirstOrDefault();

            if (purchaseType == 3)
            {
                var supplyId = _salesDB.PurchaseOrders.Where(p => p.PONumber == po).Select(p => p.SupplyOrderID).FirstOrDefault();
                var maxTransactionID = _salesDB.SupplyTransactions.Max(i => i.TransactionID);
                var maxItemNo = _salesDB.SupplyTransactions.Where(i => i.SupplyOrderID == supplyId).Max(i => i.ItemNo);

                tblSupplyTransactions supplyItem = new tblSupplyTransactions()
                {
                    TransactionID = maxTransactionID + 1,
                    SupplyOrderID = supplyId,
                    ItemNo = (maxItemNo ?? 0) + 1,
                    AddTax = false,
                    Received = false,
                    PartialReceived = false,
                    PublishWSS = false,
                    ItemComplete = false,
                    Vouchered = false
                };

                _salesDB.SupplyTransactions.Add(supplyItem);
            }
            else
            {
                var maxPODetailID = _salesDB.PurchaseOrderDetails.Max(i => i.PODetailID);
                var maxItemNo = _salesDB.PurchaseOrderDetails.Where(i => i.PONumber == po).Max(i => i.ItemNo);

                tblPODetail purchaseItem = new tblPODetail()
                {
                    PODetailID = maxPODetailID + 1,
                    PONumber = po,
                    ItemNo = (maxItemNo ?? 0) + 1,
                    Numbering = false,
                    NoMissing = false,
                    Punching = false,
                    Received = false,
                    PartialReceived = false,
                    ItemComplete = false,
                    PublishWSS = false,
                    Vouchered = false
                };

                _salesDB.PurchaseOrderDetails.Add(purchaseItem);
            }

            try
            {
                _salesDB.SaveChanges();
            }
            catch (Exception e)
            {
                var test = e;
            }

            return RedirectToAction("Edit", "Purchase", new { po = po });
        }

        public ActionResult AddContinuousItem(int? po, int? pod)
        {
            var maxContPaperDetailID = _salesDB.PurchaseOrderDetailsCont.Max(i => i.ContPaperDetailID);
            var maxStockNo = _salesDB.PurchaseOrderDetailsCont.Where(i => i.PODetailID == pod).Max(i => i.StockNo);

            tblPODetailCont continuousItem = new tblPODetailCont()
            {
                ContPaperDetailID = maxContPaperDetailID + 1,
                PODetailID = pod,
                StockNo = (maxStockNo ?? 0) + 1
            };

            _salesDB.PurchaseOrderDetailsCont.Add(continuousItem);

            return RedirectToAction("Edit", "Purchase", new { po = po });
        }

        public ActionResult Edit(string po)
        {
            if (Session["UserId"] != null)
            {
                var query = from purchaseOrders in _salesDB.PurchaseOrders
                            join purchaseVendor in _salesDB.Vendors
                                    on purchaseOrders.VendorNo equals purchaseVendor.VendorNo into vendorgroup
                            from vendor in vendorgroup.DefaultIfEmpty()
                            join purchaseCustomer in _salesDB.Customers
                                    on purchaseOrders.CustomerID equals purchaseCustomer.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join customerAddress in _salesDB.CustomerAddresses
                                    on purchaseOrders.CustAddID equals customerAddress.CustomerNo into customeraddressgroup
                            from custAddress in customeraddressgroup.DefaultIfEmpty()
                            join customerContact in _salesDB.CustomerContacts
                                    on purchaseOrders.ContactID equals customerContact.ContactID into customercontactgroup
                            from custContact in customercontactgroup.DefaultIfEmpty()
                            join purchaseShipper in _salesDB.Shippers
                                    on purchaseOrders.ShipperID equals purchaseShipper.ShipperID into shippergroup
                            from shipper in shippergroup.DefaultIfEmpty()
                            select new
                            {
                                purchaseOrders.PONumber,
                                purchaseOrders.SupplyOrderID,
                                purchaseOrders.SalesOrderNo,
                                purchaseOrders.AESContactID,
                                purchaseOrders.OrderDate,
                                purchaseOrders.OrderStatus,
                                purchaseOrders.AccountCode,
                                purchaseOrders.AccountDescription,
                                purchaseOrders.QuoteNo,
                                purchaseOrders.QuoteAmount,
                                purchaseOrders.ReqShipDate,
                                purchaseOrders.ShipperID,
                                purchaseOrders.ShippingMethod,
                                purchaseOrders.Composition,
                                purchaseOrders.PrevJobNo,
                                purchaseOrders.PrevDate,
                                purchaseOrders.VendorNo,
                                purchaseOrders.CustomerID,
                                purchaseOrders.forCustomer,
                                purchaseOrders.CustAddID,
                                purchaseOrders.ContactID,
                                purchaseOrders.SpecInst,
                                purchaseOrders.POTypeID,
                                purchaseOrders.POStatus,
                                vendor.VendorName,
                                vendor.VendorAddress1,
                                vendor.VendorAddress2,
                                vendor.VendorCity,
                                vendor.VendorState,
                                vendor.VendorZip,
                                vendor.VendorPhone,
                                customer.CustomerName,
                                CustomerContact = custContact.ContactName,
                                CustomerPhone = custContact.ContactPhone,
                                custAddress.ShipAddressID,
                                custAddress.ShipAddress1,
                                custAddress.ShipAddress2,
                                custAddress.ShipCity,
                                custAddress.ShipState,
                                custAddress.ShipZip,
                                shipper.ShipperName
                            };

                var results = query
                              .Where(d => d.PONumber == po)
                              .Select(d => new PurchaseOrderDetails
                              {
                                  PONumber = d.PONumber,
                                  SupplyOrderID = d.SupplyOrderID,
                                  SalesOrderNo = d.SalesOrderNo,
                                  AESContactID = d.AESContactID,
                                  OrderDate = d.OrderDate,
                                  AccountCode = d.AccountCode,
                                  AccountDescription = d.AccountDescription,
                                  QuoteNo = d.QuoteNo,
                                  QuoteAmount = d.QuoteAmount,
                                  ReqShipDate = d.ReqShipDate,
                                  ShipperID = d.ShipperID,
                                  ShippingMethod = d.ShippingMethod,
                                  OrderStatus = d.OrderStatus,
                                  Composition = d.Composition,
                                  PrevJobNo = d.PrevJobNo,
                                  PrevDate = d.PrevDate,
                                  VendorNo = d.VendorNo,
                                  CustomerID = d.CustomerID,
                                  ForCustomer = d.forCustomer,
                                  CustAddID = d.CustAddID,
                                  ContactID = d.ContactID,
                                  SpecInst = d.SpecInst,
                                  POType = d.POTypeID,
                                  POStatus = d.POStatus,
                                  VendorName = d.VendorName,
                                  VendorAddress1 = d.VendorAddress1,
                                  VendorAddress2 = d.VendorAddress2,
                                  VendorCity = d.VendorCity,
                                  VendorState = d.VendorState,
                                  VendorZip = d.VendorZip,
                                  VendorPhone = d.VendorPhone,
                                  CustomerName = d.CustomerName,
                                  CustomerAddressID = d.ShipAddressID,
                                  CustomerAddress1 = d.ShipAddress1,
                                  CustomerAddress2 = d.ShipAddress2,
                                  CustomerCity = d.ShipCity,
                                  CustomerState = d.ShipState,
                                  CustomerZip = d.ShipZip,
                                  CustomerContactName = d.CustomerContact,
                                  CustomerPhone = d.CustomerPhone,
                                  ShipperName = d.ShipperName
                              })
                              .FirstOrDefault();

                // Add Purchase Order Line Items to results
                if (results != null)
                {                    
                    if (results.POType == 3)
                    {
                        if (results.SupplyOrderID != null)
                        {
                            var supplyQuery = from transaction in _salesDB.SupplyTransactions
                                              join supplies in _salesDB.Supplies
                                              on transaction.ProductID equals supplies.ProductID into supplygroup
                                              from supply in supplygroup.DefaultIfEmpty()
                                              join employees in _salesDB.Employees
                                              on transaction.ReceivedBy equals employees.ContactID into employeegroup
                                              from employee in employeegroup.DefaultIfEmpty()
                                              select new
                                              {
                                                  transaction.TransactionID,
                                                  transaction.TransactionDate,
                                                  transaction.ItemNo,
                                                  transaction.ProductCategoryID,
                                                  transaction.ProductID,
                                                  transaction.SupplyOrderID,
                                                  transaction.TransactionDescription,
                                                  transaction.UnitsOrdered,
                                                  transaction.UnitsReceived,
                                                  transaction.UnitsSold,
                                                  transaction.UnitsShrinkage,
                                                  transaction.UnitsShipped,
                                                  transaction.AddTax,
                                                  transaction.TaxValue,
                                                  transaction.Freight,
                                                  transaction.Credit,
                                                  transaction.Extensionq,
                                                  transaction.Received,
                                                  transaction.PartialReceived,
                                                  transaction.DateReceived,
                                                  transaction.QtyReceived,
                                                  transaction.ReceivedBy,
                                                  transaction.InvoiceNum,
                                                  transaction.InvoiceDate,
                                                  transaction.Extension,
                                                  transaction.ItemComplete,
                                                  transaction.Vouchered,
                                                  supply.ProductName,
                                                  employee.ContactName
                                              };

                            var supplyItems = supplyQuery
                                            .Where(i => i.SupplyOrderID == results.SupplyOrderID)
                                            .Select(s => new SupplyTransactionDetails
                                            {
                                                TransactionID = s.TransactionID,
                                                TransactionDate = s.TransactionDate,
                                                ItemNo = s.ItemNo,
                                                ProductCategoryID = s.ProductCategoryID,
                                                ProductID = s.ProductID,
                                                SupplyOrderID = s.SupplyOrderID,
                                                TransactionDescription = s.TransactionDescription,
                                                UnitsOrdered = s.UnitsOrdered,
                                                UnitsReceived = s.UnitsReceived,
                                                UnitsSold = s.UnitsSold,
                                                UnitsShrinkage = s.UnitsShrinkage,
                                                UnitsShipped = s.UnitsShipped,
                                                AddTax = s.AddTax,
                                                TaxValue = s.TaxValue,
                                                Freight = s.Freight,
                                                Credit = s.Credit,
                                                Extensionq = s.Extensionq,
                                                Received = s.Received,
                                                PartialReceived = s.PartialReceived,
                                                DateReceived = s.DateReceived,
                                                QtyReceived = s.QtyReceived,
                                                ReceivedBy = s.ReceivedBy,
                                                InvoiceNum = s.InvoiceNum,
                                                InvoiceDate = s.InvoiceDate,
                                                Extension = s.Extension,
                                                ItemComplete = s.ItemComplete,
                                                Vouchered = s.Vouchered,
                                                ProductName = s.ProductName,
                                                CheckedBy = s.ContactName
                                            })
                                            .ToList();

                            if (supplyItems != null)
                            {
                                results.SupplyItems = supplyItems;
                                results.SupplyTotal = supplyItems.Sum(s => s.Extension);

                                supplyItems = null;
                            }
                        }
                    }
                    else
                    {
                        var poQuery = from purchase in _salesDB.PurchaseOrderDetails
                                        join employees in _salesDB.Employees
                                        on purchase.ReceivedBy equals employees.ContactID into employeegroup
                                        from employee in employeegroup.DefaultIfEmpty()
                                        select new
                                        {
                                            purchase.PODetailID,
                                            purchase.PONumber,
                                            purchase.ItemNo,
                                            purchase.QtyOrdered,
                                            purchase.ItemDescription,
                                            purchase.PerUnit,
                                            purchase.UnitCost,
                                            purchase.Extension,
                                            purchase.StockWeight,
                                            purchase.StockColor,
                                            purchase.StockGrade,
                                            purchase.StockSize,
                                            purchase.Ink1st,
                                            purchase.Ink2nd,
                                            purchase.Ink3rd,
                                            purchase.Ink4th,
                                            purchase.Numbering,
                                            purchase.StartNumber,
                                            purchase.NumberColor,
                                            purchase.NumberStyle,
                                            purchase.NoMissing,
                                            purchase.MICR,
                                            purchase.Construction,
                                            purchase.Punching,
                                            purchase.PunchSize,
                                            purchase.PunchNo,
                                            purchase.CtoC,
                                            purchase.PunchLoc,
                                            purchase.FromEdge,
                                            purchase.Press,
                                            purchase.Bind,
                                            purchase.Parts,
                                            purchase.Carbon,
                                            purchase.StubSize,
                                            purchase.SnapoutLoc,
                                            purchase.Received,
                                            purchase.PartialReceived,
                                            purchase.DateReceived,
                                            purchase.QtyReceived,
                                            purchase.ReceivedBy,
                                            purchase.InvoiceNum,
                                            purchase.InvoiceDate,
                                            purchase.Extensionq,
                                            purchase.ItemComplete,
                                            purchase.PublishWSS,
                                            purchase.Vouchered,
                                            employee.ContactName
                                        };

                        var purchaseItems = poQuery
                                        .Where(i => i.PONumber == results.PONumber)
                                        .Select(s => new PurchaseItemDetails
                                        {
                                            PODetailID = s.PODetailID,
                                            PONumber = s.PONumber,
                                            ItemNo = s.ItemNo,
                                            Quantity = s.QtyOrdered,
                                            ItemDescription = s.ItemDescription,
                                            PricePerUnit = s.PerUnit??0,
                                            UnitCost = s.UnitCost,
                                            Extension = s.Extension,
                                            StockWeight = s.StockWeight,
                                            StockColor = s.StockColor,
                                            StockGrade = s.StockGrade,
                                            StockSize = s.StockSize,
                                            Ink1st = s.Ink1st,
                                            Ink2nd = s.Ink2nd,
                                            Ink3rd = s.Ink3rd,
                                            Ink4th = s.Ink4th,
                                            Numbering = s.Numbering,
                                            StartNumber = s.StartNumber,
                                            NumberColor = s.NumberColor,
                                            NumberStyle = s.NumberStyle,
                                            NoMissing = s.NoMissing,
                                            MICR = s.MICR,
                                            Construction = s.Construction,
                                            Punching = s.Punching,
                                            PunchSize = s.PunchSize,
                                            PunchNo = s.PunchNo,
                                            CtoC = s.CtoC,
                                            PunchLoc = s.PunchLoc,
                                            FromEdge = s.FromEdge,
                                            Press = s.Press,
                                            Bind = s.Bind,
                                            Parts = s.Parts,
                                            Carbon = s.Carbon,
                                            StubSize = s.StubSize,
                                            SnapoutLoc = s.SnapoutLoc,
                                            Received = s.Received,
                                            PartialReceived = s.PartialReceived,
                                            DateReceived = s.DateReceived,
                                            QtyReceived = s.QtyReceived,
                                            ReceivedBy = s.ReceivedBy,
                                            InvoiceNum = s.InvoiceNum,
                                            InvoiceDate = s.InvoiceDate,
                                            Extensionq = s.Extensionq,
                                            ItemComplete = s.ItemComplete,
                                            PublishWSS = s.PublishWSS,
                                            Vouchered = s.Vouchered,
                                            CheckedBy = s.ContactName
                                        })
                                        .OrderBy(o => o.ItemNo)
                                        .ToList();

                        if (purchaseItems != null)
                        {
                            if (results.POType == 2)
                            {
                                foreach (var poItem in purchaseItems)
                                {
                                    var contItems = _salesDB.PurchaseOrderDetailsCont
                                        .Where(i => i.PODetailID == poItem.PODetailID)
                                        .Select(s => new PurchaseContinuousDetails
                                        {
                                            ContPaperDetailID = s.ContPaperDetailID,
                                            PODetailID = s.PODetailID,
                                            StockNo = s.StockNo,
                                            StockColor = s.StockColor,
                                            StockGrade = s.StockGrade,
                                            StockWeight = s.StockWeight,
                                            StockSize = s.StockSize,
                                            Ink1st = s.Ink1st,
                                            Ink2nd = s.Ink2nd,
                                            Ink3rd = s.Ink3rd,
                                            PolyPlate = s.PolyPlate,
                                            FCChg = s.FCChg,
                                            FCSame = s.FCSame,
                                            MargLeft = s.MargLeft,
                                            MargRight = s.MargRight,
                                            SpecPerfs = s.SpecPerfs,
                                            MargWords = s.MargWords
                                        })
                                        .ToList();

                                    if (contItems != null)
                                    {
                                        poItem.ContinuousItems = contItems;

                                        contItems = null;
                                    }
                                }
                            }

                            results.PurchaseOrderItems = purchaseItems;
                            results.Total = purchaseItems.Sum(s => s.Extensionq);

                            purchaseItems = null;
                        }
                    }
                    
                }
                else
                {
                    //ViewBag.SalesOrder = so;
                }

                ViewBag.POTypeList = new SelectList(_salesDB.PurchaseOrderTypes.Where(p => p.POTypeID <= 4).ToList(), "POTypeID", "POType", results.POType);
                ViewBag.EmployeeList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName", results.AESContactID);
                ViewBag.POStatusList = POStatusList(results.POStatus);
                ViewBag.AccountingList = new SelectList(_salesDB.Ledger.Where(a => a.GLCode.StartsWith("4") && a.POList == true).ToList(), "GLCode", "GLCode", results.AccountCode); 
                ViewBag.ShipperList = new SelectList(_salesDB.Shippers.ToList(), "ShipperID", "ShipperName", results.ShipperID);
                ViewBag.ShippingMethodList = new SelectList(_salesDB.ShippingMethods.Where(m => m.ShipperID == results.ShipperID).ToList(), "ShippingMethod", "ShippingMethod", results.ShippingMethod); 
                ViewBag.OrderStatusList = OrderStatusList();
                ViewBag.CompositionList = CompositionList();
                ViewBag.VendorList = new SelectList(_salesDB.Vendors.ToList(), "VendorNo", "VendorName", results.VendorNo);
                ViewBag.CustomerList = new SelectList(_salesDB.Customers.ToList(), "CustomerNo", "CustomerName", results.CustomerID);
                ViewBag.CustomerAddressList = new SelectList(_salesDB.CustomerAddresses.Where(c => c.CustomerNo == results.CustomerID).ToList(), "ShipAddressID", "ShipAddress1", results.CustomerAddressID);
                ViewBag.PricingList2 = new SelectList(_salesDB.PricingUnits.ToList(), "Unit", "UnitName");
                ViewBag.ReceivedList = RecievedByList();
                ViewBag.SuppliesList = new SelectList(_salesDB.Supplies.ToList(), "ProductID", "ProductName");
                ViewBag.NumberStyleList = NumberStyleList();
                ViewBag.MICRList = MICRList();

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public bool UpdatePurchaseOrder(PurchaseOrderDetails OrderToUpdate)
        {
            if (OrderToUpdate != null)
            {
                var purchaseOrder = _salesDB.PurchaseOrders.Where(p => p.PONumber == OrderToUpdate.PONumber).FirstOrDefault();

                if (purchaseOrder != null) 
                {
                    // Get new account description
                    OrderToUpdate.AccountDescription = _salesDB.Ledger.Where(v => v.GLCode == OrderToUpdate.AccountCode).Select(v => v.Description).FirstOrDefault();

                    purchaseOrder.POTypeID = OrderToUpdate.POType;
                    purchaseOrder.AESContactID = OrderToUpdate.AESContactID;
                    purchaseOrder.POStatus = OrderToUpdate.POStatus;
                    purchaseOrder.QuoteNo = OrderToUpdate.QuoteNo;
                    purchaseOrder.QuoteAmount = OrderToUpdate.QuoteAmount;
                    purchaseOrder.AccountCode = OrderToUpdate.AccountCode;
                    purchaseOrder.AccountDescription = OrderToUpdate.AccountDescription;
                    purchaseOrder.OrderDate = OrderToUpdate.OrderDate;
                    purchaseOrder.ReqShipDate = OrderToUpdate.ReqShipDate;
                    purchaseOrder.ShipperID = OrderToUpdate.ShipperID;
                    purchaseOrder.ShippingMethod = OrderToUpdate.ShippingMethod;
                    purchaseOrder.OrderStatus = OrderToUpdate.OrderStatus;
                    purchaseOrder.Composition = OrderToUpdate.Composition;
                    purchaseOrder.PrevJobNo = OrderToUpdate.PrevJobNo;
                    purchaseOrder.PrevDate = OrderToUpdate.PrevDate;
                    purchaseOrder.VendorNo = OrderToUpdate.VendorNo;
                    purchaseOrder.CustomerID = OrderToUpdate.CustomerID;
                    purchaseOrder.CustAddID = OrderToUpdate.CustomerAddressID;

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

        public bool UpdateEnvelopeItem(PurchaseItemDetails ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var purchaseItem = _salesDB.PurchaseOrderDetails.Where(s => s.PODetailID == ItemToUpdate.PODetailID).FirstOrDefault();

                if (purchaseItem != null)
                {
                    purchaseItem.ItemNo = ItemToUpdate.ItemNo;
                    purchaseItem.QtyOrdered = ItemToUpdate.Quantity;
                    purchaseItem.ItemDescription = ItemToUpdate.ItemDescription;
                    purchaseItem.UnitCost = ItemToUpdate.UnitCost;
                    purchaseItem.PerUnit = ItemToUpdate.PricePerUnit;
                    purchaseItem.Extensionq = ItemToUpdate.Extensionq;
                    purchaseItem.StockWeight = ItemToUpdate.StockWeight;
                    purchaseItem.StockColor = ItemToUpdate.StockColor;
                    purchaseItem.StockGrade = ItemToUpdate.StockGrade;
                    purchaseItem.StockSize = ItemToUpdate.StockSize;
                    purchaseItem.Ink1st = ItemToUpdate.Ink1st;
                    purchaseItem.Ink2nd = ItemToUpdate.Ink2nd;
                    purchaseItem.Ink3rd = ItemToUpdate.Ink3rd;
                    purchaseItem.Ink4th = ItemToUpdate.Ink4th;
                    purchaseItem.Received = ItemToUpdate.Received;
                    purchaseItem.PartialReceived = ItemToUpdate.PartialReceived;
                    purchaseItem.DateReceived = ItemToUpdate.DateReceived;
                    purchaseItem.QtyReceived = ItemToUpdate.QtyReceived;

                    if (ItemToUpdate.ReceivedBy > 0)
                    {
                        purchaseItem.ReceivedBy = ItemToUpdate.ReceivedBy;
                    }
                    else
                    {
                        purchaseItem.ReceivedBy = null;
                    }

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

        public bool UpdateContinuousItem(PurchaseItemDetails ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var purchaseItem = _salesDB.PurchaseOrderDetails.Where(s => s.PODetailID == ItemToUpdate.PODetailID).FirstOrDefault();

                if (purchaseItem != null)
                {
                    purchaseItem.ItemNo = ItemToUpdate.ItemNo;
                    purchaseItem.QtyOrdered = ItemToUpdate.Quantity;
                    purchaseItem.ItemDescription = ItemToUpdate.ItemDescription;
                    purchaseItem.UnitCost = ItemToUpdate.UnitCost;
                    purchaseItem.PerUnit = ItemToUpdate.PricePerUnit;
                    purchaseItem.Extensionq = ItemToUpdate.Extensionq;
                    purchaseItem.Numbering = ItemToUpdate.Numbering;
                    purchaseItem.StartNumber = ItemToUpdate.StartNumber;
                    purchaseItem.NumberColor = ItemToUpdate.NumberColor;
                    purchaseItem.NumberStyle = ItemToUpdate.NumberStyle;
                    purchaseItem.NoMissing = ItemToUpdate.NoMissing;
                    purchaseItem.MICR = ItemToUpdate.MICR;
                    purchaseItem.Construction = ItemToUpdate.Construction;
                    purchaseItem.Punching = ItemToUpdate.Punching;
                    purchaseItem.PunchSize = ItemToUpdate.PunchSize;
                    purchaseItem.PunchNo = ItemToUpdate.PunchNo;
                    purchaseItem.CtoC = ItemToUpdate.CtoC;
                    purchaseItem.FromEdge = ItemToUpdate.FromEdge;
                    purchaseItem.Bind = ItemToUpdate.Bind;
                    purchaseItem.StubSize = ItemToUpdate.StubSize;
                    purchaseItem.PunchLoc = ItemToUpdate.PunchLoc;

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

        public bool UpdateContinuousSubItem(PurchaseContinuousDetails ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var purchaseItem = _salesDB.PurchaseOrderDetailsCont.Where(s => s.ContPaperDetailID == ItemToUpdate.ContPaperDetailID).FirstOrDefault();

                if (purchaseItem != null)
                {
                    purchaseItem.StockNo = ItemToUpdate.StockNo;
                    purchaseItem.StockWeight = ItemToUpdate.StockWeight;
                    purchaseItem.StockColor = ItemToUpdate.StockColor;
                    purchaseItem.StockGrade = ItemToUpdate.StockGrade;
                    purchaseItem.StockSize = ItemToUpdate.StockSize;
                    purchaseItem.Ink1st = ItemToUpdate.Ink1st;
                    purchaseItem.Ink2nd = ItemToUpdate.Ink2nd;
                    purchaseItem.Ink3rd = ItemToUpdate.Ink3rd;
                    purchaseItem.PolyPlate = ItemToUpdate.PolyPlate;
                    purchaseItem.FCChg = ItemToUpdate.FCChg;
                    purchaseItem.FCSame = ItemToUpdate.FCSame;
                    purchaseItem.MargLeft = ItemToUpdate.MargLeft;
                    purchaseItem.MargRight = ItemToUpdate.MargRight;
                    purchaseItem.SpecPerfs = ItemToUpdate.SpecPerfs;
                    purchaseItem.MargWords = ItemToUpdate.MargWords;

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

        public bool UpdateSupplyItem(SupplyTransactionDetails ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var purchaseItem = _salesDB.SupplyTransactions.Where(s => s.TransactionID == ItemToUpdate.TransactionID).FirstOrDefault();

                if (purchaseItem != null)
                {
                    purchaseItem.ItemNo = ItemToUpdate.ItemNo;
                    purchaseItem.UnitsSold = ItemToUpdate.UnitsSold;
                    purchaseItem.UnitsOrdered = ItemToUpdate.UnitsOrdered;
                    purchaseItem.ProductID = ItemToUpdate.ProductID;
                    purchaseItem.TransactionDescription = ItemToUpdate.TransactionDescription;
                    purchaseItem.Freight = ItemToUpdate.Freight;
                    purchaseItem.AddTax = ItemToUpdate.AddTax??false;
                    purchaseItem.TaxValue = ItemToUpdate.TaxValue;
                    purchaseItem.Extensionq = ItemToUpdate.Extensionq;
                    purchaseItem.Received = ItemToUpdate.Received;
                    purchaseItem.PartialReceived = ItemToUpdate.PartialReceived;
                    purchaseItem.DateReceived = ItemToUpdate.DateReceived;
                    purchaseItem.QtyReceived = ItemToUpdate.QtyReceived;

                    if (ItemToUpdate.ReceivedBy > 0)
                    {
                        purchaseItem.ReceivedBy = ItemToUpdate.ReceivedBy;
                    }
                    else
                    {
                        purchaseItem.ReceivedBy = null;
                    }

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

        public bool UpdateMiscellaneousItem(PurchaseItemDetails ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var purchaseItem = _salesDB.PurchaseOrderDetails.Where(s => s.PODetailID == ItemToUpdate.PODetailID).FirstOrDefault();

                if (purchaseItem != null)
                {
                    purchaseItem.ItemNo = ItemToUpdate.ItemNo;
                    purchaseItem.QtyOrdered = ItemToUpdate.Quantity;
                    purchaseItem.ItemDescription = ItemToUpdate.ItemDescription;
                    purchaseItem.UnitCost = ItemToUpdate.UnitCost;
                    purchaseItem.PerUnit = ItemToUpdate.PricePerUnit;
                    purchaseItem.Extensionq = ItemToUpdate.Extensionq;
                    purchaseItem.Received = ItemToUpdate.Received;
                    purchaseItem.PartialReceived = ItemToUpdate.PartialReceived;
                    purchaseItem.DateReceived = ItemToUpdate.DateReceived;
                    purchaseItem.QtyReceived = ItemToUpdate.QtyReceived;

                    if (ItemToUpdate.ReceivedBy > 0)
                    {
                        purchaseItem.ReceivedBy = ItemToUpdate.ReceivedBy;
                    }
                    else
                    {
                        purchaseItem.ReceivedBy = null;
                    }

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

        private IEnumerable<SelectListItem> POStatusList(int? selected)
        {
            IEnumerable<SelectListItem> PoStatus = new List<SelectListItem>()
                {
                new SelectListItem() { Text = "Open", Value = "1", Selected = selected == 1 },
                new SelectListItem() { Text = "Closed", Value = "2", Selected = selected == 2 },
                new SelectListItem() { Text = "Backordered", Value = "3", Selected = selected == 3 }
                };

            return PoStatus;
        }

        private IEnumerable<SelectListItem> OrderStatusList()
        {
            IEnumerable<SelectListItem> OrderStatus = new List<SelectListItem>()
                {
                new SelectListItem() { Text = "New", Value = "New" },
                new SelectListItem() { Text = "Exact Repeat", Value = "Exact Repeat" },
                new SelectListItem() { Text = "Changed Repeat - Copy Chges", Value = "Changed Repeat - Copy Chges" },
                new SelectListItem() { Text = "Changed Repeat - Spec Chges", Value = "Changed Repeat - Spec Chges" }
                };

            return OrderStatus;
        }

        private IEnumerable<SelectListItem> CompositionList()
        {
            IEnumerable<SelectListItem> Composition = new List<SelectListItem>()
                {
                new SelectListItem() { Text = "Negative Furnished", Value = "Negative Furnished" },
                new SelectListItem() { Text = "Artwork Furnished", Value = "Artwork Furnished" },
                new SelectListItem() { Text = "Sample Attached", Value = "Sample Attached" },
                new SelectListItem() { Text = "Electronic File", Value = "Electronic File" }
                };

            return Composition;
        }

        private IEnumerable<SelectListItem> NumberStyleList()
        {
            IEnumerable<SelectListItem> NumberStyles = new List<SelectListItem>()
                {
                new SelectListItem() { Text = "Press", Value = "Press" },
                new SelectListItem() { Text = "Crash", Value = "Crash" }
                };

            return NumberStyles;
        }

        private IEnumerable<SelectListItem> MICRList()
        {
            IEnumerable<SelectListItem> MICR = new List<SelectListItem>()
                {
                new SelectListItem() { Text = "Yes - Consecutive", Value = "Yes - Consecutive" },
                new SelectListItem() { Text = "Yes - Spec Sheet", Value = "Yes - Spec Sheet" },
                new SelectListItem() { Text = "No", Value = "No" }
                };

            return MICR;
        }

        private IEnumerable<SelectListItem> RecievedByList()
        {
            // Convert employee table to list of SelectListItem
            var employees = ((IEnumerable<SelectListItem>)new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName")).ToList();

            // Create blank item
            var item = new SelectListItem() { Text = "", Value = "0" };

            // Add blank employee
            employees.Add(item);

            // Return a new, sorted list
            return new SelectList(employees.OrderBy(o => o.Value), "Value", "Text"); ;
        }

        //public IEnumerable<SelectListItem> CustomerAddressList(int? id)
        //{
        //    var test = new SelectList(_salesDB.CustomerAddresses.Where(c => c.CustomerNo == id).ToList(), "ShipAddressID", "ShipAddress1");
        //    return new SelectList(_salesDB.CustomerAddresses.Where(c => c.CustomerNo == id).ToList(), "ShipAddressID", "ShipAddress1");
        //}

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

        public JsonResult VendorDetails(int? id)
        {
            var vendor = _salesDB.Vendors.Where(v => v.VendorNo == id).FirstOrDefault();

            return Json(vendor, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddressDetails(int? id)
        {
            var address = _salesDB.CustomerAddresses.Where(v => v.ShipAddressID == id).FirstOrDefault();

            return Json(address, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AccountDetails(string code)
        {
            //_salesDB.Ledger.Where(a => a.GLCode.StartsWith("4") && a.POList == true).ToList()
            var account = _salesDB.Ledger.Where(v => v.GLCode == code).FirstOrDefault();

            return Json(account, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult FilteredShippingMethods(int? id)
        {
            var methodsList = _salesDB.ShippingMethods
                .Where(m => m.ShipperID == id)
                .Select(x => new SelectListItem { Value = x.ShippingMethod.ToString(), Text = x.ShippingMethod })
                .ToList();

            // Convert list object to Json object
            //string jsonList = new JavaScriptSerializer().Serialize(DayList);
            return Json(methodsList, JsonRequestBehavior.AllowGet);
        }

    }
}