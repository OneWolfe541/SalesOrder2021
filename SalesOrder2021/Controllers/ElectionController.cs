using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class ElectionController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        // GET: Election
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(int? eoid, int? custid, string custname)
        {
            if (Session["UserId"] != null)
            {
                var query = from electionOrders in _salesDB.ElectionOrders
                            join electionCustomer in _salesDB.Customers
                                    on electionOrders.CustomerNo equals electionCustomer.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
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
                                electionOrders.BOLocked,
                                customer.CustomerName
                            };

                //if (eoid == "") eoid = null;

                DateTime today = DateTime.Now.Date;
                var results = query
                              .Where(d => (d.ElectionOrderNo == eoid || eoid == null) 
                              && (d.CustomerNo == custid || custid == null)
                              && d.ElectionDate > today)
                              .Select(d => new ElectionOrderSearchDetails
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
                                  CustomerName = d.CustomerName
                              })
                              .OrderByDescending(o => o.ElectionDate)
                              .Take(1000)
                              .ToList();

                ViewBag.CustomerList = new SelectList(_salesDB.Customers.ToList(), "CustomerNo", "CustomerName", custid);
                ViewBag.CustomerName = custname;

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Order(int? so, int? eo)
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
                                electionOrders.Imaging,
                                electionOrders.NoScheduleRpt,
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
                              .Where(d => (d.SalesOrderNo == id || d.ElectionOrderNo == eo) && d.ElectionDate != null )
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
                                  Imaging = d.Imaging??false,
                                  NoScheduleRpt = d.NoScheduleRpt??false,
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
                              .ToList();

                // Add Event Calendar Line Items to results
                if (results != null)
                {
                    if(so == null) ViewBag.SalesOrder = results.FirstOrDefault().SalesOrderNo;

                    var calendarQuery = from eCalendar in _salesDB.Calendar
                                        join electionEvents in _salesDB.Events
                                          on eCalendar.EventID equals electionEvents.EventID into eventsgroup
                                        from events in eventsgroup.DefaultIfEmpty()
                                        select new
                                        {
                                            eCalendar.ElectionOrderNo,
                                            eCalendar.EventDate,
                                            eCalendar.EventPerson,
                                            eCalendar.EventLocation,
                                            events.ElectionEvent
                                        };

                    foreach (var election in results)
                    {
                        var electionCalendar = calendarQuery
                                        .Where(i => i.ElectionOrderNo == election.ElectionOrderNo)
                                        .Select(s => new ElectionOrderCalendarDetails
                                        {
                                            ElectionOrderNo = s.ElectionOrderNo,
                                            EventDate = s.EventDate,
                                            ElectionEvent = s.ElectionEvent,
                                            EventPerson = s.EventPerson,
                                            EventLocation = s.EventLocation
                                        })
                                        .ToList();

                        if (electionCalendar != null)
                        {
                            election.ElectionEventsCalendar = electionCalendar;

                            electionCalendar = null;
                        }
                    }
                }
                //else
                //{
                //    ViewBag.SalesOrder = so;
                //}

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public virtual JsonResult FilteredCustomers(string custname)
        {
            var custList = _salesDB.Customers
                .Where(c => c.CustomerName.StartsWith(custname))
                .Select(x => new SelectListItem { Value = x.CustomerNo.ToString(), Text = x.CustomerName })
                .ToList();

            // Convert list object to Json object
            //string jsonList = new JavaScriptSerializer().Serialize(DayList);
            return Json(custList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(int? so)
        {
            if (Session["UserId"] != null)
            {
                ViewBag.SalesOrder = so;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Create(int? so, DateTime date, string title)
        {
            if (Session["UserId"] != null)
            {
                var maxElectionOrderNo = _salesDB.ElectionOrders.Max(i => i.ElectionOrderNo);
                var salesOrder = _salesDB.SalesOrders.Where(s => s.SalesOrderNo == so).FirstOrDefault();

                tblElectionOrder electionOrder = new tblElectionOrder()
                {
                    ElectionOrderNo = maxElectionOrderNo + 1,
                    SalesOrderNo = so,
                    AESContactID = Int32.Parse(Session["UserId"].ToString()),
                    EntryDate = DateTime.Now,
                    CustomerNo = salesOrder.BillCustomerNo,
                    ContactID = salesOrder.CustomerContact,
                    ElectionDate = date,
                    ElectionTitle = title,
                    EngProc = false,
                    SpnProc = false,
                    Photo = false,
                    Imaging = false,
                    NoScheduleRpt = false,
                    BOD = false,
                    BOLocked = false
                };

                _salesDB.ElectionOrders.Add(electionOrder);

                try
                {
                    _salesDB.SaveChanges();
                }
                catch (Exception e)
                {
                    var test = e;
                }

                return RedirectToAction("Edit", "Election", new { eo = electionOrder.ElectionOrderNo });
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Edit(int? eo, string tab)
        {
            if (Session["UserId"] != null)
            {
                var query = from electionOrders in _salesDB.ElectionOrders
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
                                electionOrders.BOD,
                                electionOrders.BODDescription,
                                electionOrders.BODSystems,
                                electionOrders.BODNetwork,
                                electionOrders.BODData,
                                electionOrders.BODSetup,
                                electionOrders.Imaging,
                                electionOrders.NoScheduleRpt
                            };

                var results = query
                              .Where(d => (d.ElectionOrderNo == eo))
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
                                  Imaging = d.Imaging ?? false,
                                  NoScheduleRpt = d.NoScheduleRpt ?? false,
                                  BOD = d.BOD??false,
                                  BODDescription = d.BODDescription,
                                  BODSystems = d.BODSystems,
                                  BODNetwork = d.BODNetwork,
                                  BODData = d.BODData,
                                  BODSetup = d.BODSetup
                              })
                              .FirstOrDefault();

                if (results != null)
                {
                    var electionCalendar = _salesDB.Calendar
                                        .Where(i => i.ElectionOrderNo == results.ElectionOrderNo)
                                        .Select(s => new ElectionOrderCalendarDetails
                                        {
                                            EventRecordID = s.EventRecordID,
                                            ElectionOrderNo = s.ElectionOrderNo,
                                            EventDate = s.EventDate,
                                            EventID = s.EventID,
                                            EventTime = s.EventTime,
                                            EventPerson = s.EventPerson,
                                            EventLocation = s.EventLocation,
                                            ContactID = s.ContactID,
                                            ShipAddressID = s.ShipAddressID,
                                            PublishWSS = s.PublishWSS
                                        })
                                        .ToList();

                    if (electionCalendar != null && electionCalendar.Count() > 0)
                    {
                        results.ElectionEventsCalendar = electionCalendar;

                        electionCalendar = null;
                    }

                    var writeInCandidates = _salesDB.WriteInDetails
                                        .Where(i => i.ElectionOrderNo == results.ElectionOrderNo)
                                        .ToList();

                    if (writeInCandidates != null && writeInCandidates.Count() > 0)
                    {
                        results.WriteInCandidates = writeInCandidates;

                        writeInCandidates = null;
                    }

                    var votingMachines = _salesDB.VotingEquipment
                                        .Where(i => i.ElectionOrderNo == results.ElectionOrderNo)
                                        .ToList();

                    if (votingMachines != null && votingMachines.Count() > 0)
                    {
                        results.VotingEquipment = votingMachines;

                        votingMachines = null;
                    }

                    var specialInstructions = _salesDB.SpecialInstructions
                                        .Where(i => i.ElectionOrderNo == results.ElectionOrderNo)
                                        .ToList();

                    if (specialInstructions != null && specialInstructions.Count() > 0)
                    {
                        results.Instructions = specialInstructions;

                        specialInstructions = null;
                    }

                    var electionContacts = _salesDB.ElectionContacts
                                        .Where(i => i.ElectionOrderNo == results.ElectionOrderNo)
                                        .ToList();

                    if (electionContacts != null && electionContacts.Count() > 0)
                    {
                        results.Contacts = electionContacts;

                        electionContacts = null;
                    }
                }

                ViewBag.EmployeeList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName");
                ViewBag.CustomerList = new SelectList(_salesDB.Customers.ToList(), "CustomerNo", "CustomerName");
                ViewBag.EventsList = new SelectList(_salesDB.Events.ToList(), "EventID", "ElectionEvent");
                ViewBag.ElectionTypes = new SelectList(_salesDB.ElectionTypes.ToList(), "GovTypeID", "GovType");
                ViewBag.VotingMachineList = new SelectList(_salesDB.VotingMachines.ToList(), "VotingMachines", "VotingMachines"); ;
                ViewBag.CustomerAddressList = new SelectList(_salesDB.CustomerAddresses.Where(c => c.CustomerNo == results.CustomerNo).ToList(), "ShipAddressID", "ShipAddress1");
                ViewBag.CustomerContactList = new SelectList(_salesDB.CustomerContacts.Where(c => c.CustomerNo == results.CustomerNo).ToList(), "ContactID", "ContactName");
                ViewBag.BODNetworkingList = NetworkList();
                ViewBag.EquipmentStatusList = new SelectList(_salesDB.EquipmentStatus.ToList(), "EquipStatusID", "EquipmentStatus");

                if(tab == null)
                {
                    ViewBag.CurrentTab = "calendar";
                }
                else
                {
                    ViewBag.CurrentTab = tab;
                }

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        private IEnumerable<SelectListItem> NetworkList()
        {
            IEnumerable<SelectListItem> networking = new List<SelectListItem>()
                {
                new SelectListItem() { Text = "STANDALONE", Value = "STANDALONE" },
                new SelectListItem() { Text = "PEER TO PEER w/HUB", Value = "PEER TO PEER w/HUB" },
                new SelectListItem() { Text = "PEER TO PEER no HUB", Value = "PEER TO PEER no HUB" },
                new SelectListItem() { Text = "WIRELESS", Value = "WIRELESS" },
                new SelectListItem() { Text = "USE CUSTOMER NETWORK", Value = "USE CUSTOMER NETWORK" }
                };

            return networking;
        }

        public bool UpdateElectionOrder(ElectionOrderDetails OrderToUpdate)
        {
            if (OrderToUpdate != null)
            {
                var electionOrder = _salesDB.ElectionOrders.Where(s => s.ElectionOrderNo == OrderToUpdate.ElectionOrderNo).FirstOrDefault();

                if (electionOrder != null)
                {
                    // Update all values
                    electionOrder.EntryDate = OrderToUpdate.EntryDate;
                    electionOrder.AESContactID = OrderToUpdate.AESContactID;
                    electionOrder.CustomerNo = OrderToUpdate.CustomerNo;
                    electionOrder.ContactID = OrderToUpdate.ContactID;
                    electionOrder.CustomerPO = OrderToUpdate.CustomerPO;
                    electionOrder.ElectionDate = OrderToUpdate.ElectionDate;
                    electionOrder.ElectionType = OrderToUpdate.ElectionType;
                    electionOrder.Questions = OrderToUpdate.Questions;
                    electionOrder.Districts = OrderToUpdate.Districts;
                    electionOrder.ElectionTitle = OrderToUpdate.ElectionTitle;
                    electionOrder.EngProc = OrderToUpdate.EngProc;
                    electionOrder.EngProcReceived = OrderToUpdate.EngProcReceived;
                    electionOrder.SpnProc = OrderToUpdate.SpnProc;
                    electionOrder.SpnProcReceived = OrderToUpdate.SpnProcReceived;
                    electionOrder.Photo = OrderToUpdate.Photo;
                    electionOrder.PhotoReceived = OrderToUpdate.PhotoReceived;
                    electionOrder.Imaging = OrderToUpdate.Imaging;
                    electionOrder.NoScheduleRpt = OrderToUpdate.NoScheduleRpt;
                    electionOrder.BOD = OrderToUpdate.BOD;
                    electionOrder.BODDescription = OrderToUpdate.BODDescription;
                    electionOrder.BODSystems = OrderToUpdate.BODSystems;
                    electionOrder.BODNetwork = OrderToUpdate.BODNetwork;
                    electionOrder.BODData = OrderToUpdate.BODData;
                    electionOrder.BODSetup = OrderToUpdate.BODSetup;

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

        public bool UpdateEventItem(tblCalendar ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var eventItem = _salesDB.Calendar.Where(s => s.EventRecordID == ItemToUpdate.EventRecordID).FirstOrDefault();

                if (eventItem != null)
                {                    
                    eventItem.EventID = ItemToUpdate.EventID;
                    eventItem.EventDate = ItemToUpdate.EventDate;
                    eventItem.EventLocation = ItemToUpdate.EventLocation;
                    eventItem.EventPerson = ItemToUpdate.EventPerson;
                    eventItem.ContactID = ItemToUpdate.ContactID;
                    eventItem.ShipAddressID = ItemToUpdate.ShipAddressID;
                    eventItem.PublishWSS = ItemToUpdate.PublishWSS??false;

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

        public bool AddEventItem(tblCalendar ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                tblCalendar eventItem = new tblCalendar();

                if (eventItem != null)
                {
                    eventItem.ElectionOrderNo = ItemToUpdate.ElectionOrderNo;
                    eventItem.EventID = ItemToUpdate.EventID;
                    eventItem.EventDate = ItemToUpdate.EventDate;
                    eventItem.EventLocation = ItemToUpdate.EventLocation;
                    eventItem.EventPerson = ItemToUpdate.EventPerson;
                    eventItem.ContactID = ItemToUpdate.ContactID;
                    eventItem.ShipAddressID = ItemToUpdate.ShipAddressID;
                    eventItem.PublishWSS = ItemToUpdate.PublishWSS ?? false;

                    _salesDB.Calendar.Add(eventItem);

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

        public bool UpdateCandidateItem(tblWriteInDetail ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var writeInItem = _salesDB.WriteInDetails.Where(s => s.WriteinID == ItemToUpdate.WriteinID).FirstOrDefault();

                if (writeInItem != null)
                {
                    writeInItem.WriteInName = ItemToUpdate.WriteInName;
                    writeInItem.WriteInStyle = ItemToUpdate.WriteInStyle;
                    writeInItem.WriteInOffice = ItemToUpdate.WriteInOffice;

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

        public bool AddCandidateItem(tblWriteInDetail ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                tblWriteInDetail writeInItem = new tblWriteInDetail();

                if (writeInItem != null)
                {
                    writeInItem.ElectionOrderNo = ItemToUpdate.ElectionOrderNo;
                    writeInItem.WriteInName = ItemToUpdate.WriteInName;
                    writeInItem.WriteInStyle = ItemToUpdate.WriteInStyle;
                    writeInItem.WriteInOffice = ItemToUpdate.WriteInOffice;

                    _salesDB.WriteInDetails.Add(writeInItem);

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

        public bool UpdateMachineItem(tblVotingEquipment ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var machineItem = _salesDB.VotingEquipment.Where(s => s.VMEquipID == ItemToUpdate.VMEquipID).FirstOrDefault();

                if (machineItem != null)
                {
                    machineItem.VotingMach = ItemToUpdate.VotingMach;
                    machineItem.VotingMachStatus = ItemToUpdate.VotingMachStatus;
                    machineItem.BallotTub = ItemToUpdate.BallotTub;
                    machineItem.BallotTubStatus = ItemToUpdate.BallotTubStatus;
                    machineItem.TransferCase = ItemToUpdate.TransferCase;
                    machineItem.TransferCaseStatus = ItemToUpdate.TransferCaseStatus;
                    machineItem.VotingBooth = ItemToUpdate.VotingBooth;
                    machineItem.VotingBoothStatus = ItemToUpdate.VotingBoothStatus;
                    machineItem.PollingPlace = ItemToUpdate.PollingPlace;
                    machineItem.Absentee = ItemToUpdate.Absentee;
                    machineItem.EarlyVoting = ItemToUpdate.EarlyVoting;

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

        public bool AddMachineItem(tblVotingEquipment ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                tblVotingEquipment machineItem = new tblVotingEquipment();

                if (machineItem != null)
                {
                    machineItem.ElectionOrderNo = ItemToUpdate.ElectionOrderNo;
                    machineItem.VotingMach = ItemToUpdate.VotingMach;
                    machineItem.VotingMachStatus = ItemToUpdate.VotingMachStatus;
                    machineItem.BallotTub = ItemToUpdate.BallotTub;
                    machineItem.BallotTubStatus = ItemToUpdate.BallotTubStatus;
                    machineItem.TransferCase = ItemToUpdate.TransferCase;
                    machineItem.TransferCaseStatus = ItemToUpdate.TransferCaseStatus;
                    machineItem.VotingBooth = ItemToUpdate.VotingBooth;
                    machineItem.VotingBoothStatus = ItemToUpdate.VotingBoothStatus;
                    machineItem.PollingPlace = ItemToUpdate.PollingPlace;
                    machineItem.Absentee = ItemToUpdate.Absentee;
                    machineItem.EarlyVoting = ItemToUpdate.EarlyVoting;

                    _salesDB.VotingEquipment.Add(machineItem);

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

        public bool UpdateSpecialInstructions(tblInstructions ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var instructionItem = _salesDB.SpecialInstructions.Where(s => s.InstructionID == ItemToUpdate.InstructionID).FirstOrDefault();

                if (instructionItem != null)
                {
                    instructionItem.ElectionInstructions = ItemToUpdate.ElectionInstructions;

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

        public bool AddSpecialInstructions(tblInstructions ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                tblInstructions instructionItem = new tblInstructions();

                if (instructionItem != null)
                {
                    instructionItem.ElectionOrderNo = ItemToUpdate.ElectionOrderNo;
                    instructionItem.ElectionInstructions = ItemToUpdate.ElectionInstructions;

                    _salesDB.SpecialInstructions.Add(instructionItem);

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

        public bool UpdateContacts(tblElectionContacts ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var contactItem = _salesDB.ElectionContacts.Where(s => s.ElectionContactID == ItemToUpdate.ElectionContactID).FirstOrDefault();

                if (contactItem != null)
                {
                    contactItem.Request = ItemToUpdate.Request;
                    contactItem.ContactID = ItemToUpdate.ContactID;
                    contactItem.ShipAddressID = ItemToUpdate.ShipAddressID;

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

        public bool AddContacts(tblElectionContacts ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                tblElectionContacts contactItem = new tblElectionContacts();

                if (contactItem != null)
                {
                    contactItem.ElectionOrderNo = ItemToUpdate.ElectionOrderNo;
                    contactItem.Request = ItemToUpdate.Request;
                    contactItem.ContactID = ItemToUpdate.ContactID;
                    contactItem.ShipAddressID = ItemToUpdate.ShipAddressID;

                    _salesDB.ElectionContacts.Add(contactItem);

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