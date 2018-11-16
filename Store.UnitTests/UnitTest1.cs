using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Moq;
using Store.Domain.Abstract;
using Store.Domain.Entities;
using Store.WebUI.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;
using Store.WebUI.Models;
using Store.WebUI.HTMLHelper;
using Microsoft.CSharp;

namespace Store.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1"},
            new Product {ProductID = 2, Name = "P2"},
            new Product {ProductID = 3, Name = "P3"},
            new Product {ProductID = 4, Name = "P4"},
            new Product {ProductID = 5, Name = "P5"}
            });
           
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            
            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;
            
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }
        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            ProductController controler = new ProductController(mock.Object);
            controler.PageSize = 3;

            Product[] result = ((ProductListViewModel)controler.List("Cat2", 1).Model).Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
                new Product {ProductID = 2, Name = "P2", Category = "Jabłka"},
                new Product {ProductID = 3, Name = "P3", Category = "Śliwki"},
                new Product {ProductID = 4, Name = "P4", Category = "Pomarańcze"},
            });

            NavController target = new NavController(mock.Object);

            string[] result = ((IEnumerable<string>)target.Menu().Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0], "Jabłka");
            Assert.AreEqual(result[1], "Pomarańcze");
            Assert.AreEqual(result[2], "Śliwki");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] 
            {
             new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
             new Product {ProductID = 4, Name = "P2", Category = "Pomarańcze"},
             });

            NavController target = new NavController(mock.Object);

            string categoryToSelect = "Jabłka";

            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {            
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] 
            {
            new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
            new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
            new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
            new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
            new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });
            
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;
            
            int res1 = ((ProductListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }

        [TestMethod]
        public void Can_Add_New_Lines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {            
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            
            Cart target = new Cart();
            
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();
            // asercje
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {            
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };
            
            Cart target = new Cart();
            
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);
           
            target.RemoveLine(p2);
            
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {            
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };
            
            Cart target = new Cart();
            
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();
            
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {            
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };
            
            Cart target = new Cart();
            
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            
            target.Clear();
            
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {            
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Jab"},
            }.AsQueryable());

            
            Cart cart = new Cart();
           
            CartController target = new CartController(mock.Object, null);
            
            target.AddToCart(cart, 1, null);

            
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {            
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
            }.AsQueryable());
            
            Cart cart = new Cart();
            
            CartController target = new CartController(mock.Object, null);
            
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");
            
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {            
            Cart cart = new Cart();
           
            CartController target = new CartController(null, null);
            
            CartIndexViewModel result
            = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;
           
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {            
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            
            Cart cart = new Cart();
           
            ShippingDetails shippingDetails = new ShippingDetails();
           
            CartController target = new CartController(null, mock.Object);
            
            ViewResult result = target.Checkout(cart, shippingDetails);
            
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
            Times.Never());
            
            Assert.AreEqual("", result.ViewName);            
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {            
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            
            CartController target = new CartController(null, mock.Object);
           
            target.ModelState.AddModelError("error", "error");
            
            ViewResult result = target.Checkout(cart, new ShippingDetails());
            
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
            Times.Never());
            
            Assert.AreEqual("", result.ViewName);            
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {            
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            
            CartController target = new CartController(null, mock.Object);
            
            ViewResult result = target.Checkout(cart, new ShippingDetails());
            
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
            Times.Once());
           
            Assert.AreEqual("Completed", result.ViewName);           
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
