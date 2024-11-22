using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.DataAccess;
using Uniqlo.Models;
using Uniqlo.ViewModels.Sliders;

namespace Uniqlo.Areas.Admin.Controllers;
[Area("Admin")]
public class SliderController(UniqloDbContext _context,IWebHostEnvironment _env):Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Sliders.ToListAsync());
    }
    
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(SliderCreateVM vm)
    {
        if (!ModelState.IsValid) { return View(vm); }

        if (!vm.File.ContentType.StartsWith("image"))
        {
            ModelState.AddModelError("File","Format type must be image");
            return View(vm);
        }

        if (vm.File.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("File","File size must be less than 2 MB");
        }
        string newFileName =Path.GetRandomFileName()+ Path.GetExtension(vm.File.FileName);
        
        using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath,"imgs","sliders",newFileName)))
        {
            await vm.File.CopyToAsync(stream);
        }

        Slider slider = new Slider
        {
            ImageUrl = newFileName,
            Title = vm.Title,
            Subtitle = vm.Subtitle!,
            Link = vm.Link
        };
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); 
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        Slider slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();
        return View(slider);
    }

    [HttpPost]
    public async Task<ActionResult> Update(int id,SliderCreateVM vm)
    {
        Slider slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();
        slider.Title = vm.Title;
        slider.Subtitle = vm.Subtitle;
        slider.Link = vm.Link;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        Slider slider = await _context.Sliders.FindAsync(id);
        if (slider == null) {return NotFound();}
        string filePath =Path.Combine("wwwroot","imgs","sliders",slider.ImageUrl);
        if(System.IO.File.Exists(filePath)) {System.IO.File.Delete(filePath);}
        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Hide(int id)
    {
        Slider slider = await _context.Sliders.FindAsync(id);
        if (slider == null) {return NotFound();}
        slider.IsDeleted = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Show(int id)
    {
        Slider slider = await _context.Sliders.FindAsync(id);
        if (slider == null) {return NotFound();}
        slider.IsDeleted = false;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}