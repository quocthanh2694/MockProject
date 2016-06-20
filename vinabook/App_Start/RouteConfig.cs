using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Vinabook
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
name: "tacgia",
url: "Tac-Gia/{alias}-{matacgia}",
defaults: new { controller = "Sach", action = "SachTheoTacGia", id = UrlParameter.Optional }
);



            routes.MapRoute(
name: "nhaxuatban",
url: "Nha-xuat-ban/{alias}-{manxb}",
defaults: new { controller = "Sach", action = "SachTheoNhaXuatBan", id = UrlParameter.Optional }
);


            routes.MapRoute(
name: "chude",
url: "Chu-De/{alias}-{machude}",
defaults: new { controller = "Sach", action = "SachTheoChuDe", id = UrlParameter.Optional }
);

            routes.MapRoute(
    name: "XemChiTiet",
    url: "XemChiTiet/{alias}-{MaSach}",
    defaults: new { controller = "Sach", action = "XemChiTiet", id = UrlParameter.Optional }
);

            routes.MapRoute(
    name: "Authentication",
    url: "Xac-Thuc-Tai-Khoan-Nguoi-Dung/{id}",
    defaults: new { controller = "NguoiDung", action = "XacThucTaiKhoan", id = UrlParameter.Optional }
);
            routes.MapRoute(
    name: "Info",
    url: "Thong-Tin-Ca-Nhan",
    defaults: new { controller = "NguoiDung", action = "Details", id = UrlParameter.Optional }
);

            routes.MapRoute(
                name: "login",
                url: "dang-nhap-he-thong",
                defaults: new { controller = "Admin", action = "Login" }
            );

            routes.MapRoute(
                name: "register",
                url: "dang-ky-he-thong",
                defaults: new { controller = "NguoiDung", action = "Register" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
