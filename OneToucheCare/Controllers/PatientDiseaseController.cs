using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OneToucheCare.Models;

namespace OneToucheCare.Controllers
{
    public class PatientDiseaseController : Controller
    {
        private readonly db_oneTouchCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }

        public PatientDiseaseController(db_oneTouchCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index(int id)
        {
            return View();
        }


        public IActionResult Create(int id)
        {
            SelectList DiseaseList = new SelectList(_context.Disease.ToList(), "DiseaseId", "DiseaseName");
            ViewData["DiseaseId"] = DiseaseList;
            GlobalMethods global = new GlobalMethods(_context, _httpContextAccessor);
            int? DoctorId = global.GetDoctorId();
            PatientDisease patientDisease = new PatientDisease()
            {
                PatientId = id,
                DoctorId = DoctorId
            };
            return View(patientDisease);
        }

        [HttpPost]
        public IActionResult Create(PatientDisease patientDisease)
        {
            _context.PatientDisease.Add(patientDisease);
            _context.SaveChanges();
            return RedirectToAction("Create");
        }

        [HttpPost]
        public JsonResult GetDiseasesOfSelectedPatient([FromRoute]int id)
        {
            var list = (from pd in _context.PatientDisease
                        join d in _context.Disease on pd.DiseaseId equals d.DiseaseId
                        join dc in _context.Doctor on pd.DoctorId equals dc.DoctorId
                        select new
                        {
                            pd.DateOfDisease,
                            pd.DateOfFinish,
                            pd.Description,
                            pd.DiseaseId,
                            pd.DoctorId,
                            pd.DoctorName,
                            pd.PatientDiseaseId,
                            pd.PatientId,
                            d.DiseaseName,
                            ThisDoctor = dc.FirstName + " " + dc.LastName
                        }).ToList();
            return Json(list);
        }

        public IActionResult Edit(int id)
        {
            SelectList DiseaseList = new SelectList(_context.Disease.ToList(), "DiseaseId", "DiseaseName");
            ViewData["DiseaseId"] = DiseaseList;
            PatientDisease patientDisease = _context.PatientDisease.Where(x => x.PatientDiseaseId == id).FirstOrDefault();
            return View(patientDisease);
        }
        [HttpPost]
        public IActionResult Edit(PatientDisease patientDisease)
        {
            PatientDisease oldPatientDisease = _context.PatientDisease.Where(x => x.PatientDiseaseId == patientDisease.PatientDiseaseId).FirstOrDefault();
            oldPatientDisease.DateOfDisease = patientDisease.DateOfDisease;
            oldPatientDisease.DateOfFinish = patientDisease.DateOfFinish;
            oldPatientDisease.Description = patientDisease.Description;
            oldPatientDisease.DiseaseId = patientDisease.DiseaseId;
            oldPatientDisease.DoctorName = patientDisease.DoctorName;
            oldPatientDisease.FinishedFromDisease = patientDisease.FinishedFromDisease;
            oldPatientDisease.PatientId = patientDisease.PatientId;
            oldPatientDisease.DoctorId = patientDisease.DoctorId;
            _context.SaveChanges();
            return RedirectToAction("Create");
        }

        


    }
}