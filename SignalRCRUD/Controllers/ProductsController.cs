using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRCRUD.Data;
using SignalRCRUD.Models;

namespace SignalRCRUD.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalrServer> _hub;
        private readonly INotyfService _notyf;

        public ProductsController(ApplicationDbContext context,IHubContext<SignalrServer> hub, INotyfService notyf)
        {
            _context = context;
            this._hub = hub;
            this._notyf = notyf;
        }
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok( _context.Products.ToList());
        }
        // GET: Products
        public IActionResult Index()
        {
            return View();
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Category,UnitPrice,StockQty")] Products products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("LoadProducts");
                _notyf.Custom("Create Succussfully- closes in 5 seconds.", 5, "whitesmoke", "fa fa-gear");
                _notyf.Success("Success that closes in 10 Seconds.", 3);
                return RedirectToAction(nameof(Index));
            }
            _notyf.Warning("Some Error Message");
            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _notyf.Warning("Some Error Message");
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                _notyf.Warning("Some Error Message");
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ProductId, [Bind("ProductId,ProductName,Category,UnitPrice,StockQty")] Products products)
        {
            if (ProductId != products.ProductId)
            {
                _notyf.Warning("Some Error Message");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                    await _hub.Clients.All.SendAsync("LoadProducts");
                    _notyf.Success("Success");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductId))
                    {
                        _notyf.Warning("Some Error Message");
                        return NotFound();
                    }
                    else
                    {
                        _notyf.Warning("Some Error Message");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _notyf.Warning("Some Error Message");
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ProductId)
        {
            var products = await _context.Products.FindAsync(ProductId);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("LoadProducts");
            _notyf.Success("Success Notification");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
