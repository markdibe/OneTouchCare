using System;
using System.Collections.Generic;

namespace OneToucheCare.Models
{
    public class SurgeryType
    {
        public SurgeryType()
        {
            PatientSurgeries = new HashSet<PatientSurgeries>();
        }

        public int SurgeryTypeId { get; set; }
        public string SurgeryTypeName { get; set; }
        public string SurgeryTypeDescription { get; set; }
        public string SocialSecurityNumber { get; set; }

        public ICollection<PatientSurgeries> PatientSurgeries { get; set; }
    }
}