using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OneToucheCare.Models;

namespace OneToucheCare.Controllers
{
    public class PatientDiseaseMedicamentController : Controller
    {
        private readonly db_oneTouchCareContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }

        public PatientDiseaseMedicamentController(db_oneTouchCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(int id)
        {
            SelectList MedicamentList = new SelectList(_context.Medicaments.ToList(), "MedicamentId", "Name");
            ViewData["MedicamentId"] = MedicamentList;
            PatientDiseaseMedicaments patientDiseaseMedicaments = new PatientDiseaseMedicaments()
            {
                PatientDiseaseId = id
            };
            IEnumerable<PatientDiseaseMedicaments> PatientDiseaseMedicaments = _context.PatientDiseaseMedicaments.Where(x => x.PatientDiseaseId == id).ToList();
            ViewData["Medicaments"] = PatientDiseaseMedicaments;
            return View(patientDiseaseMedicaments);
        }

        [HttpPost]
        public IActionResult Create(PatientDiseaseMedicaments patientDiseaseMedicaments)
        {
            //check if medicament existed 
            //if existed get his id 
            //if not add it to medicaments and add it to list 
            if (patientDiseaseMedicaments.MedicamentName== null)
            {
                return RedirectToAction("Create");
            }
            bool MedicamentExisted = _context.Medicaments.Any(x => x.Name.ToLower().Equals(patientDiseaseMedicaments.MedicamentName.ToLower()));
            if (MedicamentExisted)
            {
                patientDiseaseMedicaments.MedicamentId = _context.Medicaments.Where(x => x.Name.ToLower().Equals(patientDiseaseMedicaments.MedicamentName.ToLower())).FirstOrDefault().MedicamentId;
            }
            else
            {
                Medicaments medicaments = new Medicaments()
                {
                    Name = patientDiseaseMedicaments.MedicamentName
                };
                _context.Medicaments.Add(medicaments);
                _context.SaveChanges();
                patientDiseaseMedicaments.MedicamentId = medicaments.MedicamentId;
            }
            _context.PatientDiseaseMedicaments.Add(patientDiseaseMedicaments);
            _context.SaveChanges();
            return RedirectToAction("Create");
        }

        public IActionResult Edit(int id)
        {
            PatientDiseaseMedicaments patientDiseaseMedicaments = _context.PatientDiseaseMedicaments.Include(x => x.Medicament).Where(p => p.PatientDiseaseMedicamentId == id).FirstOrDefault();
            patientDiseaseMedicaments.MedicamentName = patientDiseaseMedicaments.Medicament.Name;
            return View(patientDiseaseMedicaments);
        }

        [HttpPost]
        public IActionResult Edit(PatientDiseaseMedicaments patientDiseaseMedicaments)
        {
            bool MedicamentExisted = _context.Medicaments.Any(x => x.Name.ToLower().Equals(patientDiseaseMedicaments.MedicamentName.ToLower()));
            if (MedicamentExisted)
            {
                patientDiseaseMedicaments.MedicamentId = _context.Medicaments.Where(x => x.Name.ToLower().Equals(patientDiseaseMedicaments.MedicamentName.ToLower())).FirstOrDefault().MedicamentId;
            }
            else
            {
                Medicaments medicaments = new Medicaments()
                {
                    Name = patientDiseaseMedicaments.MedicamentName
                };
                _context.Medicaments.Add(medicaments);
                _context.SaveChanges();
                patientDiseaseMedicaments.MedicamentId = medicaments.MedicamentId;
            }
            PatientDiseaseMedicaments oldPDM = _context.PatientDiseaseMedicaments.Where(x => x.PatientDiseaseMedicamentId == patientDiseaseMedicaments.PatientDiseaseMedicamentId).FirstOrDefault();
            oldPDM.MedicamentId = patientDiseaseMedicaments.MedicamentId;
            oldPDM.PatientDiseaseId = patientDiseaseMedicaments.PatientDiseaseId;
            oldPDM.TimesPerDay = patientDiseaseMedicaments.TimesPerDay;
            oldPDM.Dose = patientDiseaseMedicaments.Dose;
            oldPDM.DateStart = patientDiseaseMedicaments.DateStart;
            oldPDM.DateFinish = patientDiseaseMedicaments.DateFinish;
            _context.SaveChanges();
            return RedirectToAction("Create", new { id = oldPDM.PatientDiseaseId });
        }

        public IActionResult Delete(int id)
        {
            PatientDiseaseMedicaments patientDiseaseMedicaments = _context.PatientDiseaseMedicaments.Include(x => x.Medicament).Where(p => p.PatientDiseaseMedicamentId == id).FirstOrDefault();
            return View(patientDiseaseMedicaments);
        }

        [HttpPost]
        public IActionResult DeletePatientDiseaseMedicaments([Bind("PatientDiseaseMedicamentId")] int PatientDiseaseMedicamentId)
        {
            PatientDiseaseMedicaments patientDiseaseMedicaments = _context.PatientDiseaseMedicaments.Where(x => x.PatientDiseaseMedicamentId == PatientDiseaseMedicamentId).FirstOrDefault();
            int? patientDiseaseId = patientDiseaseMedicaments.PatientDiseaseId;
            _context.PatientDiseaseMedicaments.Remove(patientDiseaseMedicaments);
            _context.SaveChanges();
            return RedirectToAction("Create", new { id = patientDiseaseId });
        }

        [HttpPost]
        public JsonResult ListOfMedicaments()
        {
            var list = _context.Medicaments.Select(x => x.Name).ToList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetMedicamentOfSelectedPatientDisease([FromRoute] int id)
        {
            var list = _context.PatientDiseaseMedicaments.Where(x => x.PatientDiseaseId == id).ToList();
            return Json(list);
        }

        [HttpPost]
        public IActionResult GeneratePrescription([FromBody]string[] arrayOfMedicament)
        {
            return RedirectToAction("GeneratePrescriptions", new { id = arrayOfMedicament });
        }


        public IActionResult GeneratePrescriptions([FromQuery]string[] values)
        {
            List<int> MedicamentList = new List<int>();
            foreach (string patientDiseaseMedicamentId in values)
            {
                int.TryParse(patientDiseaseMedicamentId, out int PatientDiseaseMedicamentId);
                MedicamentList.Add(PatientDiseaseMedicamentId);
            }
            int[] MedicamentArray = MedicamentList.ToArray();
            int? patientDisease = _context.PatientDiseaseMedicaments.Where(x => x.PatientDiseaseMedicamentId == MedicamentArray[0]).FirstOrDefault().PatientDiseaseId;
            PatientDisease pd = _context.PatientDisease.Where(x => x.PatientDiseaseId == patientDisease).Include(x=>x.Doctor).ThenInclude(x=>x.Profession).FirstOrDefault();
            string DoctorName = "Doctor "+  pd.DoctorName + "("+ pd.Doctor.Profession.Name+ ")";
            ViewData["DoctorName"] = DoctorName;
            IEnumerable<PatientDiseaseMedicaments> MedicamentLists = _context.PatientDiseaseMedicaments.Include(x => x.Medicament).Where(x => MedicamentArray.Contains(x.PatientDiseaseMedicamentId)).ToList();
            return View(MedicamentLists);
        }


        
        [HttpPost]
        public void OpenBrowser([FromBody]string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }


    }
}