using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc3ValidationGroups.Sample.Models;

namespace Mvc3ValidationGroups.Sample.Controllers
{
    public class CheckoutController : Controller
    {
        //
        // GET: /Checkout/

        public ActionResult PlaceOrder()
        {
            var or = new PlaceOrderInfo();
            or.ShoppingBasketKey = Guid.NewGuid().ToString();
            ViewData.Model = or;
            return View();
        }

        [HttpPost]
        public ActionResult PlaceOrder(PlaceOrderInfo model, string placeOrder, string placeOrderAndCreateAccount, string validateDelivery, string validateInvoice)
        {
            string group = null;
            if (placeOrder != null) group = "Critical AboutYou DeliveryAddress InvoiceAddress";
            if (placeOrderAndCreateAccount != null) group = "Critical AboutYou CreateAccount DeliveryAddress InvoiceAddress";
            if (validateDelivery != null) group = "DeliveryAddress";
            if (validateInvoice != null) group = "InvoiceAddress";
            if ((group != null && ModelState.IsGroupValid(model, group)) || (group == null && ModelState.IsValid))
            {
                ViewBag.Groups = group;
                return View("ValidationSuccess");
            }
            return View(model);
        }

    }
}
