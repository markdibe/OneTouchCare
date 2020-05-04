using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneToucheCare.Models
{
    public class PatientSurgeries
    {
        public PatientSurgeries()
        {
            SurgeryImages = new HashSet<SurgeryImages>();
        }

        public int SurgeryId { get; set; }

        [Display(Name ="Surgery Name")]
        [StringLength(100)]
        [Required]
        public string SurgeryName { get; set; }

        [Display(Name ="Description")]
        [StringLength(300)]
        public string SurgeryDescription { get; set; }

        [Display(Name ="Result")]
        [StringLength(300)]
        public string SurgeryResult { get; set; }

        [Display(Name ="Date")]
        [DataType(DataType.Date)]
        public DateTime? SurgeryDate { get; set; }

        [Display(Name ="Type")]
        public SurgeryType SurgeryType { get; set; }


        public Doctor Doctor { get; set; }

        public Patient Patient { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }
        [Display(Name ="Doctor Name")]
        [StringLength(200)]
        public string DoctorName { get; set; }
        [Display(Name = "Type")]
        public int SurgeryTypeId { get; set; }

        [NotMapped]
        public IList<IFormFile> UploadedFiles { get; set; }

        public ICollection<SurgeryImages> SurgeryImages { get; set; }
    }
   
}
