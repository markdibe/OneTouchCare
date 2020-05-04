using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneToucheCare.Models
{
    public partial class Doctor
    {
        public Doctor()
        {
            DoctorImages = new HashSet<DoctorImages>();
            PatientDoctor = new HashSet<PatientDoctor>();
            PatientDisease = new HashSet<PatientDisease>();
            PatientSurgeries = new HashSet<PatientSurgeries>();
            Appointments = new HashSet<Appointments>();
        }

        public int DoctorId { get; set; }
        [Display(Name ="First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name ="Last Name")]
        [Required]
        public string LastName { get; set; }

        public int? Nationality { get; set; }
        [Display(Name ="Profession")]
        public int? ProfessionId { get; set; }
        public int? AccountId { get; set; }

        public Account Account { get; set; }
        public Country NationalityNavigation { get; set; }
        public Profession Profession { get; set; }
        public ICollection<DoctorImages> DoctorImages { get; set; }
        public ICollection<PatientDoctor> PatientDoctor { get; set; }
        public ICollection<PatientDisease> PatientDisease { get; set; }
        public ICollection<PatientSurgeries> PatientSurgeries { get; set; }
        public ICollection<Appointments> Appointments { get; set; }
    }
}
