
using Joy.DataAccess.Repository.IRepository;
using Joy.Models;
using Joy.Models.ViewModels;
using Joy.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Collections.Generic;


namespace JoyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {

        private readonly IUnitofWork _unitofWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitofWork unitofWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofWork = unitofWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitofWork.Product.GetAll(includeProperties: "Category").ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {

            ProductVM vm = new()
            {
                CategoryList = _unitofWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                return View(vm);
            }
            else
            {
                vm.Product = _unitofWork.Product.Get(u => u.Id == id);
                return View(vm);
            }


        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productvm, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                String wwwRootpath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootpath, @"images\product");

                    if (!string.IsNullOrEmpty(productvm.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldimagepath = Path.Combine(wwwRootpath, productvm.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldimagepath))
                        {
                            System.IO.File.Delete(oldimagepath);
                        }
                    }

                    using (var filestream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }

                    productvm.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productvm.Product.Id == 0)
                {
                    _unitofWork.Product.Add(productvm.Product);
                }
                else
                {
                    _unitofWork.Product.Update(productvm.Product);
                }

                _unitofWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productvm.CategoryList = _unitofWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                return View(productvm);
            }


        }


        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {

        //        return NotFound();
        //    }
        //    Product? productfromDb = _unitofWork.Product.Get(u => u.Id == id);
        //    if (productfromDb == null)
        //    {

        //        return NotFound();
        //    }
        //    return View(productfromDb);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product? obj = _unitofWork.Product.Get(u => u.Id == id);
        //    if (obj == null)
        //    {

        //        return NotFound();
        //    }
        //    _unitofWork.Product.Remove(obj);
        //    _unitofWork.Save();
        //    TempData["success"] = "Product deleted successfully";
        //    return RedirectToAction("Index");

        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitofWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitofWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldimagepath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldimagepath))
            {
                System.IO.File.Delete(oldimagepath);
            }

           

            _unitofWork.Product.Remove(productToBeDeleted);

            _unitofWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }



        #endregion

    }
}
