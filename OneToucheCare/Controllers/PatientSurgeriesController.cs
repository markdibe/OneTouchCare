using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OneToucheCare.Models;

namespace OneToucheCare.Controllers
{
    public class PatientSurgeriesController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }
        private readonly db_oneTouchCareContext _context;

        public PatientSurgeriesController(db_oneTouchCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        //id as patient id
        public IActionResult Create(int id)
        {
            //get doctorID 
            GlobalMethods globalMethods = new GlobalMethods(_context, _httpContextAccessor);
            int DoctorId = 0;
            if (globalMethods.GetDoctorId().HasValue)
            {
                DoctorId = globalMethods.GetDoctorId().Value;
            }
            if (DoctorId > 0)
            {
                PatientSurgeries patientSurgeries = new PatientSurgeries()
                {
                    DoctorId = DoctorId,
                    PatientId = id,
                };
                IEnumerable<SurgeryType> SurgeryTypeList = _context.SurgeryType.ToList();
                SelectList SurgeryTypeId = new SelectList(SurgeryTypeList, "SurgeryTypeId", "SurgeryTypeName");
                ViewData["SurgeryTypeId"] = SurgeryTypeId;
                IEnumerable<PatientSurgeries> PatientSurgeryList = _context.PatientSurgeries.Include(x => x.Patient).Where(x => x.DoctorId == DoctorId && x.PatientId == id).ToList();
                ViewData["PatientSurgeryList"] = PatientSurgeryList;
                return View(patientSurgeries);
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public IActionResult Create(PatientSurgeries patientSurgeries)
        {
            int? SurgeryId = null;
            if (ModelState.IsValid)
            {
                _context.PatientSurgeries.Add(patientSurgeries);
                _context.SaveChanges();
                SurgeryId = patientSurgeries.SurgeryId;
            }
            if (SurgeryId != null && SurgeryId > 0)
            {
                if (patientSurgeries.UploadedFiles != null)
                {
                    foreach (IFormFile file in patientSurgeries.UploadedFiles)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            SurgeryImages surgeryImage = new SurgeryImages()
                            {
                                ImageName = file.FileName,
                                SurgeryImage = ms.ToArray(),
                                SurgeryId = SurgeryId
                            };
                            _context.SurgeryImages.Add(surgeryImage);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("Create");
        }

        //id is meant for surgery id
        public IActionResult Edit(int id)
        {
            PatientSurgeries patientSurgeries = _context.PatientSurgeries.Where(x => x.SurgeryId == id).FirstOrDefault();
            IEnumerable<SurgeryImages> surgeryImages = _context.SurgeryImages.Where(x => x.SurgeryId == id).ToList();
            SelectList SurgeryTypeList = new SelectList(_context.SurgeryType.ToList(), "SurgeryTypeId", "SurgeryTypeName");
            ViewData["SurgeryTypeId"] = SurgeryTypeList;
            ViewData["SurgeryImages"] = surgeryImages;
            return View(patientSurgeries);
        }

        [HttpPost]
        //id is for surgery id
        public JsonResult GetImages(int id)
        {
            List<SurgeryImages> surgeryImages = _context.SurgeryImages.Where(x => x.SurgeryId == id).ToList();
            return Json(surgeryImages);
        }

        [HttpPost]
        public IActionResult Edit(PatientSurgeries patientSurgeries)
        {
            if (ModelState.IsValid)
            {
                _context.Update(patientSurgeries);
                _context.SaveChanges();
            }
            else
            {
                return View(patientSurgeries);
            }
            int? SurgeryId = patientSurgeries.SurgeryId;
            if (patientSurgeries.UploadedFiles != null)
            {
                foreach (IFormFile file in patientSurgeries.UploadedFiles)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        SurgeryImages surgeryImage = new SurgeryImages()
                        {
                            ImageName = file.FileName,
                            SurgeryImage = ms.ToArray(),
                            SurgeryId = SurgeryId
                        };
                        _context.SurgeryImages.Add(surgeryImage);
                        _context.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Create", new { id = patientSurgeries.PatientId });
        }

        //id is for surgery Id
        public IActionResult Delete(int id)
        {
            PatientSurgeries surgeries = _context.PatientSurgeries.Where(x => x.SurgeryId == id).FirstOrDefault();
            return View(surgeries);
        }

        [HttpPost]
        public IActionResult ConfirmDelete([Bind("SurgeryId")] int SurgeryId)
        {
            bool existed = _context.PatientSurgeries.Any(x => x.SurgeryId == SurgeryId);
            int? patientId = null;
            if (existed)
            {
                PatientSurgeries patientSurgeries = _context.PatientSurgeries.Where(x => x.SurgeryId == SurgeryId).FirstOrDefault();
                patientId = patientSurgeries.PatientId;
                List<SurgeryImages> surgeryImages = _context.SurgeryImages.Where(x => x.SurgeryId == SurgeryId).ToList();
                _context.RemoveRange(surgeryImages);
                _context.SaveChanges();
                _context.Remove(patientSurgeries);
                _context.SaveChanges();
            }
            return RedirectToAction("Create", new { id = patientId });
        }


        [HttpPost]
        public void DeleteImage(int id)
        {
            SurgeryImages surgeryImages = _context.SurgeryImages.Where(x => x.SurgeryImageId == id).FirstOrDefault();
            _context.Remove(surgeryImages);
            _context.SaveChanges();
        }
        [HttpPost]
        public JsonResult GetSelectedImage(int id)
        {
            SurgeryImages surgeryImages = _context.SurgeryImages.Where(x => x.SurgeryImageId == id).FirstOrDefault();
            return Json(surgeryImages);
        }
    }
}