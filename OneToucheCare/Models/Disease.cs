using System;
using System.Collections.Generic;

namespace OneToucheCare.Models
{
    public partial class Disease
    {
        public Disease()
        {
            PatientDisease = new HashSet<PatientDisease>();
        }

        public int DiseaseId { get; set; }
        public string DiseaseName { get; set; }

        public ICollection<PatientDisease> PatientDisease { get; set; }
    }
}
