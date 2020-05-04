using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneToucheCare.Models;

namespace OneToucheCare.Controllers
{
    public class DiseaseController : Controller
    {
        private readonly db_oneTouchCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }

        public DiseaseController(db_oneTouchCareContext context , IHttpContextAccessor httpContextAccessor)
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

        



        



    }
}