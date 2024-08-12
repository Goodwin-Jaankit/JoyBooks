
using Joy.DataAccess.Data;
using Joy.DataAccess.Repository.IRepository;
using Joy.Models;
using Joy.Utility;
using JoyWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {

        private readonly IUnitofWork _unitofWork;
        public CategoryController(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitofWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order should not match the name");
            }
            if (ModelState.IsValid)
            {
                _unitofWork.Category.Add(obj);
                _unitofWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            return View();
        }



        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {

                return NotFound();
            }
            Category? categoryfromDb = _unitofWork.Category.Get(u => u.Id == id);
            if (categoryfromDb == null)
            {

                return NotFound();
            }
            return View(categoryfromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            if (ModelState.IsValid)
            {
                _unitofWork.Category.Update(obj);
                _unitofWork.Save();
                TempData["success"] = "Category Updated successfully";
                return RedirectToAction("Index");
            }

            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {

                return NotFound();
            }
            Category? categoryfromDb = _unitofWork.Category.Get(u => u.Id == id);
            if (categoryfromDb == null)
            {

                return NotFound();
            }
            return View(categoryfromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _unitofWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {

                return NotFound();
            }
            _unitofWork.Category.Remove(obj);
            _unitofWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
