using Microsoft.AspNetCore.Mvc;
using MVCData.Models;
using MVCData.Controllers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Reflection.Metadata.Ecma335;
using System.Linq.Expressions;
using System.Text;
using System.Data;
using System.Reflection;
using System.Xml;
using System.ComponentModel;
using System.Globalization;
using System.Data.Common;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization.Formatters;
using System.Runtime.InteropServices;
using System.Diagnostics.Metrics;
using System.Data.SqlTypes;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.VisualBasic;
using System.ComponentModel.Design;
using Dapper;
using System.Net.Security;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dapper.FastCrud;
using Npgsql;
namespace MVCData.Controllers
{
    public class StaffController : Controller
    {
        private readonly DapperContext _context;
        public StaffController(DapperContext context)
        {
            _context = context;
        }
        public IActionResult Index(int phongBanId = 0, int page = 1)
        {
            int pageSize = 10;
            int offset = (page - 1) * pageSize;
            using var conn = _context.CreateConnection();
            var phongBans = conn.Query("SELECT id, ten_phong_ban FROM phong_ban ORDER BY ten_phong_ban").ToList();
            ViewBag.PhongBanSelectList = new SelectList(phongBans,"id","ten_phong_ban",phongBanId);
            string countSql = @"SELECT COUNT(*) FROM nhan_vien WHERE (@PhongBanId = 0 OR phongban_id = @PhongBanId)";
            int totalRecord = conn.ExecuteScalar<int>( countSql,
                new { PhongBanId = phongBanId });
            int totalPage = (int)Math.Ceiling(totalRecord / (double)pageSize);
            string sql = @"
            SELECT 
            nv.id AS Id,
            nv.ho_ten AS HoTen,
            nv.ngay_sinh::timestamp AS NgaySinh,
            nv.so_dien_thoai AS SoDienThoai,
            nv.dia_chi AS DiaChi,
            nv.chuc_vu AS ChucVu,
            nv.so_nam_cong_tac AS SoNamCongTac,
            pb.ten_phong_ban AS TenPhongBan
            FROM nhan_vien nv
            LEFT JOIN phong_ban pb 
            ON nv.phongban_id = pb.id
            WHERE (@PhongBanId = 0 OR nv.phongban_id = @PhongBanId)
            ORDER BY nv.id
            LIMIT @pageSize OFFSET @offset";

            var data = conn.Query<NhanVien>(
                sql,
                new { pageSize, offset, PhongBanId = phongBanId }
            ).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.SelectedPhongBan = phongBanId;

            return View(data);
        }
        [HttpGet]
        public IActionResult LoadPage(int page)
        {
            int pageSize = 10;
            int offset = (page - 1) * pageSize;
            using var conn = _context.CreateConnection();

            var totalRow = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM nhan_vien");
            var sql = @"
        SELECT 
            nv.id AS ID,
            nv.ho_ten AS HoTen,
            nv.ngay_sinh::timestamp AS NgaySinh,
            nv.so_dien_thoai AS SoDienThoai,
            nv.dia_chi AS DiaChi,
            nv.chuc_vu AS ChucVu,
            nv.so_nam_cong_tac AS SoNamCongTac,
            pb.ten_phong_ban AS TenPhongBan
        FROM nhan_vien nv
        LEFT JOIN phong_ban pb 
            ON nv.phongban_id = pb.id
        ORDER BY nv.id
        LIMIT @pageSize OFFSET @offset";

            var list = conn.Query<NhanVien>(
                sql,
                new { pageSize, offset }
            ).ToList();

            ViewBag.TotalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View("Index", list);
        }
        [HttpGet]
        public IActionResult CreatePopup()
        {
            using var conn = _context.CreateConnection();

            ViewBag.PhongBans = conn.Query<PhongBan>(
                "SELECT id, ten_phong_ban AS TenPhongBan FROM phong_ban"
            ).ToList();

            return PartialView("_CreatePopup", new NhanVien());
        }
        [HttpPost]
        public IActionResult CreatePopup(NhanVien nv)
        {
            using var conn = _context.CreateConnection();
            var sql = @"
        INSERT INTO nhan_vien 
        (ho_ten, ngay_sinh, so_dien_thoai, dia_chi, chuc_vu, so_nam_cong_tac, phongban_id)
        VALUES
        (@HoTen, @NgaySinh, @SoDienThoai, @DiaChi, @ChucVu, @SoNamCongTac, @PhongBanId)";
            conn.Execute(sql, nv);
            return Json(new
            {
                success = true,
                message = "Thêm nhân viên thành công"
            });
        }
        [HttpGet]
        public IActionResult EditPopup(int id)
        {
            using var conn = _context.CreateConnection();
            ViewBag.PhongBans = conn.Query<PhongBan>(
                "SELECT id, ten_phong_ban AS TenPhongBan FROM phong_ban"
            ).ToList();
            var sql = @"SELECT 
                id AS ID,
                ho_ten AS HoTen,
                ngay_sinh::timestamp AS NgaySinh,
                so_dien_thoai AS SoDienThoai,
                dia_chi AS DiaChi,
                chuc_vu AS ChucVu,
                so_nam_cong_tac AS SoNamCongTac,
                phongban_id AS PhongBanId
            FROM nhan_vien
            WHERE id = @id";
            var nv = conn.QueryFirstOrDefault<NhanVien>(sql, new { id });

            if (nv == null)
                return Content("Không tìm thấy nhân viên");

            return PartialView("_EditPopup", nv);
        }
        [HttpPost]
        public IActionResult EditPopup(NhanVien nv)
        {
            using var conn = _context.CreateConnection();
            var sql = @"UPDATE nhan_vien SET
        ho_ten = @HoTen,
        ngay_sinh = @NgaySinh,
        so_dien_thoai = @SoDienThoai,
        dia_chi = @DiaChi,
        chuc_vu = @ChucVu,
        so_nam_cong_tac = @SoNamCongTac,
        phongban_id= @PhongBanId
        WHERE id = @Id";
            var row = conn.Execute(sql, nv);

            if (row == 0)
                return Json(new { success = false, message = "Cập nhật thất bại" });

            return Json(new
            {
                success = true,
                message = "Thêm nhân viên thành công"
            });
        }
        [HttpPost]
        public IActionResult Delete(NhanVien nv)
        {
            using var conn = _context.CreateConnection();
            var sql = @"DELETE FROM nhan_vien WHERE id = @id";
            conn.Execute(sql, nv);
            return Json(new
            {
                success = true,
                message = "Thêm nhân viên thành công"
            });
        }
        [HttpGet]
        public IActionResult Search(string keyword)
        {
            using var conn = _context.CreateConnection();

            var sql =
               @"SELECT 
            nv.id AS Id,
            nv.ho_ten AS HoTen,
            nv.ngay_sinh::timestamp AS NgaySinh,
            nv.so_dien_thoai AS SoDienThoai,
            nv.dia_chi AS DiaChi,
            nv.chuc_vu AS ChucVu,
            nv.so_nam_cong_tac AS SoNamCongTac,
            pb.ten_phong_ban AS TenPhongBan
        FROM nhan_vien nv
        LEFT JOIN phong_ban pb 
            ON nv.phongban_id = pb.id
        WHERE
            @kw IS NULL
            OR CAST(nv.id AS TEXT) ILIKE @kw
            OR nv.ho_ten ILIKE @kw
            OR nv.dia_chi ILIKE @kw
        ORDER BY nv.id
    ";
            var data = conn.Query<NhanVien>(
                sql,
                new { kw = string.IsNullOrEmpty(keyword) ? null : "%" + keyword + "%" }
            ).ToList();

            return PartialView("_NhanVienTableBody", data);
        }
        // public IActionResult ExportExcel()
        // {
        //     using var conn = _context.CreateConnection();
        //     ViewBag.PhongBans = conn.Query<PhongBan>(
        //             "SELECT id, ten_phong_ban AS TenPhongBan FROM phong_ban"
        //         ).ToList();
        //     var list = conn.Query<NhanVien>(
        //     @"SELECT 
        //     nv.id AS Id,
        //     nv.ho_ten AS HoTen,
        //     nv.ngay_sinh::timestamp AS NgaySinh,
        //     nv.so_dien_thoai AS SoDienThoai,
        //     nv.dia_chi AS DiaChi,
        //     nv.chuc_vu AS ChucVu,
        //     nv.so_nam_cong_tac AS SoNamCongTac,
        //     pb.ten_phong_ban AS TenPhongBan
        // FROM nhan_vien nv
        // LEFT JOIN phong_ban pb 
        //     ON nv.phongban_id = pb.id
        //     ORDER BY nv.id"
        //     ).ToList();

        //     using var package = new ExcelPackage();
        //     var ws = package.Workbook.Worksheets.Add("NhanVien");

        //     ws.Cells[1, 1].Value = "ID";
        //     ws.Cells[1, 2].Value = "Họ tên";
        //     ws.Cells[1, 3].Value = "Ngày sinh";
        //     ws.Cells[1, 4].Value = "SĐT";
        //     ws.Cells[1, 5].Value = "Địa chỉ";
        //     ws.Cells[1, 6].Value = "Chức vụ";
        //     ws.Cells[1, 7].Value = "Số năm CT";
        //     ws.Cells[1, 8].Value = "Phòng Ban";

        //     int row = 2;
        //     foreach (var nv in list)
        //     {
        //         ws.Cells[row, 1].Value = nv.ID;
        //         ws.Cells[row, 2].Value = nv.HoTen;
        //         ws.Cells[row, 3].Value = nv.NgaySinh.ToString("dd/MM/yyyy");
        //         ws.Cells[row, 4].Value = nv.SoDienThoai;
        //         ws.Cells[row, 5].Value = nv.DiaChi;
        //         ws.Cells[row, 6].Value = nv.ChucVu;
        //         ws.Cells[row, 7].Value = nv.SoNamCongTac;
        //         ws.Cells[row, 8].Value = nv.TenPhongBan;
        //         row++;
        //     }
        //     ws.Cells.AutoFitColumns();
        //     var stream = new MemoryStream(package.GetAsByteArray());
        //     return File(
        //         stream,
        //         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //         "DanhSachNhanVien.xlsx"
        //     );
        // }
        public IActionResult Filter(int phongBanId = 0)
        {
            using var conn = _context.CreateConnection();
            ViewBag.PhongBans = conn.Query(
                "SELECT id, ten_phong_ban FROM phong_ban ORDER BY ten_phong_ban"
            ).ToList();
            ViewBag.SelectedPhongBan = phongBanId;
            string sql = @"
            SELECT
                nv.id AS Id,
                nv.ho_ten AS HoTen,
                nv.ngay_sinh::timestamp AS NgaySinh,
                nv.so_dien_thoai AS SoDienThoai,
                nv.dia_chi AS DiaChi,
                nv.chuc_vu AS ChucVu,
                nv.so_nam_cong_tac AS SoNamCongTac,
                pb.ten_phong_ban AS TenPhongBan
            FROM nhan_vien nv
            JOIN phong_ban pb ON nv.phongban_id = pb.id
            WHERE (@PhongBanId = 0 OR pb.id = @PhongBanId)
            ORDER BY nv.id
        ";
            var data = conn.Query<NhanVien>(
                sql,
                new { PhongBanId = phongBanId }
            ).ToList();
            return View("Index", data);
        }
        public IActionResult Report(int page = 1)
        {
            int pageSize = 10;
            int offset = (page - 1) * pageSize;

            using var conn = _context.CreateConnection();
            int totalRecord = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM nhan_vien"
            );
            int totalPage = (int)Math.Ceiling(totalRecord / (double)pageSize);
            var sql = @"
            SELECT 
                nv.id AS ID,
                nv.ho_ten AS HoTen,
                nv.ngay_sinh::timestamp AS NgaySinh,
                nv.so_dien_thoai AS SoDienThoai,
                nv.dia_chi AS DiaChi,
                nv.chuc_vu AS ChucVu,
                nv.so_nam_cong_tac AS SoNamCongTac,
                pb.ten_phong_ban AS TenPhongBan
            FROM nhan_vien nv
            LEFT JOIN phong_ban pb 
                ON nv.phongban_id = pb.id
            ORDER BY nv.id
            LIMIT @pageSize OFFSET @offset
        ";
            var data = conn.Query<NhanVien>(
                sql,
                new { pageSize, offset }
            ).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPage;

            return View(data);
        }
        [HttpGet]
        public IActionResult CheckSDT(string soDienThoai)
        {
            using var conn = _context.CreateConnection();
            string sql = @"SELECT (1) FROM nhan_vien WHERE so_dien_thoai=@SoDienThoai";
            int count = conn.ExecuteScalar<int>(sql, new { SoDienThoai = soDienThoai });
            return Json(count > 0);
        }
    }
}