using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlanningPoker.Models;
using PlanningPoker.Services;
using PlanningPoker.ViewModels;

namespace PlanningPoker.Controllers
{
    public class CardController : Controller
    {
        private CardViewModel _cardViewModel;
        private CardService _cardService;
        private ScaleService _scaleService;

        public CardController()
        {
            _scaleService = new ScaleService();
            _cardService = new CardService();
            _cardViewModel = new CardViewModel();
            
        }

        // GET: /Card/Index
        public ActionResult Index()
        {
            _cardViewModel.Cards = _cardService.GetCards();
            _cardViewModel.Marks = _scaleService.GetMarks();
            var items = _cardViewModel;
            return View(items);

        }

        // POST: /Card/CardResult
        [HttpPost]
        public ActionResult CardResult(string card, string mark, string submit)
        {
            Card _card = new Card();
            _card.Name = card;
            return String.IsNullOrEmpty(submit) ? View("Create") : View(_card);
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
                _cardService.AddCard(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult Delete(string name)
        {
            return View(_cardService.GetById(name));
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(string name)
        {
            _cardService.DeleteCard(name);
            return RedirectToAction("Index");
        }
    }
}