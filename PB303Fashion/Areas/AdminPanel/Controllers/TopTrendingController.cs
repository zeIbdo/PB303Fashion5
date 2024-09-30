using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PB303Fashion.DataAccessLayer;
using PB303Fashion.DataAccessLayer.Entities;
using PB303Fashion.Extensions;
using PB303Fashion.Models;

namespace PB303Fashion.Areas.AdminPanel.Controllers;

public class TopTrendingController : AdminController
{
    private readonly AppDbContext _dbContext;

    public TopTrendingController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var topTrendings = await _dbContext.TopTrendings.ToListAsync();
        return View(topTrendings);
    }
    public async Task<IActionResult> Update(int? id)
    {
        if(id== null)return NotFound();
        var topTrending=await _dbContext.TopTrendings.FindAsync(id);
        if(topTrending == null) return NotFound();
        return View(topTrending);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(TopTrending topTrending)
    {
        var existingTt=await _dbContext.TopTrendings.FindAsync(topTrending.Id);
        if(existingTt == null) return NotFound();
        if(!ModelState.IsValid)return View(existingTt);
        if (topTrending.ImageFile != null)
        {
            if (!topTrending.ImageFile.IsImage())
            {
                ModelState.AddModelError("ImageFile", "Sekil secmelisiz");

                return View();
            }

            if (!topTrending.ImageFile.IsAllowedSize(1))
            {
                ModelState.AddModelError("ImageFile", "Sekil olcusu max 1mb olmalidir");

                return View();
            }
            var path = Path.Combine(Constants.TopTrendingImagePath,existingTt.ImgUrl);
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            var newUrl= await topTrending.ImageFile.GenerateFileAsync(Constants.TopTrendingImagePath);
            existingTt.ImgUrl = newUrl;
        }
        existingTt.Description = topTrending.Description;
        existingTt.Content = topTrending.Content;
        existingTt.SubText = topTrending.SubText;
            _dbContext.TopTrendings.Update(existingTt);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TopTrending topTrending)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        if(topTrending.ImageFile==null)return View();
        if (!topTrending.ImageFile.IsImage())
        {
            ModelState.AddModelError("ImageFile", "Sekil secmelisiz");

            return View();
        }

        if (!topTrending.ImageFile.IsAllowedSize(1))
        {
            ModelState.AddModelError("ImageFile", "Sekil olcusu max 1mb olmalidir");

            return View();
        }

        var imageName = await topTrending.ImageFile.GenerateFileAsync(Constants.TopTrendingImagePath);

        topTrending.ImgUrl = imageName;


        await _dbContext.TopTrendings.AddAsync(topTrending);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null) return NotFound();
        var topTrending= await _dbContext.TopTrendings.FindAsync(id);
        if(topTrending == null) return NotFound();
        return View(topTrending);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(TopTrending topTrending)
    {
        var existingTt = await _dbContext.TopTrendings.FindAsync(topTrending.Id);

        if (existingTt == null) return NotFound();
        var path = Path.Combine(Constants.TopTrendingImagePath, existingTt!.ImgUrl);
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        _dbContext.TopTrendings.Remove(existingTt);

        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
}
