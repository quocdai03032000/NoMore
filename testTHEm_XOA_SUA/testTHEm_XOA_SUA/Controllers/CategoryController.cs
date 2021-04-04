using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using testTHEm_XOA_SUA.Models;

namespace testTHEm_XOA_SUA.Controllers
{
    public class CategoryController : Controller
    {
        testWebNCEntities database = new testWebNCEntities();
        // GET: Category
        public ActionResult Index()
        {
            return View(database.Categories.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category cate)
        {
            database.Categories.Add(cate);
            database.SaveChanges();           
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            return View(database.Categories.Where(a => a.Id == id).FirstOrDefault());
        }

        
        public ActionResult Edit(int id)
        {
            return View(database.Categories.Where(a => a.Id == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(int id , Category cate)
        {
            database.Entry(cate).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            return View(database.Categories.Where(a => a.Id == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(int id , Category cate)
        {
            cate = database.Categories.Where(a => a.Id == id).FirstOrDefault();
            database.Categories.Remove(cate);
            database.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}