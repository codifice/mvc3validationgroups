using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Mvc3ValidationGroups.Sample.Models
{
    public class PlaceOrderInfo
    {
        [Required(AllowEmptyStrings = false)]
        [ValidationGroup("Critical")]
        public string ShoppingBasketKey { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Duplicate Error Message")]
        [DataType(DataType.EmailAddress)]
        [ValidationGroup("AboutYou")]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [ValidationGroup("CreateAccount")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [ValidationGroup("CreateAccount")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Duplicate Error Message")]
        [DataType(DataType.Text)]
        [ValidationGroup("AboutYou")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [ValidationGroup("DeliveryAddress")]
        public string DeliveryAddressLine1 { get; set; }

        [Required(AllowEmptyStrings = false)]
        [ValidationGroup("DeliveryAddress")]
        public string DeliveryAddressLine2 { get; set; }

        [Required(AllowEmptyStrings = false)]
        [ValidationGroup("DeliveryAddress")]
        public string DeliveryAddressPostalCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [ValidationGroup("InvoiceAddress")]
        public string InvoiceAddressLine1 { get; set; }

        [Required(AllowEmptyStrings = false)]
        [ValidationGroup("InvoiceAddress")]
        public string InvoiceAddressLine2 { get; set; }

        [Required(AllowEmptyStrings = false)]
        [ValidationGroup("InvoiceAddress")]
        public string InvoiceAddressPostalCode { get; set; }

    }
}