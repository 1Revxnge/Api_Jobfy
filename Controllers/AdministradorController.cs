﻿using Microsoft.AspNetCore.Mvc;

namespace ApiJobfy.Controllers
{
    public class AdministradorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
