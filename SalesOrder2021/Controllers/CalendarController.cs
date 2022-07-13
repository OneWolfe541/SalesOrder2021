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
    public class CalendarController : Controller
    {
        private SalesContext _salesDB = new SalesContext();

        public ActionResult Index()
        {
            var today = DateTime.Now.Date;
            var newDate = _salesDB.Calendar
                          .Where(d => d.EventDate >= today)
                          .OrderBy(o => o.EventDate)
                          .Select(s => s.EventDate)
                          .FirstOrDefault();
            return RedirectToAction("Daily", new { date = newDate });
        }

        // GET: Calendar
        public ActionResult Monthly(int? mo, int? yr, DateTime date)
        {
            if (Session["UserId"] != null)
            {
                if (date == null) date = DateTime.Now;

                var query = from elections in _salesDB.ElectionOrders
                            join electionCustomers in _salesDB.Customers
                                    on elections.CustomerNo equals electionCustomers.CustomerNo into customergroup
                            from customers in customergroup.DefaultIfEmpty()
                            join electionTypes in _salesDB.ElectionTypes
                                    on elections.ElectionType equals electionTypes.GovTypeID into typesgroup
                            from types in typesgroup.DefaultIfEmpty()
                            select new
                            {
                                elections.ElectionOrderNo,
                                elections.SalesOrderNo,
                                elections.ElectionTitle,
                                elections.ElectionDate,
                                elections.Questions,
                                elections.Districts,
                                elections.BOD,
                                elections.BODData,
                                elections.BODSystems,
                                elections.BODNetwork,
                                elections.BODSetup,
                                elections.Imaging,
                                types.GovType,
                                customers.CustomerName
                            };

                var results = query
                              .Where(d => DbFunctions.TruncateTime(d.ElectionDate.Value).Value.Month == DbFunctions.TruncateTime(date).Value.Month
                              &&
                              DbFunctions.TruncateTime(d.ElectionDate.Value).Value.Year == DbFunctions.TruncateTime(date).Value.Year
                              &&
                              DbFunctions.TruncateTime(d.ElectionDate.Value) >= DbFunctions.TruncateTime(DateTime.Now))
                              .Select(d => new ElectionCalendarDetails
                              {
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  SalesOrderNo = d.SalesOrderNo,
                                  ElectionTitle = d.ElectionTitle,
                                  ElectionDate = d.ElectionDate,
                                  Questions = d.Questions,
                                  Districts = d.Districts,
                                  BOD = d.BOD,
                                  BODData = d.BODData,
                                  BODSetup = d.BODSetup,
                                  BODSystems = d.BODSystems,
                                  Imaging = d.Imaging,
                                  CustomerName = d.CustomerName,
                                  GovType = d.GovType,
                              })
                              .OrderBy(o => o.ElectionDate)
                              .ToList();

                if (results != null)
                {
                    foreach (var election in results)
                    {
                        var eventQuery = from calendar in _salesDB.Calendar
                                    join electionEvents in _salesDB.Events
                                            on calendar.EventID equals electionEvents.EventID into eventsgroup
                                    from events in eventsgroup.DefaultIfEmpty()
                                    select new
                                    {
                                        calendar.EventRecordID,
                                        calendar.EventDate,
                                        calendar.ElectionOrderNo,
                                        calendar.EventID,
                                        events.ElectionEvent
                                    };

                        var eventItems = eventQuery
                                      .Where(d => d.ElectionOrderNo == election.ElectionOrderNo)
                                      .Select(d => new EventCalendarDetails
                                      {
                                          EventRecordID = d.EventRecordID,
                                          EventDate = d.EventDate,
                                          ElectionOrderNo = d.ElectionOrderNo,
                                          EventID = d.EventID,
                                          ElectionEvent = d.ElectionEvent
                                      })
                                      .OrderBy(o => o.EventDate)
                                      .ToList();

                        if (eventItems != null)
                        {
                            election.EventsCalendar = eventItems;

                            eventItems = null;
                        }
                    }
                }

                ViewBag.CurrentDate = date;

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult PreviousMonth(DateTime date)
        {
            return RedirectToAction("Monthly", new { date = date.AddMonths(-1) });
        }

        public ActionResult NextMonth(DateTime date)
        {
            return RedirectToAction("Monthly", new { date = date.AddMonths(1) });
        }

        public ActionResult Daily(DateTime? date)
        {
            if (Session["UserId"] != null)
            {
                // Never show records earlier than today
                if (date < DateTime.Now.Date || date == null) date = DateTime.Now.Date;

                var query = from calendar in _salesDB.Calendar
                            join electionEvents in _salesDB.Events
                                    on calendar.EventID equals electionEvents.EventID into eventsgroup
                            from events in eventsgroup.DefaultIfEmpty()
                            join electionOrders in _salesDB.ElectionOrders
                                    on calendar.ElectionOrderNo equals electionOrders.ElectionOrderNo into electiongroup
                            from elections in electiongroup.DefaultIfEmpty()
                            join electionCustomers in _salesDB.Customers
                                    on elections.CustomerNo equals electionCustomers.CustomerNo into customergroup
                            from customers in customergroup.DefaultIfEmpty()
                            join electionTypes in _salesDB.ElectionTypes
                                    on elections.ElectionType equals electionTypes.GovTypeID into typesgroup
                            from types in typesgroup.DefaultIfEmpty()
                            select new
                            {
                                calendar.EventRecordID,
                                calendar.EventDate,
                                calendar.ElectionOrderNo,
                                calendar.EventID,
                                events.ElectionEvent,
                                elections.SalesOrderNo,
                                elections.ElectionTitle,
                                elections.ElectionDate,
                                elections.Questions,
                                elections.Districts,
                                elections.BOD,
                                elections.BODData,
                                elections.BODSystems,
                                elections.BODNetwork,
                                elections.BODSetup,
                                elections.Imaging,
                                types.GovType,
                                customers.CustomerName
                            };

                //var endDate = date.AddDays(7);
                var results = query
                              .Where(d => DbFunctions.TruncateTime(d.EventDate.Value) == DbFunctions.TruncateTime(date)
                              //&& // Add 7 days for weekly report
                              //DbFunctions.TruncateTime(d.EventDate.Value) <= DbFunctions.TruncateTime(endDate)
                              )
                              .Select(d => new EventCalendarDetails
                              {
                                  EventRecordID = d.EventRecordID,
                                  EventDate = d.EventDate,
                                  ElectionOrderNo = d.ElectionOrderNo,
                                  EventID = d.EventID,
                                  ElectionEvent = d.ElectionEvent,
                                  SalesOrderNo = d.SalesOrderNo,
                                  ElectionTitle = d.ElectionTitle,
                                  ElectionDate = d.ElectionDate,
                                  Questions = d.Questions,
                                  Districts = d.Districts,
                                  BOD = d.BOD,
                                  BODData = d.BODData,
                                  BODSetup = d.BODSetup,
                                  BODSystems = d.BODSystems,
                                  Imaging = d.Imaging,
                                  CustomerName = d.CustomerName,
                                  GovType = d.GovType,
                              })
                              .OrderBy(o => o.EventDate)
                              .ToList();

                ViewBag.CurrentDate = date;

                //if (results.Count == 0) results = null;

                return View(results);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult PreviousDay(DateTime date)
        {
            var newDate = _salesDB.Calendar
                          .Where(d => d.EventDate < date)
                          .OrderByDescending(o => o.EventDate)
                          .Select(s => s.EventDate)
                          .FirstOrDefault();

            if (newDate == null || newDate < DateTime.Now.Date) newDate = date;

            return RedirectToAction("Daily", new { date = newDate });
        }

        public ActionResult NextDay(DateTime date)
        {
            var newDate = _salesDB.Calendar
                          .Where(d => d.EventDate > date)
                          .OrderBy(o => o.EventDate)
                          .Select(s => s.EventDate)
                          .FirstOrDefault();

            return RedirectToAction("Daily", new { date = newDate });
        }
    }
}