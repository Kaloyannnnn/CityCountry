using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CityCountry.Data;
using CityCountry.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CityCountry.Controllers
{
    public class CitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cities
        public async Task<IActionResult> Index(int page = 1)
        {
            var totalCities = await _context.Cities.CountAsync();
            var cities = await _context.Cities
                .Include(c => c.Country)
                .OrderBy(c => c.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCities / (double)PageSize);

            return View(cities);
        }

        // GET: Cities/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            return View();
        }

        // POST: Cities/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CountryId")] City city)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Cities.AnyAsync(c => c.Name == city.Name))
                {
                    ModelState.AddModelError("Name", "City with this name already exists");
                    ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
                    return View(city);
                }

                _context.Add(city);
                await _context.SaveChangesAsync();
                TempData["Success"] = "City added successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
            return View(city);
        }

        // GET: Cities/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var city = await _context.Cities.FindAsync(id);
            if (city == null) return NotFound();

            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
            return View(city);
        }

        // POST: Cities/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CountryId")] City city)
        {
            if (id != city.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Cities.AnyAsync(c => c.Name == city.Name && c.Id != id))
                {
                    ModelState.AddModelError("Name", "City with this name already exists");
                    ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
                    return View(city);
                }

                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "City updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
            return View(city);
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}