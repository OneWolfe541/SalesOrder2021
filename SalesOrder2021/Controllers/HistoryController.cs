using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class HistoryController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        // GET: History
        public ActionResult Index()
        {
            //DirectoryInfo vidDir = null;
            //FileInfo[] files = null;

            //string dirPath = @"/PDFCentral/Ballots/5005/";
            //vidDir = new DirectoryInfo(Server.MapPath(dirPath));
            //files = vidDir.GetFiles();

            //string server = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            //var filesList = DirSearch(Server.MapPath(dirPath), server);

            return RedirectToAction("Search");
        }

        public ActionResult Search(string custid, string custname)
        {
            if (Session["UserId"] != null)
            {
                var query = from salesOrders in _salesDB.SalesOrders
                            join salesItems in _salesDB.SalesOrderItems
                                    on salesOrders.SalesOrderNo equals salesItems.SalesOrderNo into salesitemsgroup
                            from items in salesitemsgroup.DefaultIfEmpty()
                            join elections in _salesDB.ElectionOrders
                                    on salesOrders.SalesOrderNo equals elections.SalesOrderNo
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
                                status.SOStatus,
                                elections.ElectionOrderNo,
                                elections.ElectionTitle,
                                elections.ElectionDate
                            };
                
                if (custid == "") custid = null;

                var searchResults = query
                              .Where(d => d.SOStatusID == 3 && (d.CustomerID == custid || custid == null) && d.ItemNo == 1
                              && d.CustomerID != null && d.GenDescription != null && d.ElectionDate != null)                              
                              .Select(d => new SalesOrderSearchDetails
                              {
                                  SalesOrderNo = d.SalesOrderNo,
                                  OrderDate = d.OrderDate,
                                  CustomerID = d.CustomerID,
                                  CustLookupIndex = d.CustLookupIndex,
                                  CustomerName = d.CustomerName,
                                  ItemRecordID = d.ItemRecordID,
                                  ItemNo = d.ItemNo,
                                  GenDescription = d.ElectionTitle,
                                  SOStatusID = d.SOStatusID,
                                  SOStatus = d.SOStatus,
                                  ElectionOrderNo = d.ElectionOrderNo
                              })
                              .OrderByDescending(o => o.OrderDate)
                              .Take(1000)
                              .ToList();

                ViewBag.CustomerList = new SelectList(_salesDB.Customers.ToList(), "CustomerID", "CustomerName", custid);
                ViewBag.CustomerName = custname;

                return View(searchResults);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Images(int? so, int? eo)
        {
            if (Session["UserId"] != null)
            {
                ViewBag.SalesOrder = so;

                var altContacts = from electionContacts in _salesDB.ElectionContacts
                                  join customerContact in _salesDB.CustomerContacts
                                    on electionContacts.ContactID equals customerContact.ContactID into customercontactgroup
                                  from contact in customercontactgroup.DefaultIfEmpty()
                                  select new
                                  {
                                      electionContacts.ElectionContactID,
                                      electionContacts.ElectionOrderNo,
                                      contact.ContactName,
                                      contact.ContactPhone,
                                      contact.ContactEmail
                                  };

                var query = from electionOrders in _salesDB.ElectionOrders
                            join electionCustomer in _salesDB.Customers
                                    on electionOrders.CustomerNo equals electionCustomer.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join customerContact in _salesDB.CustomerContacts
                                    on electionOrders.ContactID equals customerContact.ContactID into customercontactgroup
                            from contact in customercontactgroup.DefaultIfEmpty()
                            join votingEquipment in _salesDB.VotingEquipment
                                    on electionOrders.ElectionOrderNo equals votingEquipment.ElectionOrderNo into equipmentgroup
                            from equipment in equipmentgroup.DefaultIfEmpty()
                            join electionType in _salesDB.ElectionTypes
                                    on electionOrders.ElectionType equals electionType.GovTypeID into typegroup
                            from type in typegroup.DefaultIfEmpty()
                            join alternateContact in altContacts
                                    on electionOrders.ElectionOrderNo equals alternateContact.ElectionOrderNo into alternatecontactgroup
                            from altcontact in alternatecontactgroup.DefaultIfEmpty()
                            join specialInstructions in _salesDB.SpecialInstructions
                                    on electionOrders.ElectionOrderNo equals specialInstructions.ElectionOrderNo into instructionsgroup
                            from instructions in instructionsgroup.DefaultIfEmpty()
                            select new
                            {
                                electionOrders.ElectionOrderNo,
                                electionOrders.SalesOrderNo,
                                electionOrders.AESContactID,
                                electionOrders.EntryDate,
                                electionOrders.CustomerNo,
                                electionOrders.ContactID,
                                electionOrders.CustomerPO,
                                electionOrders.ElectionType,
                                electionOrders.ElectionTitle,
                                electionOrders.ElectionDate,
                                electionOrders.Questions,
                                electionOrders.Districts,
                                electionOrders.EngProc,
                                electionOrders.EngProcReceived,
                                electionOrders.SpnProc,
                                electionOrders.SpnProcReceived,
                                electionOrders.Photo,
                                electionOrders.PhotoReceived,
                                customer.CustomerName,
                                type.GovType,
                                customer.BillAddress1,
                                customer.BillAddress2,
                                customer.BillCity,
                                customer.BillState,
                                customer.BillZip,
                                customer.MainPhone,
                                customer.MainExtension,
                                customer.MainFax,
                                customer.MainEmail,
                                contact.ContactName,
                                contact.ContactFax,
                                contact.ContactEmail,
                                contact.ContactPhone,
                                equipment.PollingPlace,
                                equipment.Absentee,
                                equipment.EarlyVoting,
                                AltContactName = altcontact.ContactName,
                                AltContactPhone = altcontact.ContactPhone,
                                AltContactEmail = altcontact.ContactEmail,
                                instructions.ElectionInstructions
                            };

                int id = so ?? 0;
                var results = query
                              .Where(d => d.SalesOrderNo == id || d.ElectionOrderNo == eo)
                              .Select(d => new ElectionOrderDetails
                              {
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  SalesOrderNo = d.SalesOrderNo,
                                  AESContactID = d.AESContactID,
                                  EntryDate = d.EntryDate,
                                  CustomerNo = d.CustomerNo,
                                  ContactID = d.ContactID,
                                  CustomerPO = d.CustomerPO,
                                  ElectionType = d.ElectionType,
                                  ElectionTitle = d.ElectionTitle,
                                  ElectionDate = d.ElectionDate,
                                  Questions = d.Questions,
                                  Districts = d.Districts,
                                  EngProc = d.EngProc??false,
                                  EngProcReceived = d.EngProcReceived,
                                  SpnProc = d.SpnProc??false,
                                  SpnProcReceived = d.SpnProcReceived,
                                  Photo = d.Photo??false,
                                  PhotoReceived = d.PhotoReceived,
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
                                  ContactName = d.ContactName,
                                  ContactEmail = d.ContactEmail,
                                  ContactPhone = d.ContactPhone,
                                  GovType = d.GovType,
                                  PollingPlace = d.PollingPlace,
                                  Absentee = d.Absentee,
                                  EarlyVoting = d.EarlyVoting,
                                  AltContactName = d.AltContactName,
                                  AltContactEmail = d.AltContactEmail,
                                  AltContactPhone = d.AltContactPhone,
                                  SpecialInstructions = d.ElectionInstructions
                              })
                              .FirstOrDefault();

                if (results != null)
                {
                    if (so == null) ViewBag.SalesOrder = results.SalesOrderNo;

                    string dirPath = @"../PDFCentral/Ballots/" + results.ElectionOrderNo.ToString() + "/";
                    // All elections below 5000 have been moved to the purge folder
                    if(results.ElectionOrderNo < 5000)
                    {
                        dirPath = @"../PDFCentral/Ballots/Purge/" + results.ElectionOrderNo.ToString() + "/";
                    }
                    //results.BallotPath = Server.MapPath(dirPath);
                    string server = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                    //results.BallotPath = server;
                    results.Ballots = DirSearch(Server.MapPath(dirPath), server);
                }

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // https://stackoverflow.com/questions/14305581/method-to-get-all-files-within-folder-and-subfolders-that-will-return-a-list
        private List<BallotFile> DirSearch(string sDir, string server)
        {
            List<BallotFile> files = new List<BallotFile>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    //if (!f.Contains("thumbs.db") && !f.Contains("Thumbs.db"))
                    if(f.Contains(".pdf"))
                    {
                        BallotFile file = new BallotFile();                        
                        file.FileName = Path.GetFileName(f);
                        file.Path = f.Replace("C:\\inetpub\\wwwroot\\salesorder\\", server);
                        files.Add(file);
                    }
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d, server));
                }
            }
            catch (System.Exception excpt)
            {
                //MessageBox.Show(excpt.Message);
            }

            return files;
        }
    }
}