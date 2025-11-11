using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CityCountry.Data;
using CityCountry.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CityCountry.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index(int page = 1)
        {
            var totalCountries = await _context.Countries.CountAsync();
            var countries = await _context.Countries
                .Include(c => c.Cities)
                .OrderBy(c => c.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCountries / (double)PageSize);

            return View(countries);
        }

        // GET: Countries/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Country country)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Countries.AnyAsync(c => c.Name == country.Name))
                {
                    ModelState.AddModelError("Name", "Country with this name already exists");
                    return View(country);
                }

                _context.Add(country);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Country added successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var country = await _context.Countries.FindAsync(id);
            if (country == null) return NotFound();

            return View(country);
        }

        // POST: Countries/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
        {
            if (id != country.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Countries.AnyAsync(c => c.Name == country.Name && c.Id != id))
                {
                    ModelState.AddModelError("Name", "Country with this name already exists");
                    return View(country);
                }

                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Country updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}