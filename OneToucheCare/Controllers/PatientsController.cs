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
    public class PatientsController : Controller
    {
        private readonly db_oneTouchCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }

        public PatientsController(db_oneTouchCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patient = await _context.Patient
                .Include(p => p.Account)
                .Include(p => p.NationalityNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
        [HttpPost]
        public IActionResult PatientListOfSelectedDoctor()
        {
            int? doctorId = new GlobalMethods(_context, _httpContextAccessor).GetDoctorId();
            if (doctorId != null && doctorId > 0)
            {
                var list = (from pd in _context.PatientDoctor
                            join p in _context.Patient on pd.PatientId equals p.PatientId
                            join d in _context.Doctor on pd.DoctorId equals d.DoctorId
                            where pd.DoctorId == doctorId
                            select new
                            {
                                pd.DoctorId,
                                pd.PatientId,
                                pd.PatientDoctorId,
                                p.LastName,
                                p.MotherName,
                                p.NationalityNavigation.Name,
                                p.ProfileImage,
                                p.FirstName,
                                p.FatherName
                            }).ToList();
                return Json(list);
            }
            return Json("");
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Account, "AccountId", "AccountId");
            ViewData["Nationality"] = new SelectList(_context.Country, "CountryId", "Name");
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("PatientId,FirstName,LastName,Nationality,AccountId,Image,FatherName,MotherName,UserName,Password")] Patient patient, IFormFile Image)
        {
            if (Image != null)
            {
                using (var stream = new MemoryStream())
                {
                    Image.CopyTo(stream);
                    byte[] ProfileImage = stream.ToArray();
                    patient.ProfileImage = ProfileImage;
                }
            }
            if (ModelState.IsValid)
            {
                //check if patient existed
                bool ExistedBefore = _context.Patient.Any(x =>
                x.FirstName.ToLower().TrimEnd().Equals(patient.FirstName.ToLower().TrimEnd())
                && x.LastName.ToLower().TrimEnd().Equals(patient.LastName.ToLower().TrimEnd())
                && x.MotherName.ToLower().TrimEnd().Equals(patient.MotherName.ToLower().TrimEnd())
                && x.FatherName.ToLower().TrimEnd().Equals(patient.FatherName.ToLower().TrimEnd()));

                //create account and generate a user name 
                bool IsAccountExisted = _context.Account.Any(x => x.UserName.ToLower().TrimEnd().Equals(patient.UserName.ToLower().TrimEnd()));
                int? AccountId = null;
                Account account = new Account()
                {
                    UserName = patient.UserName,
                    Password = patient.Password,
                    AccountTypeId = 2
                };

                _context.Account.Add(account);
                _context.SaveChanges();
                AccountId = account.AccountId;

                if (!ExistedBefore)
                {
                    patient.AccountId = AccountId;
                    _context.Add(patient);
                    _context.SaveChanges();
                    int? PatientId = patient.PatientId;
                    int? DoctorId = new GlobalMethods(_context, _httpContextAccessor).GetDoctorId();
                    bool PatientDoctorExisted = _context.PatientDoctor.Any(x => x.DoctorId == DoctorId && x.PatientId == PatientId);
                    if (!PatientDoctorExisted)
                    {
                        PatientDoctor patientDoctor = new PatientDoctor()
                        {
                            PatientId = PatientId,
                            DoctorId = DoctorId
                        };
                        _context.PatientDoctor.Add(patientDoctor);
                        _context.SaveChanges();
                    }
                    return RedirectToAction("Create");
                }
                else
                {
                    ViewData["FeedBack"] = "this account is already existed!";
                    return View(patient);
                }
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Account, "AccountId", "AccountId", patient.AccountId);
            ViewData["Nationality"] = new SelectList(_context.Country, "CountryId", "Name", patient.Nationality);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(int id, [Bind("PatientId,FirstName,LastName,Nationality,AccountId,Image,FatherName,MotherName")] Patient patient,IFormFile Image)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }
            if (Image != null)
            {
                using (var streamReader = new MemoryStream())
                {
                    Image.CopyTo(streamReader);
                    patient.ProfileImage = streamReader.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                Patient oldPatient = _context.Patient.Where(x => x.PatientId == patient.PatientId).FirstOrDefault();
                oldPatient.FatherName = patient.FatherName;
                oldPatient.FirstName = patient.FirstName;
                oldPatient.LastName = patient.LastName;
                oldPatient.MotherName = patient.MotherName;
                oldPatient.Nationality = patient.Nationality;
                oldPatient.Password = patient.Password;
                if (Image != null)
                {
                    oldPatient.ProfileImage = patient.ProfileImage;
                }
                _context.SaveChanges();
                return RedirectToAction("Create");
            }
            ViewData["AccountId"] = new SelectList(_context.Account, "AccountId", "AccountId", patient.AccountId);
            ViewData["Nationality"] = new SelectList(_context.Country, "CountryId", "CountryId", patient.Nationality);
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.Account)
                .Include(p => p.NationalityNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction("Create");
        }

        private bool PatientExists(int id)
        {
            return _context.Patient.Any(e => e.PatientId == id);
        }
    }
}
