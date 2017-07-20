using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlanningPocker.Services;

namespace PlanningPocker.Controllers
{
    public class HomeController : Controller
    {
        public CardService cards;

        public HomeController()
        {
            cards = new CardService();
        }
        public IActionResult Index()
        {
            return View(cards);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
