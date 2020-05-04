using System;
using System.Collections.Generic;

namespace OneToucheCare.Models
{
    public partial class Medicaments
    {
        public Medicaments()
        {
            PatientDiseaseMedicaments = new HashSet<PatientDiseaseMedicaments>();
        }

        public int MedicamentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<PatientDiseaseMedicaments> PatientDiseaseMedicaments { get; set; }
    }
}
