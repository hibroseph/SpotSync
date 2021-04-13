using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace SpotSync.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NoPremium()
        {
            return View();
        }
    }
}
