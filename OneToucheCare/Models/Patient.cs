using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneToucheCare.Models
{
    public partial class Patient
    {
        public Patient()
        {
            PatientDisease = new HashSet<PatientDisease>();
            PatientTest = new HashSet<PatientTest>();
            PatientDoctor = new HashSet<PatientDoctor>();
            PatientSurgeries = new HashSet<PatientSurgeries>();
            Appointments = new HashSet<Appointments>();
        }

        public int PatientId { get; set; }
        [Display(Name ="First name")]
        [Required]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [Display(Name ="Last name")]
        [Required]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        public int? Nationality { get; set; }
        [Display(Name ="Profile image")]
        public byte[] ProfileImage { get; set; }
        public int? AccountId { get; set; }
        [Display(Name ="Father name")]
        public string FatherName { get; set; }
        [Display(Name ="Mother name")]
        public string MotherName { get; set; }

        [NotMapped]
        
        [Display(Name ="User name or Email")]
        public string UserName { get; set; }
        [NotMapped]
        [Display(Name ="Password")]
        public string Password { get; set; }
        public Account Account { get; set; }
        public Country NationalityNavigation { get; set; }

        public ICollection<PatientDisease> PatientDisease { get; set; }
        public ICollection<PatientTest> PatientTest { get; set; }
        public ICollection<PatientDoctor> PatientDoctor { get; set; }
        public ICollection<PatientSurgeries> PatientSurgeries { get; set; }
        public ICollection<Appointments> Appointments { get; set; }
        
    }
}
