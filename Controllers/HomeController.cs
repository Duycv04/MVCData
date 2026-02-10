using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCData.Models;
using System.Data;

using Dapper.FastCrud;

namespace MVCData.Controllers;

public class HomeController : Controller
{
    private readonly DapperContext _context;
     public HomeController(DapperContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        using var connection = _context.CreateConnection();

        var products = connection.Find<SanPham>(stmt => stmt
                        .OrderBy($"{nameof(SanPham.ProductID):C} DESC")
                        .Top(8)
                    ).ToList();

        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
