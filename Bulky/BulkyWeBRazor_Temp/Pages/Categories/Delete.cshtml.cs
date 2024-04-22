using Azure.Core.Pipeline;
using BulkyWeBRazor_Temp.Data;
using BulkyWeBRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeBRazor_Temp.Pages.Categories;

[BindProperties]
public class Delete : PageModel
{
    private readonly ApplicationDbContext _db;
    public Category Category { get; set; }
    public Delete(ApplicationDbContext db)
    {
        _db = db;
    }
    public void OnGet(int? id)
    {
        if (id != null && id != 0)
        {
            Category = _db.Categories.Find(id);
        }
    }

    public IActionResult OnPost()
    {
        Category? obj = _db.Categories.Find(Category.Id);
        if (obj == null)
        {
            return NotFound();
        }

        _db.Categories.Remove(obj);
        _db.SaveChanges();
        TempData["success"] = "Category successfully deleted!";
        return RedirectToPage("Index");
    }
}