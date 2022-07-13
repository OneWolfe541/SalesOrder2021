using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class SalesController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        // GET: Sales
        public ActionResult Search(int? statid, string custid, string custname)
        {
            if (Session["UserId"] != null)
            {
                var query = from salesOrders in _salesDB.SalesOrders
                            join salesItems in _salesDB.SalesOrderItems
                                    on salesOrders.SalesOrderNo equals salesItems.SalesOrderNo into salesitemsgroup
                            from items in salesitemsgroup.DefaultIfEmpty()
                            join salesStatus in _salesDB.SalesOrderStatus
                                    on salesOrders.SOStatusID equals salesStatus.SOStatusID into salesstatusgroup
                            from status in salesstatusgroup.DefaultIfEmpty()
                            join salesCustomer in _salesDB.Customers
                                    on salesOrders.CustomerID equals salesCustomer.CustomerID into salescustomergroup
                            from customer in salescustomergroup.DefaultIfEmpty()
                            select new
                            {
                                salesOrders.SalesOrderNo,
                                salesOrders.OrderDate,
                                customer.CustomerID,
                                customer.CustLookupIndex,
                                customer.CustomerName,
                                items.ItemRecordID,
                                items.ItemNo,
                                items.GenDescription,
                                status.SOStatusID,
                                status.SOStatus
                            };

                if (statid == null) statid = 1;
                if (custid == "") custid = null;

                var searchResults = query
                              .Where(d => d.SOStatusID == statid && (d.CustomerID == custid || custid == null) && d.ItemNo == 1
                              && d.CustomerID != null && d.GenDescription != null)
                              .Select(d => new SalesOrderSearchDetails
                              {
                                  SalesOrderNo = d.SalesOrderNo,
                                  OrderDate = d.OrderDate,
                                  CustomerID = d.CustomerID,
                                  CustLookupIndex = d.CustLookupIndex,
                                  CustomerName = d.CustomerName,
                                  ItemRecordID = d.ItemRecordID,
                                  ItemNo = d.ItemNo,
                                  GenDescription = d.GenDescription,
                                  SOStatusID = d.SOStatusID,
                                  SOStatus = d.SOStatus
                              })
                              .OrderByDescending(o => o.OrderDate)
                              .Take(1000)
                              .ToList();

                ViewBag.DocumentTypes = new SelectList(DocumentTypes(), "Value", "Text", "Sales Order");
                ViewBag.SalesStatusList = new SelectList(_salesDB.SalesOrderStatus.ToList(), "SOStatusID", "SOStatus", statid);
                ViewBag.CustomerList = new SelectList(_salesDB.Customers.ToList(), "CustomerID", "CustomerName", custid);
                ViewBag.CustomerName = custname;

                return View(searchResults);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        private List<SelectListItem> DocumentTypes()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Text="Sales Order", Value="1"},
                new SelectListItem() { Text="Purchase Order", Value="2"},
                new SelectListItem() { Text="Shipping Order", Value="3"},
                new SelectListItem() { Text="Election Order", Value="4"}
            };
        }

        public virtual JsonResult FilteredCustomers(string custname)
        {
            var custList = _salesDB.Customers
                .Where(c => c.CustomerName.StartsWith(custname))
                .Select(x => new SelectListItem { Value = x.CustomerID.ToString(), Text = x.CustomerName })
                .ToList();

            // Convert list object to Json object
            //string jsonList = new JavaScriptSerializer().Serialize(DayList);
            return Json(custList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Order(int? so)
        {
            if (Session["UserId"] != null)
            {
                var query = from salesOrders in _salesDB.SalesOrders
                            join salesCustomer in _salesDB.CustomerContacts
                                    on salesOrders.CustomerContact equals salesCustomer.ContactID into salescustomergroup
                            from customer in salescustomergroup.DefaultIfEmpty()
                            join billingCustomer in _salesDB.Customers
                                    on salesOrders.BillCustomerNo equals billingCustomer.CustomerNo into billingcustomergroup
                            from billing in billingcustomergroup.DefaultIfEmpty()
                            join salesEmployee in _salesDB.Employees
                                    on salesOrders.Createdby equals salesEmployee.ContactID into salesemployeegroup
                            from employee in salesemployeegroup.DefaultIfEmpty()
                            join salesStatus in _salesDB.SalesOrderStatus
                                    on salesOrders.SOStatusID equals salesStatus.SOStatusID into salesstatusgroup
                            from status in salesstatusgroup.DefaultIfEmpty()
                            select new
                            {
                                salesOrders.SalesOrderNo,
                                salesOrders.OrderDate,
                                salesOrders.AccountCode,
                                salesOrders.ReqShipDate,
                                salesOrders.ActShipDate,
                                salesOrders.Createdby,
                                salesOrders.BillCustomerNo,
                                salesOrders.CustomerPO,
                                salesOrders.CustomerID,
                                salesOrders.CustomerContact,
                                salesOrders.SalesOrderTotal,
                                salesOrders.SalesOrderCost,
                                salesOrders.TaxStatus,
                                salesOrders.TaxValue,
                                salesOrders.SOStatusID,
                                customer.ContactName,
                                customer.ContactPhone,
                                customer.ContactFax,
                                billing.CustomerName,
                                billing.BillAddress1,
                                billing.BillAddress2,
                                billing.BillCity,
                                billing.BillState,
                                billing.BillZip,
                                EmployeeName = employee.ContactName,
                                status.SOStatus
                            };

                int id = so ?? 0;
                var results = query
                              .Where(d => d.SalesOrderNo == id)
                              .Select(d => new SalesOrderDetails
                              {
                                  SalesOrderNo = d.SalesOrderNo,
                                  OrderDate = d.OrderDate,
                                  AccountCode = d.AccountCode,
                                  ReqShipDate = d.ReqShipDate,
                                  ActShipDate = d.ActShipDate,
                                  Createdby = d.Createdby,
                                  CreatedbyName = d.EmployeeName,
                                  BillCustomerNo = d.BillCustomerNo,
                                  CustomerPO = d.CustomerPO,
                                  CustomerID = d.CustomerID,
                                  CustomerContact = d.CustomerContact,
                                  SalesOrderTotal = d.SalesOrderTotal,
                                  SalesOrderCost = d.SalesOrderCost,
                                  TaxStatus = d.TaxStatus,
                                  TaxValue = d.TaxValue,
                                  SOStatusID = d.SOStatusID,
                                  ContactName = d.ContactName,
                                  MainPhone = d.ContactPhone,
                                  MainFax = d.ContactFax,
                                  CustomerName = d.CustomerName,
                                  BillAddress1 = d.BillAddress1,
                                  BillAddress2 = d.BillAddress2,
                                  BillCity = d.BillCity,
                                  BillState = d.BillState,
                                  BillZip = d.BillZip,
                                  SOStatus = d.SOStatus
                              })
                              .FirstOrDefault();

                // Add Sales Order Line Items to results
                if (results != null)
                {
                    var salesItems = _salesDB.SalesOrderItems
                                    .Where(i => i.SalesOrderNo == results.SalesOrderNo)
                                    .Select(s => new SalesItemDetails
                                    {
                                        ItemRecordID = s.ItemRecordID,
                                        SalesOrderNo = s.SalesOrderNo,
                                        ItemNo = s.ItemNo,
                                        Quantity = s.Quantity,
                                        GenDescription = s.GenDescription,
                                        PricePerUnit = s.PricePerUnit,
                                        CustPrice = s.CustPrice,
                                        Extension = s.Extension
                                    })
                                    .ToList();

                    if (salesItems != null)
                    {
                        results.SalesOrderItems = salesItems;

                        salesItems = null;
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

        public ActionResult Add(string custid, string custname)
        {
            if (Session["UserId"] != null)
            {
                ViewBag.CustomerList = new SelectList(_salesDB.Customers.ToList(), "CustomerID", "CustomerName", custid);
                ViewBag.CustomerName = custname;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Create(string custId)
        {
            var maxSalesOrderNo = _salesDB.SalesOrders.Max(i => i.SalesOrderNo);
            var customerNumber = _salesDB.Customers.Where(c => c.CustomerID == custId).Select(c => c.CustomerNo).FirstOrDefault();

            tblSalesOrder salesOrder = new tblSalesOrder()
            {
                SalesOrderNo = maxSalesOrderNo + 1,
                OrderDate = DateTime.Now,
                SOStatusID = 1,
                BillCustomerNo = customerNumber,
                CustomerID = custId,
                Createdby = Int32.Parse(Session["UserId"].ToString()),
                MiscSalesOrder = false
            };

            _salesDB.SalesOrders.Add(salesOrder);

            try
            {
                _salesDB.SaveChanges();
            }
            catch (Exception e)
            {
                var test = e;
            }

            return RedirectToAction("Edit", "Sales", new { so = salesOrder.SalesOrderNo } );
        }

        public ActionResult AddItem(int? so)
        {
            var maxRecordId = _salesDB.SalesOrderItems.Max(i => i.ItemRecordID);
            var maxItemNo = _salesDB.SalesOrderItems.Where(i => i.SalesOrderNo == so).Max(i => i.ItemNo);

            tblSalesOrderItems orderItem = new tblSalesOrderItems()
            {
                ItemRecordID = maxRecordId + 1,
                SalesOrderNo = so,
                ItemNo = (maxItemNo??0) + 1
            };

            _salesDB.SalesOrderItems.Add(orderItem);

            try
            {
                _salesDB.SaveChanges();
            }
            catch
            {
                
            }

            return RedirectToAction("Edit", "Sales", new { so = so });
        }

        public ActionResult Edit(int? so)
        {
            if (Session["UserId"] != null)
            {
                var query = from salesOrders in _salesDB.SalesOrders
                            join salesCustomer in _salesDB.CustomerContacts
                                    on salesOrders.CustomerContact equals salesCustomer.ContactID into salescustomergroup
                            from customer in salescustomergroup.DefaultIfEmpty()
                            join billingCustomer in _salesDB.Customers
                                    on salesOrders.BillCustomerNo equals billingCustomer.CustomerNo into billingcustomergroup
                            from billing in billingcustomergroup.DefaultIfEmpty()
                            join salesEmployee in _salesDB.Employees
                                    on salesOrders.Createdby equals salesEmployee.ContactID into salesemployeegroup
                            from employee in salesemployeegroup.DefaultIfEmpty()
                            select new
                            {
                                salesOrders.SalesOrderNo,
                                salesOrders.OrderDate,
                                salesOrders.AccountCode,
                                salesOrders.ReqShipDate,
                                salesOrders.ActShipDate,
                                salesOrders.Createdby,
                                salesOrders.BillCustomerNo,
                                salesOrders.CustomerPO,
                                salesOrders.CustomerID,
                                salesOrders.CustomerContact,
                                salesOrders.SalesOrderTotal,
                                salesOrders.SalesOrderCost,
                                salesOrders.TaxStatus,
                                salesOrders.TaxValue,
                                salesOrders.SOStatusID,
                                salesOrders.InvoiceNo,
                                salesOrders.Billedby,
                                salesOrders.BilledDate,
                                salesOrders.ProjectNo,
                                salesOrders.MiscSalesOrder,
                                customer.ContactName,
                                customer.ContactPhone,
                                customer.ContactFax,
                                billing.CustomerName,
                                billing.BillAddress1,
                                billing.BillAddress2,
                                billing.BillCity,
                                billing.BillState,
                                billing.BillZip,
                                EmployeeName = employee.ContactName
                            };

                int id = so ?? 0;
                var results = query
                              .Where(d => d.SalesOrderNo == id)
                              .Select(d => new SalesOrderDetails
                              {
                                  SalesOrderNo = d.SalesOrderNo,
                                  OrderDate = d.OrderDate,
                                  AccountCode = d.AccountCode,
                                  ReqShipDate = d.ReqShipDate,
                                  ActShipDate = d.ActShipDate,
                                  Createdby = d.Createdby,
                                  CreatedbyName = d.EmployeeName,
                                  BillCustomerNo = d.BillCustomerNo,
                                  CustomerPO = d.CustomerPO,
                                  CustomerID = d.CustomerID,
                                  CustomerContact = d.CustomerContact,
                                  SalesOrderTotal = d.SalesOrderTotal,
                                  SalesOrderCost = d.SalesOrderCost,
                                  TaxStatus = d.TaxStatus,
                                  TaxValue = d.TaxValue,
                                  SOStatusID = d.SOStatusID,
                                  InvoiceNo = d.InvoiceNo,
                                  Billedby = d.Billedby,
                                  BilledDate = d.BilledDate,
                                  ProjectNo = d.ProjectNo,
                                  MiscSalesOrder = d.MiscSalesOrder,
                                  ContactName = d.ContactName,
                                  MainPhone = d.ContactPhone,
                                  MainFax = d.ContactFax,
                                  CustomerName = d.CustomerName,
                                  BillAddress1 = d.BillAddress1,
                                  BillAddress2 = d.BillAddress2,
                                  BillCity = d.BillCity,
                                  BillState = d.BillState,
                                  BillZip = d.BillZip
                              })
                              .FirstOrDefault();

                // Add Sales Order Line Items to results
                if (results != null)
                {
                    var salesItems = _salesDB.SalesOrderItems
                                    .Where(i => i.SalesOrderNo == results.SalesOrderNo)
                                    .Select(s => new SalesItemDetails
                                    {
                                        ItemRecordID = s.ItemRecordID,
                                        SalesOrderNo = s.SalesOrderNo,
                                        ItemNo = s.ItemNo,
                                        Quantity = s.Quantity,
                                        GenDescription = s.GenDescription,
                                        PricePerUnit = s.PricePerUnit,
                                        CustPrice = s.CustPrice,
                                        Extension = s.Extension
                                    })
                                    .ToList();

                    if (salesItems != null)
                    {
                        results.SalesOrderItems = salesItems;

                        salesItems = null;
                    }

                    var poquery = from purchaseOrders in _salesDB.PurchaseOrders
                                  join purchaseVendor in _salesDB.Vendors
                                        on purchaseOrders.VendorNo equals purchaseVendor.VendorNo into vendorgroup
                                  from vendor in vendorgroup.DefaultIfEmpty()
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
                                    vendor.VendorPhone
                                };

                    var poresults = poquery
                              .Where(d => d.SalesOrderNo == id)
                              .Select(d => new PurchaseOrderSummary
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
                                  POStatus = d.POStatus,
                                  VendorName = d.VendorName,
                                  VendorAddress1 = d.VendorAddress1,
                                  VendorAddress2 = d.VendorAddress2,
                                  VendorCity = d.VendorCity,
                                  VendorState = d.VendorState,
                                  VendorZip = d.VendorZip,
                                  VendorPhone = d.VendorPhone
                              })
                              .ToList();

                    if (poresults != null)
                    {
                        foreach (var po in poresults)
                        {
                            //if (po.POStatus != null)
                            //{
                            //    po.OrderStatus.Replace("1", "Open");
                            //    po.OrderStatus.Replace("2", "Closed");
                            //    po.OrderStatus.Replace("3", "Backorder");
                            //}

                            if(po.POStatus == 1)
                            {
                                po.OrderStatus = "Open";
                            }
                            else if(po.POStatus == 2)
                            {
                                po.OrderStatus = "Closed";
                            }
                            else if(po.POStatus == 2)
                            {
                                po.OrderStatus = "Backorder";
                            }

                            if (po.POType == 3)
                            {
                                var supplyQuery = from transaction in _salesDB.SupplyTransactions                                                  
                                                  select new
                                                  {                                                      
                                                      transaction.SupplyOrderID,
                                                      transaction.Extension
                                                  };

                                var supplyItems = supplyQuery
                                                .Where(i => i.SupplyOrderID == po.SupplyOrderID)
                                                .Select(s => new SupplyTransactionDetails
                                                {
                                                    SupplyOrderID = s.SupplyOrderID,
                                                    Extension = s.Extension
                                                })
                                                .ToList();

                                if (supplyItems != null)
                                {
                                    po.SupplyTotal = supplyItems.Sum(s => s.Extension);

                                    supplyItems = null;
                                }
                            }
                            else
                            {
                                var poQuery = from purchase in _salesDB.PurchaseOrderDetails
                                              select new
                                              {
                                                  purchase.PODetailID,
                                                  purchase.PONumber,
                                                  purchase.Extensionq
                                              };

                                var purchaseItems = poQuery
                                                .Where(i => i.PONumber == po.PONumber)
                                                .Select(s => new PurchaseItemDetails
                                                {
                                                    PODetailID = s.PODetailID,
                                                    PONumber = s.PONumber,
                                                    Extensionq = s.Extensionq
                                                })
                                                .ToList();

                                if (purchaseItems != null)
                                {                                    
                                    po.SupplyTotal = purchaseItems.Sum(s => s.Extensionq);

                                    purchaseItems = null;
                                }
                            }
                        }

                        results.PurchaseOrders = poresults;
                        results.PurchaseTotal = poresults.Sum(s => s.SupplyTotal);

                        poresults = null;
                    }

                    ViewBag.SalesStatusList = new SelectList(_salesDB.SalesOrderStatus.ToList(), "SOStatusID", "SOStatus", results.SOStatusID);
                    ViewBag.CustomerList = new SelectList(_salesDB.Customers.OrderBy(o => o.CustomerName).ToList(), "CustomerNo", "CustomerName", results.BillCustomerNo);
                    ViewBag.EmployeeList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName", results.Createdby);
                    if (results.CustomerContact != null)
                    {
                        ViewBag.ContactList = new SelectList(_salesDB.CustomerContacts.Where(c => c.CustomerNo == results.BillCustomerNo).ToList(), "ContactID", "ContactName", results.CustomerContact);
                    }
                    else
                    {
                        ViewBag.ContactList = new SelectList(_salesDB.CustomerContacts.Where(c => c.CustomerNo == results.BillCustomerNo).ToList(), "ContactID", "ContactName");
                    }
                    //ViewBag.AccountingList = new SelectList(_salesDB.Ledger.Where(a => a.GLCode.StartsWith("4") && a.POList == true).ToList(), "GLCode", "GLCode", results.AccountCode);
                    ViewBag.AccountingList = AccountingList();
                    ViewBag.TaxTypeList = TaxTypeList(results.TaxStatus);
                    ViewBag.BilledByList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName", results.Billedby);
                    ViewBag.PricingList = new SelectList(_salesDB.PricingUnits.ToList(), "Unit", "UnitName");
                    ViewBag.SalesTax = _salesDB.TaxIndex.Select(t => t.TaxValue).FirstOrDefault();
                }
                else
                {
                    ViewBag.SalesOrder = so;
                    ViewBag.SalesStatusList = new SelectList(_salesDB.SalesOrderStatus.ToList(), "SOStatusID", "SOStatus");
                    ViewBag.CustomerList = new SelectList(_salesDB.Customers.ToList(), "CustomerNo", "CustomerName");
                    ViewBag.EmployeeList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName");
                    ViewBag.ContactList = new SelectList(_salesDB.CustomerContacts.Where(c => c.CustomerNo == results.BillCustomerNo).ToList(), "ContactID", "ContactName");
                    ViewBag.AccountingList = new SelectList(_salesDB.Ledger.Where(a => a.GLCode.StartsWith("4") && a.POList == true).ToList(), "GLCodeID", "GLCode");
                    ViewBag.TaxTypeList = TaxTypeList(null);
                }

                if (results.SOStatusID == 1)
                {
                    return View(results);
                }
                else
                {
                    return RedirectToAction("Order", "Sales", new { so = so });
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        private IEnumerable<SelectListItem> TaxTypeList(string selected)
        {
            bool non = false;
            bool tax = false;
            if (selected == "Taxable") tax = true;
            else non = true;

            IEnumerable<SelectListItem> TaxTypes = new List<SelectListItem>()
                {
                new SelectListItem () { Text="Non-Taxable", Value="Non-Taxable", Selected = non},
                new SelectListItem() { Text = "Taxable", Value = "Taxable", Selected = tax }
                };

            return TaxTypes;
        }

        private IEnumerable<SelectListItem> AccountingList()
        {
            var accountCodesList = _salesDB.Ledger
                                   .Where(a => a.GLCode.StartsWith("4") && a.POList == true)
                                   .Select(a => new AccountCodeDetails {
                                       Code = a.GLCode,
                                       Description = a.GLCode + " - " + a.Description
                                   })
                                   .ToList();

            var codesList = ((IEnumerable<SelectListItem>)new SelectList(accountCodesList, "Code", "Description")).ToList();

            // Return a new, sorted list
            return new SelectList(codesList.OrderBy(o => o.Value), "Value", "Text"); ;
        }

        public bool UpdateSalesOrder(SalesOrderDetails OrderToUpdate)
        {
            if(OrderToUpdate != null)
            {
                var salesOrder = _salesDB.SalesOrders.Where(s => s.SalesOrderNo == OrderToUpdate.SalesOrderNo).FirstOrDefault();

                if(salesOrder != null && salesOrder.SOStatusID == 1)
                {
                    // Update all values
                    salesOrder.OrderDate = OrderToUpdate.OrderDate;
                    salesOrder.SOStatusID = OrderToUpdate.SOStatusID;
                    salesOrder.BillCustomerNo = OrderToUpdate.BillCustomerNo;
                    salesOrder.Createdby = OrderToUpdate.Createdby;
                    salesOrder.ReqShipDate = OrderToUpdate.ReqShipDate;
                    salesOrder.ActShipDate = OrderToUpdate.ActShipDate;
                    salesOrder.ContractNo = OrderToUpdate.ContractNo;
                    salesOrder.CustomerContact = OrderToUpdate.CustomerContact;
                    salesOrder.CustomerPO = OrderToUpdate.CustomerPO;
                    salesOrder.CustomerID = OrderToUpdate.CustomerID;
                    salesOrder.AccountCode = OrderToUpdate.AccountCode;
                    salesOrder.SalesOrderTotal = OrderToUpdate.SalesOrderTotal;
                    salesOrder.SalesOrderCost = OrderToUpdate.SalesOrderCost;
                    salesOrder.TaxStatus = OrderToUpdate.TaxStatus;
                    salesOrder.TaxValue = OrderToUpdate.TaxValue;
                    salesOrder.Billedby = OrderToUpdate.Billedby;
                    salesOrder.BilledDate = OrderToUpdate.BilledDate;
                    salesOrder.InvoiceNo = OrderToUpdate.InvoiceNo;
                    salesOrder.ProjectNo = OrderToUpdate.ProjectNo;
                    salesOrder.MiscSalesOrder = OrderToUpdate.MiscSalesOrder;

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

        public bool UpdateSalesOrderTotal(SalesOrderDetails OrderToUpdate)
        {
            var salesItems = _salesDB.SalesOrderItems
                                    .Where(i => i.SalesOrderNo == OrderToUpdate.SalesOrderNo)
                                    .Select(s => new SalesItemDetails
                                    {
                                        ItemRecordID = s.ItemRecordID,
                                        SalesOrderNo = s.SalesOrderNo,
                                        ItemNo = s.ItemNo,
                                        Quantity = s.Quantity,
                                        GenDescription = s.GenDescription,
                                        PricePerUnit = s.PricePerUnit,
                                        CustPrice = s.CustPrice,
                                        Extension = s.Extension
                                    })
                                    .ToList();

            if (salesItems != null)
            {
                var subTotal = salesItems.Sum(s => s.Extension);

                if (OrderToUpdate.TaxStatus == "Taxable")
                {
                    decimal? taxIndex = (decimal?)_salesDB.TaxIndex.Select(t => t.TaxValue).FirstOrDefault();
                    OrderToUpdate.TaxValue = subTotal * taxIndex;
                    OrderToUpdate.SalesOrderTotal = subTotal + OrderToUpdate.TaxValue;
                }
                else
                {
                    OrderToUpdate.TaxValue = null;
                    OrderToUpdate.SalesOrderTotal = subTotal;
                }                 

                salesItems = null;
            }

            UpdateSalesOrder(OrderToUpdate);

            return true;
        }

        public bool UpdateSalesOrderItem(SalesItemDetails ItemToUpdate)
        {
            if(ItemToUpdate != null)
            {
                var salesItem = _salesDB.SalesOrderItems.Where(s => s.ItemRecordID == ItemToUpdate.ItemRecordID).FirstOrDefault();

                if (salesItem != null)
                {
                    salesItem.SalesOrderNo = ItemToUpdate.SalesOrderNo;
                    salesItem.ItemNo = ItemToUpdate.ItemNo;
                    salesItem.Quantity = ItemToUpdate.Quantity;
                    salesItem.GenDescription = ItemToUpdate.GenDescription;
                    salesItem.PricePerUnit = ItemToUpdate.PricePerUnit;
                    salesItem.CustPrice = ItemToUpdate.CustPrice;
                    salesItem.Extension = ItemToUpdate.Extension;

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

        public JsonResult CutomerCode(int? cust)
        {
            if(cust != null)
            {
                return Json(_salesDB.Customers.Where(c => c.CustomerNo == cust).Select(c => c.CustomerID).FirstOrDefault(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public virtual JsonResult FilteredCustomerContacts(int? cust)
        {
            var custList = _salesDB.CustomerContacts
                .Where(c => c.CustomerNo == cust)
                .Select(x => new SelectListItem { Value = x.ContactID.ToString(), Text = x.ContactName })
                .ToList();

            // Convert list object to Json object
            //string jsonList = new JavaScriptSerializer().Serialize(DayList);
            return Json(custList, JsonRequestBehavior.AllowGet);
        }
    }
}