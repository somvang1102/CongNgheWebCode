﻿using System;
using System.Diagnostics;
using BanVeDiTourDuLich.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace BanVeDiTourDuLich.Controllers
{
    public class AdminController : Controller
    {
        private DataContext _context;
        public AdminController()
        {
            _context = new DataContext();
        }
        // GET: Admin
        public ActionResult Index()
        {
            if (CheckUser())
            {
                return View("Index");
            }
            return HttpNotFound("Hãy Đăng Nhập");
        }

        public ActionResult QuanLyBanVe(string id)
        {
            if (CheckUser())
            {
                if (id != null)
                {
                    ThongTinHoaDon thongTin = new ThongTinHoaDon();
                    thongTin.HoaDon = _context.Ves.Find(id).HoaDon;
                    thongTin.CacVe = thongTin.HoaDon.Ves.ToList();
                    thongTin.KhachHang = thongTin.HoaDon.KhachHang;
                    thongTin.NhanVien = thongTin.HoaDon.NhanVien;
                    return View("~/Views/Admin/ChiTietVe.cshtml", thongTin);
                }
                QuanLyVeViewModel quanLyVeViewModel = new QuanLyVeViewModel();
                quanLyVeViewModel.DanhSachThongTinVe = _context.Ves.Join(_context.Tours, ve => ve.MaTour, tour => tour.MaTour,
                    (ve, tour) =>
                        new
                        {
                            Ve = ve,
                            DiaDiemDen = tour.DiaDiemDen,
                            DiaDiemDi = tour.DiaDiemDi
                        }).Join(_context.LoaiVes, c => c.Ve.MaLoaiVe, loaiVe => loaiVe.MaLoaiVe,
                    (c, loaiVe) => new ThongTinVeExpanded()
                    {
                        Ve = c.Ve,
                        GiaTien = loaiVe.GiaTien,
                        DiaDiemDen = c.DiaDiemDen,
                        DiaDiemDi = c.DiaDiemDi
                    }).ToList();
                return View(quanLyVeViewModel);
            }
            return HttpNotFound("Hãy Đăng Nhập");
        }

        public ActionResult QuanLyNguoiDung()
        {
            QuanLyNguoiDungViewModel data = new QuanLyNguoiDungViewModel();
            foreach(KhachHang khach in _context.KhachHangs.ToList())
            {
                double tongTien = khach.HoaDons.Sum(hoaDon => hoaDon.Ves.Sum(ve => ve.LoaiVe.GiaTien));
                int soVe = khach.HoaDons.Sum(hoaDon => hoaDon.Ves.Count);
                data.ThongTinCacNguoiDung.Add(new NguoiDungViewModel
                {
                    TenNguoiDung = khach.Ten,
                    SoTienMua = tongTien,
                    SoVeMua = soVe
                });
            };
            return View(data);
        }

        public bool CheckUser()
        {
            var userId = Session["MaTaiKhoan"];
            if (userId != null)
            {
                NhanVien nhanVien = _context.NhanViens.Find(userId);
                if (nhanVien == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}