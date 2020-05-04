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
    public class DoctorsController : Controller
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }
        private readonly db_oneTouchCareContext _context;
        private int AccountType_Doctor = 1;
        public DoctorsController(IHttpContextAccessor httpContextAccessor, db_oneTouchCareContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //only allowed for Doctors
        public IActionResult DoctorInfo()
        {
            string userName = new Encription().Decrypt(new ApplicationKeys().Key_UserName, _session.GetString("x").ToString());
            string password = new Encription().Decrypt(new ApplicationKeys().Key_Password, _session.GetString("y").ToString());
            bool IsExisted = _context.Account.Any(x => x.UserName.ToLower().Equals(userName) && x.Password.Equals(password));
            int? SelectedDoctorId = null;
            if (IsExisted)
            {
                //generate list of profession 
                var ListOfProfessions = _context.Profession.ToList();
                SelectList ProfessionList = new SelectList(ListOfProfessions, "ProfessionId", "Name");
                ViewData["ProfessionId"] = ProfessionList;

                //generate list of Nationality
                var listOfCountries = _context.Country.ToList();
                SelectList CountryList = new SelectList(listOfCountries, "CountryId", "Name");
                ViewData["Nationality"] = CountryList;

                Account account = _context.Account.Where(x => x.UserName.ToLower().Equals(userName) && x.Password.Equals(password)).FirstOrDefault();
                int? accountId = account.AccountId;
                //check if this account is for a doctor
                //if not return to the login page
                if (account.AccountTypeId != AccountType_Doctor)
                {
                    return RedirectToAction("Login", "Account");
                }
                //Check if the doctor has filled his info before

                bool IsDoctorExisted = _context.Doctor.Any(x => x.AccountId == accountId);
                // if doctor never filled his info before create neew one 
                if (!IsDoctorExisted)
                {
                    Doctor Thisdoctor = new Doctor();
                    Thisdoctor.AccountId = accountId;
                    _context.Doctor.Add(Thisdoctor);
                    _context.SaveChanges();
                    SelectedDoctorId = Thisdoctor.DoctorId;
                    return View(Thisdoctor);
                }
                // if doctor record is already created 
                // get account id of selected doctor
                else
                {
                    Doctor doctor = _context.Doctor.Where(x => x.AccountId == accountId).FirstOrDefault();
                    SelectedDoctorId = doctor.DoctorId;
                    return View(doctor);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult DoctorInfo([Bind("FirstName,LastName,DoctorId,Nationality,ProfessionId,AccountId")]Doctor doctor)
        {
            int? SelectedDoctorId = null;
            if (ModelState.IsValid)
            {
                Doctor oldDoctor = _context.Doctor.Where(x => x.AccountId == doctor.AccountId).FirstOrDefault();
                oldDoctor.FirstName = doctor.FirstName;
                oldDoctor.LastName = doctor.LastName;
                oldDoctor.Nationality = doctor.Nationality;
                oldDoctor.ProfessionId = doctor.ProfessionId;
                _context.SaveChanges();
                SelectedDoctorId = oldDoctor.DoctorId;
                return RedirectToAction("DoctorDetails", "Doctors", new { id = SelectedDoctorId });
            }
            return RedirectToAction("DoctorInfo", "Doctors");
        }
        

        public IActionResult DoctorDetails(int id)
        {
            int? SelectDoctorID = id;
            if (SelectDoctorID <= 0 || SelectDoctorID==null)
            {
                SelectDoctorID = new GlobalMethods(_context,_httpContextAccessor).GetDoctorId();
            }
            Doctor _doctor = _context.Doctor.Where(x => x.DoctorId == SelectDoctorID).Include(a => a.Account).Include(n => n.NationalityNavigation).Include(p => p.Profession).FirstOrDefault();
            string username = _doctor.Account.UserName;
            string profession = _doctor.Profession.Name;
            return View(_doctor);
        }

        [HttpPost]
        public void UploadFile([Bind("Image,IsProfile,DoctorId")]IFormFile Image,bool IsProfile, int DoctorId) 
        {
            if (IsProfile)
            {

            }

            if (Image.ContentType.Contains("image"))
            {
                using (var stream = new MemoryStream())
                {
                    Image.CopyTo(stream);
                    byte[] thisImage = stream.ToArray();
                    DoctorImages images = new DoctorImages()
                    {
                        Image = thisImage,
                        DoctorId = DoctorId,
                        IsProfile = IsProfile
                    };
                    _context.DoctorImages.Add(images);
                    _context.SaveChanges();
                }
            }
        }




    }
}