using BulkyWeBRazor_Temp.Data;
using BulkyWeBRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeBRazor_Temp.Pages.Categories;

[BindProperties]
public class Create : PageModel
{
    private readonly ApplicationDbContext _db;
    public Category Category { get; set; }
    public Create(ApplicationDbContext db)
    {
        _db = db;
    }
    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        _db.Categories.Add(Category);
        _db.SaveChanges();
        TempData["success"] = "Category successfully created!";
        return RedirectToPage("Index");
    }
}