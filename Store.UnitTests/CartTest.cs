using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Store.Domain.Abstract;
using Store.WebUI.Controllers;
using System.Web.Mvc;
using Store.WebUI.Models;
using Store.Domain.Entities;

namespace Store.UnitTests
{
    [TestClass]
    class CartTest
    {
        [TestMethod]
        public void Can_Add_To_Cart()
        {
            // przygotowanie — tworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Jab"},
            }.AsQueryable());

            // przygotowanie — utworzenie koszyka
            Cart cart = new Cart();
            // przygotowanie — utworzenie kontrolera
            CartController target = new CartController(mock.Object);
            // działanie — dodanie produktu do koszyka
            target.AddToCart(cart, 1, null);

            // asercje
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            // przygotowanie — tworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
            }.AsQueryable());
            // przygotowanie — utworzenie koszyka
            Cart cart = new Cart();
            // przygotowanie — utworzenie kontrolera
            CartController target = new CartController(mock.Object);
            // działanie — dodanie produktu do koszyka
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");
            // asercje
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            // przygotowanie — utworzenie koszyka
            Cart cart = new Cart();
            // przygotowanie — utworzenie kontrolera
            CartController target = new CartController(null);
            // działanie — wywołanie metody akcji Index
            CartIndexViewModel result
            = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;
            // asercje
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
    }
}
