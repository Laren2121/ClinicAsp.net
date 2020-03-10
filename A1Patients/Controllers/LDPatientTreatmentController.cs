using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using A1Patients.Models;

namespace A1Patients.Controllers
{
    public class LDPatientTreatmentController : Controller
    {
        private readonly PatientsContext _context;

        public LDPatientTreatmentController(PatientsContext context)
        {
            _context = context;
        }

        // GET: LDPatientTreatment


        //passing patientDiagnosisID 
        public async Task<IActionResult> Index(string patientDiagnosisID, string diagnosisID)
        {
            //getting value for cookies and session 
            if (patientDiagnosisID != null)
            {
                Response.Cookies.Append("patientDiagnosisID", patientDiagnosisID);
            }
            else if (Request.Query["patientDiagnosisID"].Any())
            {
                Response.Cookies.Append("patientDiagnosisID", Request.Query["patientDiagnosisID"]);
                patientDiagnosisID = Request.Query["patientDiagnosisID"];
            }
            else if (Request.Cookies["patientDiagnosisID"] != null)
            {
                patientDiagnosisID = Request.Cookies["patientDiagnosisID"].ToString();
            }
            else
            {
                TempData["Message"] = "Select patient diagnosis";
                return RedirectToAction("Index", "LDPatientDiagnosis");
            }

            //diagnosisid
            if (Request.Query["diagnosisID"].Any())
            {
                Response.Cookies.Append("diagnosisID", Request.Query["diagnosisID"]);
                patientDiagnosisID = Request.Query["diagnosisID"];
            }
            else if(Request.Cookies["diagnosisID"] != null)
            {
                patientDiagnosisID = Request.Cookies["patientDiagnosisID"].ToString();
            }


            var name = _context.PatientDiagnosis.Include(p => p.Diagnosis).Include(p => p.Diagnosis)
                .Include(p => p.Patient).Where(p => p.PatientDiagnosisId == Convert.ToInt32(patientDiagnosisID)).FirstOrDefault();

            ViewData["name"] = name.Patient.LastName + " " + name.Patient.FirstName;
            ViewData["diagnosis"] = name.Diagnosis.Name;


            var patientsContext = _context.PatientTreatment.Include(p => p.PatientDiagnosis).Include(p => p.Treatment)
                .Where(p=>p.PatientDiagnosisId == Convert.ToInt32(patientDiagnosisID)).OrderBy(m=>m.DatePrescribed);
            return View(await patientsContext.ToListAsync());
        }

        // GET: LDPatientTreatment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            return View(patientTreatment);
        }

        // GET: LDPatientTreatment/Create
        public IActionResult Create()
        {
            string patientID = string.Empty;
            if(Request.Cookies["patientDiagnosisID"] != null)
            {
                patientID = Request.Cookies["patientDiagnosisID"];
            }

            var name = _context.PatientDiagnosis.Include(p => p.Diagnosis).Include(p => p.Diagnosis)
                .Include(p => p.Patient).Where(p => p.PatientDiagnosisId == Convert.ToInt32(patientID)).FirstOrDefault();

            ViewData["name"] = name.Patient.LastName + " " + name.Patient.FirstName;
            ViewData["diagnosis"] = name.Diagnosis.Name;

            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId");
            ViewData["TreatmentId"] = new SelectList(_context.Treatment.Where(a=>a.DiagnosisId == Convert.ToInt32(patientID)), "TreatmentId", "Name");
            return View();
        }

        // POST: LDPatientTreatment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            string patientID = string.Empty;
            if (Request.Cookies["patientDiagnosisID"] != null)
            {
                patientID = Request.Cookies["patientDiagnosisID"];
            }
            ViewData["patientDiagnosisID"] = patientID;

            if (ModelState.IsValid)
            {
                _context.Add(patientTreatment);
                patientTreatment.DatePrescribed = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: LDPatientTreatment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string patientID = string.Empty;
            if (Request.Cookies["patientDiagnosisID"] != null)
            {
                patientID = Request.Cookies["patientDiagnosisID"];
            }

            var name = _context.PatientDiagnosis.Include(p => p.Diagnosis).Include(p => p.Diagnosis)
                .Include(p => p.Patient).Where(p => p.PatientDiagnosisId == Convert.ToInt32(patientID)).FirstOrDefault();

            ViewData["name"] = name.Patient.LastName + " " + name.Patient.FirstName;
            ViewData["diagnosis"] = name.Diagnosis.Name;

            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            //ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment.Where(a => a.DiagnosisId == Convert.ToInt32(patientID)), "TreatmentId", "Name");
            return View(patientTreatment);
        }

        // POST: LDPatientTreatment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            if (id != patientTreatment.PatientTreatmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientTreatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientTreatmentExists(patientTreatment.PatientTreatmentId))
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
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: LDPatientTreatment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }


            string patientID = string.Empty;
            if (Request.Cookies["patientDiagnosisID"] != null)
            {
                patientID = Request.Cookies["patientDiagnosisID"];
            }

            var name = _context.PatientDiagnosis.Include(p => p.Diagnosis).Include(p => p.Diagnosis)
                .Include(p => p.Patient).Where(p => p.PatientDiagnosisId == Convert.ToInt32(patientID)).FirstOrDefault();

            ViewData["name"] = name.Patient.LastName + " " + name.Patient.FirstName;
            ViewData["diagnosis"] = name.Diagnosis.Name;



            return View(patientTreatment);
        }

        // POST: LDPatientTreatment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
            _context.PatientTreatment.Remove(patientTreatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientTreatmentExists(int id)
        {
            return _context.PatientTreatment.Any(e => e.PatientTreatmentId == id);
        }
    }
}
