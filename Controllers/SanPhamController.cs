using Microsoft.AspNetCore.Mvc;
using MVCData.Models;
using MVCData.Controllers;
using MVCData.Helpers;
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
using System.Net.WebSockets;
using System.Diagnostics.SymbolStore;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Web;
using System.Runtime.ConstrainedExecution;
using System.Dynamic;
using Microsoft.Win32;
using Dapper.FastCrud;

namespace MVCData.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly DapperContext _context;
        public SanPhamController(DapperContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            using var conn = _context.CreateConnection();
            var data = conn.Find<SanPham>(stmt => stmt
            .OrderBy($"product_id ASC")).ToList();
            return View(data);
        }
        [HttpGet]
        public IActionResult GetById(int id)
        {
            using var conn = _context.CreateConnection();

            var product = conn.Get(new SanPham
            {
                ProductID = id
            });

            return Json(product);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(SanPham model, IFormFile imageFile)
        {
            try
            {
                string fileName = model.ImageUrl ?? "";
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    var originalName = Path.GetFileName(imageFile.FileName);
                    var existingPath = Path.Combine(uploadDir, originalName);

                    if (System.IO.File.Exists(existingPath))
                    {
                        fileName = originalName;
                    }
                    else
                    {
                        var ext = Path.GetExtension(originalName);
                        fileName = Guid.NewGuid().ToString() + ext;
                        using var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create);
                        imageFile.CopyTo(stream);
                    }
                }
                model.ImageUrl = fileName;

                using var conn = _context.CreateConnection();
                conn.Insert(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            using var conn = _context.CreateConnection();
            var product = conn.Get(new SanPham { ProductID = id });
            if (product == null)
                return NotFound();
            return View(product);
        }
        [HttpPost]
        public IActionResult Update(SanPham model, IFormFile imageFile)
        {
            try
            {
                var oldFile = model.ImageUrl;
                string fileName = oldFile ?? "";

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    var originalName = Path.GetFileName(imageFile.FileName);
                    var existingPath = Path.Combine(uploadDir, originalName);

                    if (System.IO.File.Exists(existingPath))
                    {
                        fileName = originalName;
                    }
                    else
                    {
                        var ext = Path.GetExtension(originalName);
                        fileName = Guid.NewGuid().ToString() + ext;
                        using var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create);
                        imageFile.CopyTo(stream);
                    }
                }
                model.ImageUrl = fileName;
                using var conn = _context.CreateConnection();
                conn.Update(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            using var conn = _context.CreateConnection();
            var product = conn.Get(new SanPham { ProductID = id });
            if (product == null)
                return NotFound();
            try
            {
                conn.Delete(product);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = _context.CreateConnection();
            var product = conn.Get(new SanPham { ProductID = id });
            if (product == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            try
            {
                conn.Delete(product);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult Search(string keyword)
        {
            try
            {
                using var conn = _context.CreateConnection();
                var query = @"SELECT
                item.product_id As ProductID,
                item.product_code As ProductCode,
                item.product_name As ProductName,
                item.image_url As ImageUrl,
                item.brand As Brand,
                item.price As Price,
                item.quantity As Quantity,
                item.description As Description,
                item.status As status
                FROM products item
                WHERE 
                @kw IS NULL
                OR CAST(item.product_id AS TEXT) ILIKE @kw
                OR item.product_code ILIKE @kw
                OR item.product_name ILIKE @kw
                OR item.Brand ILIKE @kw
                ORDER BY item.product_id;
                ";
                var data = conn.Query<SanPham>(query,
                new { kw = string.IsNullOrEmpty(keyword) ? null : "%" + keyword + "%" }).ToList();
                return PartialView("_TableBody", data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public IActionResult FillterPrice(decimal? minPrice, decimal? maxPrice, string? brand, string? sortPrice)
        {
            try
            {
                using var conn = _context.CreateConnection();
                var orderBy = sortPrice == "desc"
                ? "ORDER BY price DESC"
                : "ORDER BY price ASC";
                var sql = $@"
                        SELECT 
                        item.product_id As ProductID,
                        item.product_code As ProductCode,
                        item.product_name As ProductName,
                        item.image_url As ImageUrl,
                        item.brand As Brand,
                        item.price As Price,
                        item.quantity As Quantity,
                        item.description As Description,
                        item.status As status
                        FROM products item
                        WHERE (@Minprice IS NULL OR item.price >= @Minprice)
                        AND (@Maxprice IS NULL OR item.price <= @Maxprice)
                        AND (@Brand IS NULL OR item.brand = @Brand)
                        {orderBy}
                    ";
                var ds = conn.Query<SanPham>(sql, new
                {
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Brand = string.IsNullOrEmpty(brand) ? null : brand
                }).ToList();
                return View("_TableBody", ds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public IActionResult SelectSP()
        {
            using var conn = _context.CreateConnection();

            var sql = @"
            SELECT 
                item.product_id ASProductID,
                item.product_name AS ProductName,
                item.price AS Price,
                item.quantity AS Quantity,
                item.image_url AS ImageUrl
            FROM products AS
            WHERE Quantity >= 0
            ORDER BY ProductID DESC
            LIMIT 8
        ";
            var products = conn.Query<SanPham>(sql).ToList();

            return View(products);
        }
    }
}