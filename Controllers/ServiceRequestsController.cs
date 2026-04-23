using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using PROG7311Part2.Models;
using PROG7311Part2.Database;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace PROG7311Part2.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;

        public ServiceRequestsController(AppDbContext context, IHttpClientFactory factory, IWebHostEnvironment env)
        {
            _context = context;
            _httpClient = factory.CreateClient();
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            var requests = _context.ServiceRequests.Include(s => s.Contract);
            return View(await requests.ToListAsync());
        }

        public async Task<IActionResult> Create(int? clientId)
        {
            var contractsQuery = _context.Contracts.Include(c => c.Client).AsQueryable();
            if (clientId.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.ClientId == clientId.Value);
            }

            var list = await contractsQuery
                .Select(c => new { c.ContractId, Text = c.ContractId + " - " + (c.Client != null ? c.Client.Name : "") })
                .ToListAsync();

            ViewBag.ContractId = new SelectList(list, "ContractId", "Text");
            ViewBag.HasContracts = list.Any();
            ViewBag.SelectedClientId = clientId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest request, IFormFile file)
        {
            var contract = await _context.Contracts.FindAsync(request.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("", "Invalid contract.");
            }
            else if (contract.Status == "Expired" || contract.Status == "On Hold")
            {
                ModelState.AddModelError("", "Cannot create request for inactive contract.");
            }

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".pdf")
                {
                    ModelState.AddModelError("", "Only PDF files are allowed for attachments.");
                }
                else
                {
                    var safeFileName = Path.GetFileName(file.FileName);

                    var filesFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "files");
                    Directory.CreateDirectory(filesFolder);

                    var path = Path.Combine(filesFolder, safeFileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    request.PDFAttachmentFilePath = "/files/" + safeFileName;
                }
            }

            if (string.IsNullOrEmpty(request.Status))
            {
                request.Status = "Open";
            }

            if (string.IsNullOrEmpty(request.PDFAttachmentFilePath))
            {
                request.PDFAttachmentFilePath = string.Empty;
            }

            if (ModelState.IsValid)
            {
                request.Cost = await ConvertUsdToZar(request.Cost);
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var contracts = await _context.Contracts.Include(c => c.Client)
                .Select(c => new { c.ContractId, Text = c.ContractId + " - " + (c.Client != null ? c.Client.Name : "") })
                .ToListAsync();
            ViewBag.ContractId = new SelectList(contracts, "ContractId", "Text", request.ContractId);
            ViewBag.HasContracts = contracts.Any();
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.Errors = errors;

            return View(request);
        }

        private async Task<decimal> ConvertUsdToZar(decimal usd)
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://api.exchangerate-api.com/v4/latest/USD");

                var j = JObject.Parse(response);
                var rateToken = j.SelectToken("rates.ZAR");
                if (rateToken == null)
                {
                    return usd;
                }

                if (!decimal.TryParse(rateToken.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate))
                {
                    try
                    {
                        rate = rateToken.Value<decimal>();
                    }
                    catch
                    {
                        return usd;
                    }
                }

                return usd * rate;
            }
            catch
            {
                return usd;
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();
            ViewBag.ContractId = new SelectList(
                _context.Contracts.Include(c => c.Client)
                    .Select(c => new { c.ContractId, Text = c.ContractId + " - " + (c.Client != null ? c.Client.Name : "") }),
                "ContractId",
                "Text",
                request.ContractId);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ServiceRequest request)
        {
            if (id != request.ServiceRequestId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(s => s.ServiceRequestId == id);

            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();
            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
