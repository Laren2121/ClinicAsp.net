using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using A1Patients.Models;
using Microsoft.AspNetCore.Http;

namespace A1Patients.Controllers
{
    public class LDMedicationController : Controller
    {
      
        private readonly PatientsContext _context;
        const string medId = "medId";
        const string medName = "MedTypeName";
        string MedTypeName = "";

        public LDMedicationController(PatientsContext context)
        {
            _context = context;
        }

        // GET: LDMedication
        public async Task<IActionResult> Index(int id)
        {
            if (id <= 0)
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString(medId)))
                {
                    TempData["MedSubsistError"] = "Select from list";//MedSubsistError
                    return RedirectToAction("Index", "A1MedicationTypes");
                }
                else
                {
                    id = Convert.ToInt32(HttpContext.Session.GetString(medId));
                    MedTypeName = HttpContext.Session.GetString(medName);
                }
            }
            else
            {
                var MedicationDetail = _context.MedicationType.Where(m => m.MedicationTypeId == id).FirstOrDefault();
                MedTypeName = MedicationDetail.Name.ToString();
                HttpContext.Session.SetString(medId, id.ToString());
                HttpContext.Session.SetString(medName, MedTypeName.ToString());
                MedTypeName = MedicationDetail.Name.ToString();
            }
            var patientsContext = _context.Medication.Include(m => m.ConcentrationCodeNavigation).Include(m => m.DispensingCodeNavigation).Include(m => m.MedicationType).
                Where(m => m.MedicationTypeId == id).OrderBy(m => m.Name).ThenBy(n => n.Concentration);
            MedTypeName = HttpContext.Session.GetString(medName);
            ViewBag.MedicationTypeName = MedTypeName;
            return View(await patientsContext.ToListAsync());
        }

        // GET: LDMedication/Details/5
        public async Task<IActionResult> Details(string id)
        {
            MedTypeName = HttpContext.Session.GetString(medName);
            ViewBag.MedicationTypeName = MedTypeName;

            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // GET: LDMedication/Create
        public IActionResult Create()
        {
            MedTypeName = HttpContext.Session.GetString(medName);
            ViewBag.MedicationTypeName = MedTypeName;


            if (TempData.ContainsKey("MedicationExistErr"))
            {
                string MedicationExistErr = TempData["MedicationExistErr"].ToString();
                if (!String.IsNullOrEmpty(MedicationExistErr))
                {
                    TempData["MedicationExistErr"] = "Already Exist, Fill Again";
                    //ViewBag.MedicationExistErr = MedicationExistErr;
                }
            }
            ViewBag.MedicationTypeId = Convert.ToInt32(HttpContext.Session.GetString(medId));
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m => m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode");
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(m => m.DispensingCode), "DispensingCode", "DispensingCode");
          //  ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name");
            return View();
        }

        // POST: LDMedication/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            if (ModelState.IsValid)
            {
                if (MedicationExists(medication.Din) == false)
                {
                    _context.Add(medication);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString(medId, medication.MedicationTypeId.ToString());
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["MedicationExistErr"] = "Already Exist, Fill Again";
                    return RedirectToAction("Create", "LDMedication");
                }
            }

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);
           // ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        // GET: LDMedication/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            MedTypeName = HttpContext.Session.GetString(medName);
            ViewBag.MedicationTypeName = MedTypeName;
            @ViewBag.MedicationTypeId = Convert.ToInt32(HttpContext.Session.GetString(medId));

            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m => m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(m => m.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        // POST: LDMedication/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            if (id != medication.Din)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.Din))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        // GET: LDMedication/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            MedTypeName = HttpContext.Session.GetString(medName);
            ViewBag.MedicationTypeName = MedTypeName;
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // POST: LDMedication/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medication = await _context.Medication.FindAsync(id);
            _context.Medication.Remove(medication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(string id)
        {
            return _context.Medication.Any(e => e.Din == id);
        }
    }
}
