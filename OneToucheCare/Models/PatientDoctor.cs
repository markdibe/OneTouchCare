using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneToucheCare.Models
{
    public class PatientDoctor
    {
        public int PatientDoctorId { get; set; }
        public int ?DoctorId { get; set; }
        public int ?PatientId { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }

    }
}
