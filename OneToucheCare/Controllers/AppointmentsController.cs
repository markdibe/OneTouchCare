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
    public class AppointmentsController : Controller
    {
        private readonly db_oneTouchCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }
        private readonly GlobalMethods _global;

        public AppointmentsController(db_oneTouchCareContext context, IHttpContextAccessor httpContextAccessor, GlobalMethods global)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _global = global;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Doctor_Create()
        {
            int? doctorId = _global.GetDoctorId();
            if (doctorId != null)
            {
                Appointments appointments = new Appointments()
                {
                    DoctorId = doctorId
                };
                var list = (from pd in _context.PatientDoctor
                            join d in _context.Doctor on pd.DoctorId
                            equals d.DoctorId
                            join p in _context.Patient
                            on pd.PatientId equals p.PatientId
                            select new
                            {
                                PatientId = pd.PatientId,
                                PatientName = p.FirstName + " " + p.FatherName + " " + p.LastName
                            }).ToList();
                SelectList PatientList = new SelectList(list, "PatientId", "PatientName");
                ViewData["PatientId"] = PatientList;
                return View(appointments);
            }
            return RedirectToAction("login", "account");
        }


    }
}