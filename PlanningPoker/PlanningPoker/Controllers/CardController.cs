using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlanningPoker.Models;
using PlanningPoker.Services;

namespace PlanningPoker.Controllers
{
    public class CardController : Controller
    {
        private  CardService cardService;

        public CardController()
        {
            cardService = new CardService();
        }

        // GET: /Card/Index
        public ActionResult Index()
        {
            var items = cardService.GetCards();
            return View(items);

        }

        public ActionResult Create()
        {
            return View();
        }

        //POST: /Card/Create
        [HttpPost]
        public ActionResult Create(Card model)
        {
            if (ModelState.IsValid)
            {
                cardService.AddCard(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            return View(cardService.GetById(id));
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            cardService.DeleteCard(id);
            return RedirectToAction("Index");
        }
    }
}