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
    public class TestsController : Controller
    {
        private readonly db_oneTouchCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }


        public TestsController(db_oneTouchCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        //check if the test is related to a patient disease 
        //test can be independent or related to a patient disease 
        //when it is related the PatientDisease Value will the id of patient disease
        //otherwise it will be empty
        //if it is related to a disease we have to get all the past tests related to this disease

        public IActionResult Create(int id, [FromQuery(Name = "PatientDisease")]string PatientDisease)
        {
            var TestTypeList = _context.TestType.ToList();
            SelectList TestTypeId = new SelectList(TestTypeList, "TestTypeId", "TestTypeName");
            ViewData["TestTypeId"] = TestTypeId;

            PatientTest patientTest = new PatientTest() { PatientId = id };
            if (PatientDisease != null)
            {
                int.TryParse(PatientDisease, out int patientDiseaseID);
                patientTest.PatientDiseaseId = patientDiseaseID;
                IEnumerable<PatientTest> PatientTestList = _context.PatientTest.Include(x => x.TestType).Include(x => x.Patient).Where(x => x.PatientDiseaseId == patientTest.PatientDiseaseId).ToList();
                ViewData["PatientTestList"] = PatientTestList;
            }
            else
            {
                IEnumerable<PatientTest> PatientTestList = _context.PatientTest.Include(x => x.TestType).Include(x => x.Patient).Where(x => x.PatientId == id).ToList();
                ViewData["PatientTestList"] = PatientTestList;
            }
            return View(patientTest);
        }


        //1 - add new patient test
        //2 - take patient test id 
        //3 - check if file list contains images
        //4 -if yes save them into patient test images
        [HttpPost]
        public IActionResult Create(PatientTest patientTest)
        {
            List<IFormFile> ImageList = patientTest.ImageFiles;

            if (ModelState.IsValid)
            {
                _context.PatientTest.Add(patientTest);
                _context.SaveChanges();

                long patientTestId = patientTest.PatientTestId;
                //int? patientDiseaseId = patientTest.PatientDiseaseId;
                //int? patientId = patientTest.PatientId;
                if (ImageList != null)
                {
                    foreach (IFormFile image in ImageList)
                    {
                        if (image != null && image.ContentType.Contains("image"))
                        {
                            using (var stream = new MemoryStream())
                            {
                                image.CopyTo(stream);
                                byte[] Image = stream.ToArray();
                                PatientTestImages patientTestImages = new PatientTestImages();
                                patientTestImages.Image = Image;
                                patientTestImages.PatientTestId = patientTestId;
                                _context.PatientTestImages.Add(patientTestImages);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
                return RedirectToAction("Create", new { id = patientTest.PatientId, PatientDisease = patientTest.PatientDiseaseId.ToString() });
            }
            else
            {
                return View(patientTest);
            }
        }

        //Get Patient test id parameter 
        //get all images related to this patient test from database 
        [HttpPost]
        public JsonResult GetImages([FromRoute] int id)
        {
            if (id > 0)
            {
                var list = _context.PatientTestImages.Where(x => x.PatientTestId == id).ToList();
                return Json(list);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult GetSelectedImage([FromRoute]int id)
        {
            if (id > 0)
            {
                PatientTestImages patientTestImages = _context.PatientTestImages.Where(x => x.PatientTestImageId == id).FirstOrDefault();
                return Json(patientTestImages);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        public void DeleteThisImage([FromRoute] int id)
        {
            bool existed = _context.PatientTestImages.Any(x => x.PatientTestImageId == id);
            if (existed)
            {
                PatientTestImages p = _context.PatientTestImages.Where(x => x.PatientTestImageId == id).FirstOrDefault();
                _context.Remove(p);
                _context.SaveChanges();
            }
        }

        //Get patient test id from route
        public IActionResult Edit(int id)
        {
            bool existed = _context.PatientTest.Any(x => x.PatientTestId == id);
            if (existed)
            {
                SelectList TestTypeList = new SelectList(_context.TestType.ToList(), "TestTypeId", "TestTypeName");
                ViewData["TestTypeId"] = TestTypeList;
                IEnumerable<PatientTestImages> PatientTestImageList = _context.PatientTestImages.Where(x => x.PatientTestId == id).ToList();
                ViewData["ImageList"] = PatientTestImageList;
                PatientTest patientTest = _context.PatientTest.Where(x => x.PatientTestId == id).FirstOrDefault();
                return View(patientTest);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Edit(PatientTest patientTest)
        {
            List<IFormFile> ImageList = patientTest.ImageFiles;
            long? patientTestID = patientTest.PatientTestId;
            if (ImageList != null)
            {
                foreach (IFormFile image in ImageList)
                {
                    if (image != null && image.ContentType.Contains("image"))
                    {
                        using (MemoryStream memory = new MemoryStream())
                        {
                            image.CopyTo(memory);
                            PatientTestImages patientTestImages = new PatientTestImages()
                            {
                                PatientTestId = patientTestID,
                                Image = memory.ToArray()
                            };
                            _context.PatientTestImages.Add(patientTestImages);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            _context.Update(patientTest);
            _context.SaveChanges();
            return RedirectToAction("Create", new { id = patientTest.PatientId, PatientDisease = patientTest.PatientDiseaseId.ToString() });
        }
    }
}