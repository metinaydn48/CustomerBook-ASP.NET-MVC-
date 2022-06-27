using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CustomerBook.Models;



namespace CustomerBook.Controllers
{
     public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Admin adminModal = new Admin();
            adminModal = (Admin)Session["LoggedUser"];

            if (adminModal != null && Convert.ToUInt16(adminModal.Role) == 2)
                return RedirectToAction("Customers");
            else if (adminModal != null && Convert.ToUInt16(adminModal.Role) == 1)
                return View();
            else
                return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            if (Session["LoggedUser"] == null)
                return View();
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            string hashPass = ComputeSha256Hash(password);
            CustomerBookEntities db = new CustomerBookEntities();

            var admin = (from i in db.Admin
                         where i.Email.Equals(email) && i.PassHash.Equals(hashPass)
                         select i).SingleOrDefault();

            if (admin == null)
                return RedirectToAction("Login");
            else
            {
                Session.Add("LoggedUser", admin);
                return RedirectToAction("Index");
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
        public ActionResult Customers()
        {
            if (Session["LoggedUser"] != null)
            {
                CustomerBookEntities db = new CustomerBookEntities();

                var Customers = db.Customers.ToList();
                return View(Customers);
            }
            return RedirectToAction("Login");
        }

        public ActionResult CustomerAdd()
        {
            if (Session["LoggedUser"] == null)
                return RedirectToAction("Login");
            else
            {
                Admin adminModal = new Admin();
                adminModal = (Admin)Session["LoggedUser"];
                if (Convert.ToUInt16(adminModal.Role) == 1)
                    return View();
                else
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult CustomerAdd(CustomerModel customerModel)
        {
            string fileName = Server.MapPath("~/Uploads/img/") + customerModel.ImageFile.FileName;

            if (customerModel.ImageFile == null || customerModel.ImageFile.ContentLength <= 0)
            {
                ViewBag.ErrorMessage = "Dosya yok ya da geçersiz.";
            }
            else
            {
                customerModel.ImageFile.SaveAs(fileName);
            }

            using (var context = new CustomerBookEntities())
            {
                Customers customerModal = new Customers() 
                {
                    Name = customerModel.Name,
                    Surname = customerModel.Surname,
                    Birthday = Convert.ToDateTime(customerModel.Birthday),
                    Email = customerModel.Email,
                    ImagePath = fileName,
                    ImageFile = System.IO.File.ReadAllBytes(fileName)
                };
                CustomerContact customerContactModal = new CustomerContact() 
                {
                    Adress = customerModel.Adress,
                    Email = customerModel.Email,
                    Phone = customerModel.Phone
                };
                CustomerBusiness customerBusinessModal = new CustomerBusiness()
                {
                    Company = customerModel.Company,
                    Email = customerModel.Email,
                    Job = customerModel.Job
                };
                context.Customers.Add(customerModal);
                context.CustomerContact.Add(customerContactModal);
                context.CustomerBusiness.Add(customerBusinessModal);
                context.SaveChanges();
            }

            #region Email
            //SmtpClient client = new SmtpClient("Mail.nisantasi.edu.tr");
            //MailMessage msg = new MailMessage("info@nisantasi.edu.tr", "mtnaydn48@gmail.com");
            //msg.Subject = "Yeni Kayıt";
            //msg.IsBodyHtml = true;
            //msg.Body = "<b>Yeni müşteri kaydı yapıldı.</b>";

            //NetworkCredential userInfo = new NetworkCredential("info@nisantasi.edu.tr", "123456");
            //client.Credentials = userInfo;

            //try
            //{
            //    client.Send(msg);
            //}
            //catch
            //{
            //    throw new Exception("Mail gönderilirken bir hata oluştu");
            //}

            #endregion

            return View();
        }

        private string ComputeSha256Hash(string passStr)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(passStr));

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}