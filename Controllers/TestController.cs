using Dapper;
using Microsoft.AspNetCore.Mvc;

public class TestController : Controller
{
    private readonly DapperContext _context;

    public TestController(DapperContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        using var conn = _context.CreateConnection();
        var count = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM products");
        return Content("Kết nối thành công. Số nhân viên = " + count);
    }
}
