using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneToucheCare.Models
{
    public class Appointments
    {
        public int AppointmentId { get; set; }
        [Required]
        [Display(Name = "Appointment date")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Patient Name")]
        public string PatientFullName { get; set; }
        [Display(Name = "title")]
        [StringLength(200)]
        public string Title { get; set; }
        [Display(Name = "Subject")]
        [StringLength(500)]
        public string Subject { get; set; }
        [Required]
        public int ?DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int? PatientId { get; set; }
        public Patient Patient { get; set; }

        public bool IsRequest { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
