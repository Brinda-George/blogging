using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Functions;
using BlogWebsite.Models;
using System.IO;
using System.Web.Security;
using BlogWebsite.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Net.Mail;

namespace BlogWebsite.Controllers
{
    public class BlogController : Controller
    {
        int a;
        // GET: Blog
        private BlogEntities1 db = new BlogEntities1();
        Class1 obj = new Class1();

        public ActionResult Index()
        {
            var entities = new BlogEntities1();


            var query = from c in entities.BlogTables
                        orderby c.Blogid descending
                        select c;

            return View(query.Take(10).ToList());
        }


        public ActionResult Login() //login page
        {
            return View("LoginView");
        }

        public ActionResult LoginAction(LoginModel lm)
        {
            if (Request.HttpMethod != "POST")
            {
                return View("LoginView");
            }
            else
            {
                //trail_mvc mvc = db.trail_mvc.Find(tc.Name);
                obj.connection("SUYPC086\\SQLEXPRESS", "Blog");
                string query = "select * from Registration where Email='" + lm.Email + "' and Password='" + lm.Password + "'";
                a = obj.query1(query);
                if (a == 0)
                {
                    Response.Write("<script>alert('Login Failed')</script>");
                    //ModelState.AddModelError("", "Invalid login attempt.");
                    return View("LoginView");
                }
                else
                {
                    //FormsAuthentication.SetAuthCookie(tc.Name, false);
                    //return RedirectToAction("index2");
                    Session["Userid"] = a;
                    Session["Username"] = lm.Email;
                    FormsAuthentication.SetAuthCookie(lm.Email, false);
                    return RedirectToAction("User");
                }
            }
        }

        public ActionResult Registration() //Registration page
        {
            return View("RegistrationView");
        }

        public ActionResult Register(RegistrationModel rm)//Register Action
        {
            if (ModelState.IsValid)
            {
                obj.connection("SUYPC086\\SQLEXPRESS", "Blog");
                string query = "insert into Registration(Fname,Lname,Email,Password) values('" + rm.FirstName + "','" + rm.LastName + "','" + rm.Email + "','" + rm.Password + "');SELECT CAST(scope_identity() AS int)";
                string query1 = "select * from Registration where Email='" + rm.Email + "'";
                string email = obj.query2(query1);

                if (email != "")
                {
                    ModelState.AddModelError("", "This Email is already registered.!!!");
                    return View("RegistrationView");
                }
                else
                {
                    //int count = obj.query(query);
                    Session["Username"] = rm.FirstName;
                    int id = obj.query1(query);
                    Session["Userid"] = id;
                    FormsAuthentication.SetAuthCookie(rm.Email, false);
                    return RedirectToAction("User");
                }

            }
            else
            {
                return View("RegistrationView");
            }
        }

        [Authenticationfilter]
        public ActionResult CreateBlog() //Create Blog page
        {
            return View("CreateblogView");
        }

        public ActionResult CreateBlogAction(CreateblogModel cm, HttpPostedFileBase file) //Create Blog Action
        {
            string[] sAllowedExt = new string[] { ".jpg", ".gif", ".png" };

            if (ModelState.IsValid)
            {
                if (file != null && (sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')))))
                {
                    string path = Path.Combine(Server.MapPath("~/images"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.filename = file.FileName;
                    obj.connection("SUYPC086\\SQLEXPRESS", "Blog");
                    string query = "insert into BlogTable(Userid,Titile,Description,Impagepath,Date) values('" + Session["Userid"] + "','" + cm.Title + "','" + cm.Description + "','~/images/" + file.FileName + "','" + DateTime.Now + "')";
                    int count = obj.query(query);
                    return RedirectToAction("User");
                }
                else
                {
                   
                    return View("CreateblogView");
                }
            }
            else
            {
                Response.Write("<script>alert('Please upload a image!!!')</script>");
                return View("CreateblogView");
            }
        }

        [Authenticationfilter]
        public ActionResult User() //User page
        {

            int id = Convert.ToInt32(Session["Userid"]);

            obj.connection("SUYPC086\\SQLEXPRESS", "Blog");
            string query1 = "select Fname from Registration where Userid='" + id + "'";
            var username = obj.query2(query1);
            ViewBag.LoginAuthor = username;


            BlogEntities1 entities = new BlogEntities1();
            var query = from c in entities.BlogTables
                        where c.Userid == id
                        orderby c.Blogid descending
                        select c;
            return View(query.ToList());
        }

        public ActionResult Blog(int? id) //Blog page
        {
            obj.connection("SUYPC086\\SQLEXPRESS", "Blog");
            string query = "select Userid from BlogTable where Blogid=" + id;
            var userid = obj.query1(query);
            string query1 = "select Fname from Registration where Userid='" + userid + "'";
            var username = obj.query2(query1);
            ViewBag.Author = username;
            BlogTable mvc = db.BlogTables.Find(id);
            return View(mvc);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Blog");
        }



        public ActionResult forget()
        {
            return View("forget");
        }

        public ActionResult forgetAction(ForgetModel lm)
        {
            MailMessage mail = new MailMessage();
            string from = "gokulk.mec@gmail.com";
            mail.To.Add(new MailAddress(lm.Email));
            mail.From = new MailAddress(from);
            mail.Subject = "Forget password ";

            obj.connection("SUYPC086\\SQLEXPRESS", "Blog");
            string query1 = "select Password from Registration where Email='" + lm.Email + "'";
            var password = obj.query2(query1);
            if (password != "")
            {
                string Body = "<h3 style='color:black'> " + "UNTOLD TALES !!!!!!.... Hai Your password is: " + password + "<br />" + "<br />" + "Thankyou " + "</h3>";
                mail.Attachments.Add(new Attachment(@"C:\Users\trainee\Downloads\Suyati.jpg"));
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                ("gokulk.mec@gmail.com", "gokulkwynad");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                Response.Write("<script>alert('Mail sent successfully')</script>");
                return View("LoginView");
            }
            else
            {
                Response.Write("<script>alert('Email not registered')</script>");
                return View("forget");
            }

        }

    }
}
