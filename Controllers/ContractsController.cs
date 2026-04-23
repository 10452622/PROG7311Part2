using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311Part2.Models;
using PROG7311Part2.Database;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PROG7311Part2.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var contracts = _context.Contracts.Include(c => c.Client).AsQueryable();

            var totalContracts = await _context.Contracts.CountAsync();
            ViewBag.TotalContracts = totalContracts;

            if (!string.IsNullOrEmpty(status))
            {
                var statusNorm = status.Trim().ToLower();
                contracts = contracts.Where(c => c.Status.ToLower() == statusNorm);
            }

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c => c.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                contracts = contracts.Where(c => c.EndDate <= endDate.Value);
            }

            var matchedCount = await contracts.CountAsync();
            ViewBag.MatchedContracts = matchedCount;

            ViewBag.CurrentStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            var allContracts = await _context.Contracts.Include(c => c.Client).ToListAsync();
            ViewBag.AllContracts = allContracts;

            return View(await contracts.ToListAsync());
        }

        public IActionResult Create(int? clientId)
        {
            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", clientId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile file)
        {
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                    ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
                    ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return View(contract);
                }

                var filesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
                Directory.CreateDirectory(filesFolder);
                var safeFileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(filesFolder, safeFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                contract.PDFAgreementFilePath = "/files/" + safeFileName;
            }

            if (string.IsNullOrEmpty(contract.PDFAgreementFilePath))
            {
                contract.PDFAgreementFilePath = string.Empty;
            }

            if (ModelState.IsValid)
            {
                _context.Add(contract);
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Contract #{contract.ContractId} created.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Failed to save contract: " + ex.Message);
                }
            }

            // collect modelstate errors for display
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.Errors = errors;
            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract?.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract)
        {
            if (id != contract.ContractId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
