using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vinabook.Models;
using System.Web.Security;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Vinabook.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            //Username: myvinabook@gmail.com 
            // Password: vinabook123
            //
            return View();
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            string sTaiKhoan = f["username"].ToString();
            string sMatKhau = f.Get("password").ToString();
            string urlString = f.Get("urlString").ToString();
            var usr = (from u in db.KhachHangs
                       where u.TaiKhoan == sTaiKhoan && u.MatKhau == sMatKhau
                       select u).FirstOrDefault();
            //TempData["UserName"] = usr.TaiKhoan;
            if (usr != null)
            {
                if (usr.IsActive == "0")
                {
                    ViewBag.ThongBao = "Tài khoản chưa được kích hoạt. Vui lòng kích hoạt qua email đã đăng ký!!!";
                    return View();
                }

                //create seession/ token for loged in user
                // FormsAuthentication.SetAuthCookie(usr.TaiKhoan, false);
                Session["TaiKhoan"] = usr;
                //lay gio hang cua khach hang 
                if (urlString.Trim() != "")
                {
                    string[] url = urlString.Split('/');
                    if (url[url.Length - 1] == "Login")
                        return RedirectToAction("Index", "Home");
                    else
                        return Redirect(urlString);
                }
                else
                    return RedirectToAction("Index", "Home");
            }

            TempData["Message"] = "Username or password is wrong";
            ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng!";

            return View();
            //KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.TaiKhoan == sTaiKhoan && n.MatKhau == sMatKhau);
            //if (kh != null)
            //{
            //    //<script type="text/javascript"> alert('Xss done');</script>

            //    ViewBag.ThongBao = "Chúc mừng bạn đăng nhập thành công !";
            //    Session["TaiKhoan"] = kh;
            //    return RedirectToAction("Index","Home");
            //}
            //ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng!";
            //return View();

        }

        [HttpGet]
        public ActionResult Register()
        {

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Register([Bind(Include = "MaKH,HoTen, NgaySinh, GioiTinh, DienThoai,TaiKhoan, MatKhau,Email,DiaChi")]KhachHang kh)
        {
            string[] full = Request.Url.ToString().Split('/');
            //full[2] domain

            if (ModelState.IsValid)
            {
                var checkUserName = db.KhachHangs.Any(s => s.TaiKhoan == kh.TaiKhoan);
                if (checkUserName)
                {
                    ViewBag.thongbao = "Tài khoản đã tồn tại.";
                }
                else
                {
                    kh.IsActive = "0";
                    //Guid guidName = new Guid();
                    Guid guidName = Guid.NewGuid();
                    Guid guidName2 = Guid.NewGuid();
                    Guid guidName1 = Guid.NewGuid();
                 

                    kh.code = guidName.ToString() + guidName1.ToString() + guidName2.ToString();
                    //Chèn dữ liệu vào bảng khách hàng
                    db.KhachHangs.Add(kh);

                    //Lưu vào csdl 
                    db.SaveChanges();
                    SendMail(kh.Email, kh.code, full[2]);
                    ViewBag.thanhcong = "Đăng ký tài khoản thành công!";

                }
            }
            return View();
            return RedirectToAction("XacThucTaiKhoan");
        }
        void SendMail(string email, string code, string domain)
        {
            var body = "<p>Email From: {0} ({1})</p><p> </p><p>{2}</p><p><a href=\"http://{3}/Xac-Thuc-Tai-Khoan-Nguoi-Dung/{4}\"> Vui lòng nhấn vào đây để xác nhận</a></p>";
            string content = "Cảm  ơn bạn đã sử dụng hệ thống Vinabook.";
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
                                                     // message.To.Add(new MailAddress("mr.vothanhthien@gmail.com"));
            message.From = new MailAddress("myvinabook@gmail.com");  // replace with valid value
            message.Subject = "Authentication for Vinabook";
            message.Body = string.Format(body, "Vinabook", "Send to: " + email, content, domain,code);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "myvinabook@gmail.com",  // replace with valid value
                    Password = "vinabook123"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(message);
                //   return RedirectToAction("Sent");
            }
        }
        //return View(model);

        //cap nhat thong tin khách hang
        public ActionResult Details(int? id)
        {
            KhachHang khachhang = db.KhachHangs.Find(id);
            ViewBag.GioiTinh = khachhang.GioiTinh;
            return View(khachhang);
        }
        [HttpPost]

        public ActionResult Details(KhachHang khachhang)
        {
            try
            {
                ViewBag.GioiTinh = khachhang.GioiTinh;
                if (ModelState.IsValid)
                {
                    foreach (var item in db.KhachHangs.ToList())
                    {
                        if (item.TaiKhoan == khachhang.TaiKhoan)
                        {
                            item.GioiTinh = khachhang.GioiTinh;
                            item.HoTen = khachhang.HoTen;
                            item.MatKhau = khachhang.MatKhau;
                            item.NgaySinh = khachhang.NgaySinh;
                            item.DiaChi = khachhang.DiaChi;
                            item.DienThoai = khachhang.DienThoai;
                            item.Email = khachhang.Email;
                            db.SaveChanges();
                            break;
                        }
                    }
                    //db.Entry(khachhang).State = EntityState.Modified;
                    // KhachHang kh = db.KhachHangs.SingleOrDefault(s => s.TaiKhoan == khachhang.TaiKhoan);

                    ViewBag.thanhcong = "Cập nhật thành công!";
                    //return RedirectToAction("Index");

                }
            }
            catch { }
            return View(khachhang);
        }
        public ActionResult Logout(string urlString)
        {
            //FormsAuthentication.SignOut();
            Session["TaiKhoan"] = null;
            if (urlString.Trim() != "")
                return Redirect(urlString);
            return RedirectToAction("Index", "Home");
        }
        //[HttpPost]
        //public ActionResult KiemTraTK(string inputName)
        //{
        //    //var list = (from s in db.KhachHangs
        //    //           select s).ToList();
        //    //foreach (var item in list)
        //    //{
        //    //    if(name==item.TaiKhoan)
        //    //    {
        //    //        return Json(new { tb = 1 });
        //    //    }
        //    //}
        //    return Json(new { tb = 0 });
        //}

        public ActionResult XacThucTaiKhoan(string id)
        {
            if (id !=null)
            {
                List<KhachHang> listUser = db.KhachHangs.ToList();
                foreach (KhachHang k in listUser)
                {
                    if (k.code == id)
                    {
                        Session["TaiKhoan"] = k;
                        k.IsActive = "1";
                        db.SaveChanges();
                       return View();
                    }
                }
                
            }
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index","Home");
        }
        public ActionResult QuenMatKhau(string email)
        {
            int flag = 0;
            string matkhau = "";
            matkhau = RandomString(10);

            List<KhachHang> listUser = db.KhachHangs.ToList();
            foreach (KhachHang k in listUser)
            {
                if (k.Email == email)
                {
                    flag = 1;
                    k.MatKhau = matkhau;
                    db.SaveChanges();
                    break;
                }
            }

            if (flag == 0)
            {
                return Json(new { done=0 });
            }


            var body = "<p>Email From: {0} ({1})</p><p> </p><p>{2}</p><p>Mật khẩu mới của bạn là: {3}</p>";
            string content = "Cảm  ơn bạn đã sử dụng hệ thống Vinabook.";
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
                                                     // message.To.Add(new MailAddress("mr.vothanhthien@gmail.com"));
            message.From = new MailAddress("myvinabook@gmail.com");  // replace with valid value
            message.Subject = "Reset password for account Vinabook";
            message.Body = string.Format(body, "Vinabook", "Send to: " + email, content, matkhau);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "myvinabook@gmail.com",  // replace with valid value
                    Password = "vinabook123"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(message);
                //   return RedirectToAction("Sent");
            }

            return Json(new { done=1 });
        }
        private string RandomString(int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

    }
}