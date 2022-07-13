using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class BallotController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        // GET: Ballot
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

                var query = from ballotOrder in _salesDB.BallotOrders
                            join electionOrder in _salesDB.ElectionOrders
                                    on ballotOrder.ElectionOrderNo equals electionOrder.ElectionOrderNo into electiongroup
                            from election in electiongroup.DefaultIfEmpty()
                            join ballotCustomers in _salesDB.Customers
                                    on election.CustomerNo equals ballotCustomers.CustomerNo into customergroup
                            from customer in customergroup.DefaultIfEmpty()
                            join customerContacts in _salesDB.CustomerContacts
                                    on election.ContactID equals customerContacts.ContactID into contactgroup
                            from contact in contactgroup.DefaultIfEmpty()
                            join electionType in _salesDB.ElectionTypes
                                    on election.ElectionType equals electionType.GovTypeID into eltypegroup
                            from el_type in eltypegroup.DefaultIfEmpty()
                            join orderType in _salesDB.OrderTypes 
                                    on ballotOrder.OrderTypeID equals orderType.OrderTypeID into otypegroup
                            from o_type in otypegroup.DefaultIfEmpty()
                            select new
                            {
                                ballotOrder.BallotOrderNo,
                                ballotOrder.ElectionOrderNo,
                                ballotOrder.AESContactID,
                                ballotOrder.EntryDate,
                                ballotOrder.OrderTypeID,
                                ballotOrder.BallotOrderName,
                                ballotOrder.BOInstructions,
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
                              .Select(d => new BallotOrderDetails
                              {
                                  BallotOrderNo = d.BallotOrderNo,
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  AESContactID = d.AESContactID,
                                  EntryDate = d.EntryDate,
                                  OrderTypeID = d.OrderTypeID,
                                  BallotOrderName = d.BallotOrderName,
                                  BOInstructions = d.BOInstructions,
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

                // Add Ballot Order Line Items to results
                if (results != null)
                {
                    foreach (var ballot in results)
                    {

                        var stylesQuery = from orderDetails in _salesDB.BallotOrderDetails
                                          join votingType in _salesDB.VotingTypes
                                                on orderDetails.VotingTypeID equals votingType.VotingTypeID into votingtypesgroup
                                          from v_type in votingtypesgroup.DefaultIfEmpty()
                                          join ballotSizes in _salesDB.BallotSizes
                                                on orderDetails.BallotSize equals ballotSizes.OpSizeID into sizesgroup
                                          from sizes in sizesgroup.DefaultIfEmpty()
                                          join ballotStyles in _salesDB.BallotStyles
                                                on orderDetails.BallotStyleID equals ballotStyles.BallotStyleID into stylesgroup
                                          from styles in stylesgroup.DefaultIfEmpty()
                                          join ballotType in _salesDB.BallotTypes
                                                on orderDetails.BallotTypeID equals ballotType.BallotTypeID into ballottypesgroup
                                          from b_type in ballottypesgroup.DefaultIfEmpty()
                                          select new
                                          {
                                              orderDetails.BallotDetailID,
                                              orderDetails.BallotOrderNo,
                                              orderDetails.BallotStyleID,
                                              orderDetails.BallotIndex,
                                              orderDetails.VotingTypeID,
                                              orderDetails.BallotTypeID,
                                              orderDetails.Quantity,
                                              orderDetails.StartNo,
                                              orderDetails.EndNo,
                                              orderDetails.LeftHeaderCode,
                                              orderDetails.RightHeaderCode,
                                              orderDetails.PDFPage,
                                              orderDetails.ItemStatusID,
                                              b_type.BallotTypes,
                                              styles.StyleIndex,
                                              styles.StyleName1,
                                              styles.StyleName2,
                                              styles.AddColors,
                                              styles.StyleColor,
                                              styles.TwoSides,
                                              sizes.ColWidth,
                                              sizes.SizeDesc,
                                              sizes.Width,
                                              sizes.length,
                                              v_type.VotingType
                                          };

                        var ballotItems = stylesQuery.ToList()
                                        .Where(i => i.BallotOrderNo == ballot.BallotOrderNo)
                                        .Select(s => new BallotStyleDetails
                                        {
                                            BallotDetailID = s.BallotDetailID,
                                            BallotOrderNo = s.BallotOrderNo,
                                            BallotIndex = s.BallotIndex,
                                            BallotStyleID = s.BallotStyleID,
                                            VotingTypeID = s.VotingTypeID,
                                            BallotTypeID = s.BallotTypeID,
                                            Quantity = s.Quantity,
                                            LeftHeaderCode = s.LeftHeaderCode,
                                            RightHeaderCode = s.RightHeaderCode,
                                            StartNo = s.StartNo,
                                            EndNo = s.EndNo,
                                            PDFPage = s.PDFPage,
                                            ItemStatusID = s.ItemStatusID,
                                            BallotTypes = s.Width.ToString().TrimEnd('0').TrimEnd('.') + "x" + s.length.ToString().TrimEnd('0').TrimEnd('.') + " " + s.VotingType + " " + s.BallotTypes,
                                            StyleIndex = s.StyleIndex,
                                            StyleName1 = s.StyleName1,
                                            StyleName2 = s.StyleName2,
                                            AddColors = s.AddColors,
                                            StyleColor = s.StyleColor,
                                            TwoSides = s.TwoSides,
                                            ColWidth = s.ColWidth,
                                            SizeDesc = s.SizeDesc,
                                            Width = s.Width,
                                            Length = s.length,
                                            VotingType = s.VotingType
                                        })
                                        .ToList();

                        if (ballotItems != null)
                        {
                            ballot.BallotStyles = ballotItems;

                            ballotItems = null;
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
            var existingBallotOrders = _salesDB.BallotOrders.Where(p => p.ElectionOrderNo == eo).ToList();
            var newBallotOrderNo = existingBallotOrders.Count() + 1;

            tblBallotOrder ballotOrder = new tblBallotOrder()
            {
                ElectionOrderNo = eo,
                BallotOrderNo = eo.ToString() + "-" + newBallotOrderNo,
                EntryDate = DateTime.Now,
                AESContactID = Int32.Parse(Session["UserId"].ToString()),
                BOLocked = false
            };

            _salesDB.BallotOrders.Add(ballotOrder);

            try
            {
                _salesDB.SaveChanges();
            }
            catch (Exception e)
            {
                var test = e;
            }
            return RedirectToAction("Edit", "Ballot", new { bo = ballotOrder.BallotOrderNo });
        }

        public ActionResult Edit(string bo)
        {
            if (Session["UserId"] != null)
            {
                var results = _salesDB.BallotOrders
                             .Where(d => d.BallotOrderNo == bo)
                             .Select(d => new BallotOrderDetails
                             {
                                BallotOrderNo = d.BallotOrderNo,
                                ElectionOrderNo = d.ElectionOrderNo,
                                AESContactID = d.AESContactID,
                                EntryDate = d.EntryDate,
                                OrderTypeID = d.OrderTypeID,
                                BallotOrderName = d.BallotOrderName,
                                BOInstructions = d.BOInstructions
                             })
                             .FirstOrDefault();

                if (results != null)
                {
                    results.BallotDistricts = _salesDB.BallotStyles
                                           .Where(d => d.BallotOrderNo == results.BallotOrderNo)
                                           .Select(d => new BallotStyleDistricts
                                           {
                                               BallotOrderNo = d.BallotOrderNo,
                                               BallotStyleID = d.BallotStyleID,
                                               StyleIndex = d.StyleIndex,
                                               StyleName1 = d.StyleName1,
                                               StyleName2 = d.StyleName2,
                                               AddColors = d.AddColors??false,
                                               StyleColor = d.StyleColor,
                                               TwoSides = d.TwoSides??false
                                           })
                                           .ToList();

                    if(results.BallotDistricts != null && results.BallotDistricts.Count() > 0)
                    {
                        foreach(var style in results.BallotDistricts)
                        {
                            style.StyleDetails = _salesDB.BallotOrderDetails
                                                 .Where(d => d.BallotStyleID == style.BallotStyleID)
                                                 .Select(d => new BallotStyleDetails
                                                 {
                                                     BallotDetailID = d.BallotDetailID,
                                                     BallotOrderNo = d.BallotOrderNo,
                                                     BallotIndex = d.BallotIndex,
                                                     BallotStyleID = d.BallotStyleID,
                                                     VotingTypeID = d.VotingTypeID,
                                                     BallotTypeID = d.BallotTypeID,
                                                     Quantity = d.Quantity,
                                                     BallotSize = d.BallotSize,
                                                     LeftHeaderCode = d.LeftHeaderCode,
                                                     RightHeaderCode = d.RightHeaderCode,
                                                     StartNo = d.StartNo,
                                                     EndNo = d.EndNo,
                                                     AddBallot = d.AddBallot,
                                                     PDFPage = d.PDFPage,
                                                     ItemStatusID = d.ItemStatusID
                                                 })
                                                 .ToList();
                        }
                    }
                }

                ViewBag.EmployeeList = new SelectList(_salesDB.Employees.ToList(), "ContactID", "ContactName");
                ViewBag.OrderTypesList = new SelectList(_salesDB.OrderTypes.ToList(), "OrderTypeID", "OrderType");
                ViewBag.VotingTypesList = new SelectList(_salesDB.VotingTypes.ToList(), "VotingTypeID", "VotingType");
                ViewBag.BallotTypesList = new SelectList(_salesDB.BallotTypes.ToList(), "BallotTypeID", "BallotTypes");                
                //ViewBag.BallotSizeList = new SelectList(_salesDB.BallotSizes.ToList(), "OpSizeID", "SizeDesc");
                ViewBag.BallotSizeList = _salesDB.BallotSizes.ToList();

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public bool UpdateBallotOrder(BallotOrderDetails OrderToUpdate)
        {
            if (OrderToUpdate != null)
            {
                var ballotOrder = _salesDB.BallotOrders.Where(s => s.BallotOrderNo == OrderToUpdate.BallotOrderNo).FirstOrDefault();

                if(ballotOrder != null)
                {
                    // Update all values
                    ballotOrder.EntryDate = OrderToUpdate.EntryDate;
                    ballotOrder.AESContactID = OrderToUpdate.AESContactID;
                    ballotOrder.OrderTypeID = OrderToUpdate.OrderTypeID;
                    ballotOrder.BallotOrderName = OrderToUpdate.BallotOrderName;
                    ballotOrder.BOInstructions = OrderToUpdate.BOInstructions;

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

        public bool UpdateStyleItem(tblBallotStyles ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var styleItem = _salesDB.BallotStyles.Where(s => s.BallotStyleID == ItemToUpdate.BallotStyleID).FirstOrDefault();

                if (styleItem != null)
                {
                    styleItem.StyleIndex = ItemToUpdate.StyleIndex;
                    styleItem.StyleName1 = ItemToUpdate.StyleName1;
                    styleItem.StyleName2 = ItemToUpdate.StyleName2;
                    styleItem.AddColors = ItemToUpdate.AddColors??false;
                    styleItem.StyleColor = ItemToUpdate.StyleColor;
                    styleItem.TwoSides = ItemToUpdate.TwoSides??false;

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

        public ActionResult AddStyleItem(string bo)
        {
            if(bo != null)
            {
                var maxStyleIndex = _salesDB.BallotStyles.Where(bs => bs.BallotOrderNo == bo).Max(i => i.StyleIndex);

                tblBallotStyles style = new tblBallotStyles()
                {
                    BallotOrderNo = bo,
                    StyleIndex = maxStyleIndex + 1,
                    AddColors = false,
                    TwoSides = false
                };

                _salesDB.BallotStyles.Add(style);

                try
                {
                    _salesDB.SaveChanges();
                }
                catch (Exception e)
                {
                    var test = e;
                    
                }
            }

            return RedirectToAction("Edit", "Ballot", new { bo = bo });
        }

        public bool UpdateBallotItem(tblBallotDetail ItemToUpdate)
        {
            if (ItemToUpdate != null)
            {
                var ballotItem = _salesDB.BallotOrderDetails.Where(s => s.BallotDetailID == ItemToUpdate.BallotDetailID).FirstOrDefault();

                if (ballotItem != null)
                {
                    ballotItem.BallotIndex = ItemToUpdate.BallotIndex;
                    ballotItem.BallotStyleID = ItemToUpdate.BallotStyleID;
                    ballotItem.Quantity = ItemToUpdate.Quantity;
                    ballotItem.VotingTypeID = ItemToUpdate.VotingTypeID;
                    ballotItem.StartNo = ItemToUpdate.StartNo;
                    ballotItem.EndNo = ItemToUpdate.EndNo;
                    ballotItem.BallotTypeID = ItemToUpdate.BallotTypeID;
                    ballotItem.BallotSize = ItemToUpdate.BallotSize;
                    ballotItem.RightHeaderCode = ItemToUpdate.RightHeaderCode;
                    ballotItem.PDFPage = ItemToUpdate.PDFPage;

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

        public ActionResult AddBallotItem(string bo, int? bs)
        {
            if (bo != null && bs != null)
            {
                var maxBallotIndex = _salesDB.BallotOrderDetails.Where(bd => bd.BallotStyleID == bs).Max(i => i.BallotIndex);

                tblBallotDetail ballot = new tblBallotDetail()
                {
                    BallotOrderNo = bo,
                    BallotStyleID = bs,
                    BallotIndex = maxBallotIndex + 1
                };

                _salesDB.BallotOrderDetails.Add(ballot);

                try
                {
                    _salesDB.SaveChanges();
                }
                catch (Exception e)
                {
                    var test = e;
                }
            }

            return RedirectToAction("Edit", "Ballot", new { bo = bo });
        }

        public virtual JsonResult FilteredBallotWidths(int? id)
        {
            var sizeList = _salesDB.BallotSizes.Where(m => m.BallotTypeID == id).ToList();
            var widthsList = sizeList
                .Select(x => new SelectListItem { Value = x.OpSizeID.ToString(), Text = x.SizeDesc + " - " + x.Width.ToString().TrimEnd('0').TrimEnd('.') + " x " + x.length.ToString().TrimEnd('0').TrimEnd('.') })
                .ToList();

            // Convert list object to Json object
            //string jsonList = new JavaScriptSerializer().Serialize(DayList);
            return Json(widthsList, JsonRequestBehavior.AllowGet);
        }
    }
}