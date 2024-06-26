﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KendoCRUDService.Models;
using KendoCRUDService.Common;
using System.Web.Script.Serialization;

namespace KendoCRUDService.Controllers
{
    public class ProductsController : Controller
    {
        public ActionResult Index()
        {
            return this.Jsonp(ProductRepository.All());
        }                       
        
        public JsonResult Update()
        {
            var models = this.DeserializeObject<List<ProductModel>>("models");
            if (models != null)
            {
                ProductRepository.Update(models);
            }
            return this.Jsonp(models);
        }
        
        public ActionResult Destroy()
        {
            var products = this.DeserializeObject<List<ProductModel>>("models");

            if (products != null)
            {
                ProductRepository.Delete(products);
            }
            return this.Jsonp(products);
        }
        
        public ActionResult Create()
        {
            var products = this.DeserializeObject<List<ProductModel>>("models");
            if (products != null)
            {
                ProductRepository.Insert(products);
            }
            return this.Jsonp(products);
        }

        public JsonResult Read(int skip, int take)
        {
            IEnumerable<ProductModel> result = ProductRepository.All().OrderByDescending(p => p.ProductID);
            
            result = result.Skip(skip).Take(take);

            return this.Jsonp(result);
        }

        public JsonResult Submit()
        {
            var model = this.DeserializeObject<SpreadsheetSubmitViewModel>("models");

            if (model != null && model.Created != null)
            {
                ProductRepository.Insert(model.Created);
            }

            if (model != null && model.Updated != null)
            {
                ProductRepository.Update(model.Updated);
            }

            if (model != null && model.Destroyed != null)
            {
                ProductRepository.Delete(model.Destroyed);
            }

            return this.Jsonp(model);
        }
    }
}
