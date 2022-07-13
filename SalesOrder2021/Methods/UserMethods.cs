using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SalesOrder2021.Context;
using SalesOrder2021.Models;

namespace SalesOrder2021.Methods
{
    public static class UserMethods
    {
        public static User ValidateUser(User usr)
        {
            using (SalesContext _salesDB = new SalesContext())
            {
                return _salesDB.Employees
                    .Where(u =>
                        u.LogonName.ToUpper() == usr.UserName.ToUpper()
                        &&
                        u.LogonPass.ToUpper() == usr.Password.ToUpper()
                        &&
                        u.AuthUser == true)
                    .Select(a => new User
                    {
                        UserId = a.ContactID,
                        UserName = a.ContactName,
                        Department = a.AESDepartmentID,
                        Title = a.ContactTitle,
                        Email = a.Email
                    }).FirstOrDefault();
            }
        }

        public static User GetUser(int id)
        {
            using (SalesContext _salesDB = new SalesContext())
            {
                return _salesDB.Employees
                    .Where(u => u.ContactID == id)
                    .Select(a => new User
                    {
                        UserId = a.ContactID,
                        UserName = a.ContactName,
                        Department = a.AESDepartmentID,
                        Title = a.ContactTitle,
                        Email = a.Email
                    }).FirstOrDefault();
            }
        }
    }
}