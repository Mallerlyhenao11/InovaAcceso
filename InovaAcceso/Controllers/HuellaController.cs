using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.AspNetCore.Mvc;
using System;

public class HuellaController : Controller
{
    private readonly AppDBContext _appDbContext;
    public HuellaController(AppDBContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public IActionResult CapturarHuella()
    {
        return View();
    }

}
