using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneToucheCare.Models
{
    public partial class PatientTest
    {
        public PatientTest()
        {
            PatientTestImages = new HashSet<PatientTestImages>();
        }

        public long PatientTestId { get; set; }
        [Display(Name ="Test Date")]
        public DateTime? TestDate { get; set; }
        [Display(Name ="Test name")]
        public string TestName { get; set; }
        [Display(Name ="Test type")]
        public int? TestTypeId { get; set; }
        [Display(Name ="Description")]
        public string Description { get; set; }
        public int? PatientId { get; set; }
        public string Result { get; set; }
        public string Deduction { get; set; }
        public int PatientDiseaseId { get; set; }

        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }

        
        public Patient Patient { get; set; }
        public TestType TestType { get; set; }
        public PatientDisease PatientDisease { get; set; }
        public ICollection<PatientTestImages> PatientTestImages { get; set; }
    }
}
