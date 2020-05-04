using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneToucheCare.Models
{
    public partial class PatientDisease
    {
        public PatientDisease()
        {
            PatientDiseaseMedicaments = new HashSet<PatientDiseaseMedicaments>();
            PatientTest = new HashSet<PatientTest>();
        }

        public int PatientDiseaseId { get; set; }
        public int? PatientId { get; set; }
        [Display(Name ="Date of disease")]
        public DateTime? DateOfDisease { get; set; }
        [Display(Name ="Disease Name")]
        public int? DiseaseId { get; set; }
        [Display(Name ="Description")]
        public string Description { get; set; }
        [Display(Name ="Finished from Disease?")]
        public bool FinishedFromDisease { get; set; }
        [Display(Name ="Healing Date")]
        public DateTime? DateOfFinish { get; set; }
        public int? DoctorId { get; set; }
        [Display(Name ="Doctor Name")]
        public string DoctorName { get; set; }

        public Disease Disease { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public ICollection<PatientDiseaseMedicaments> PatientDiseaseMedicaments { get; set; }
        public ICollection<PatientTest> PatientTest { get; set; }
    }
}
