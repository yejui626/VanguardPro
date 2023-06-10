using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanguardPro.Models;

namespace VanguardPro.Controllers
{
    public class LoginController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Index(tb_user objk)
        {
            if (ModelState.IsValid)
            {
                using (db_vanguardproEntities db = new db_vanguardproEntities());
                {
                    var obj = db.tb_user.Where(a => a.u_username.Equals(objk.u_username) && a.u_pwd.Equals(objk.u_pwd)).FirstOrDefault();

                    if (obj != null)
                    {
                        Session["UserID"] = obj.u_id.ToString();
                        Session["UserName"] = obj.u_username.ToString();
                        Session["UserType"] = obj.u_usertype.ToString();

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The Username or Password Incorrect");


                    }
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}