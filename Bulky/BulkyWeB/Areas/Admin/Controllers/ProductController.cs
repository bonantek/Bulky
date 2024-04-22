using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeB.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller 
    {
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _UnitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
    public IActionResult Index()
    {
        List<Product> objProductList = _UnitOfWork.Product.GetAll(includeProperties:"Category").ToList();
        return View(objProductList);
    }

    public IActionResult Upsert(int? id)
    {
        IEnumerable<SelectListItem> CategoryList = _UnitOfWork.Category.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });

        ProductVM productVm = new()
        {
            CategoryList = CategoryList,
            Product = new Product()
        };
        if (id == null || id == 0)
        {
            //create
            return View(productVm);
        }
        else
        {
            productVm.Product = _UnitOfWork.Product.Get(u => u.Id == id);
            return View(productVm);
        }
        
    }
    
    [HttpPost]
    public IActionResult Upsert(ProductVM productVm, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (file != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images/product");
                
                if(!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                {
                    //delete old image
                    var oldImagePath = Path.Combine(wwwRootPath, productVm.Product.ImageUrl.Trim('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                {  
                    file.CopyTo(fileStream);
                }

                productVm.Product.ImageUrl = @"/images/product/" + filename;
            }

            if (productVm.Product.Id == 0)
            {
                _UnitOfWork.Product.Add(productVm.Product);
            }
            else
            {
                _UnitOfWork.Product.Update(productVm.Product );
            }
            _UnitOfWork.Save();
            TempData["success"] = "Product created successfully!";
            return RedirectToAction("Index", "Product");
        }
        else
        {
            productVm.CategoryList = _UnitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }
        return View(productVm);

    }
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
 
        Product? productFromDb = _UnitOfWork.Product.Get(u => u.Id == id);
        if (productFromDb == null)
        {
            return NotFound();
        }
        return View(productFromDb); 
    }
    
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        Product? obj = _UnitOfWork.Product.Get(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }

        _UnitOfWork.Product.Remove(obj);
        _UnitOfWork.Save();
        TempData["success"] = "Product deleted successfully!";
        return RedirectToAction("Index");
    }
    
    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Product> objProductList = _UnitOfWork.Product.GetAll(includeProperties:"Category").ToList();
        return Json(new { data = objProductList });
    }
    #endregion
    
    }
}