using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneToucheCare.Models
{
    public partial class PatientDiseaseMedicaments
    {
        public int PatientDiseaseMedicamentId { get; set; }
        public int? PatientDiseaseId { get; set; }
        public int? MedicamentId { get; set; }
        public string Dose { get; set; }
        [Display(Name ="Times per day")]
        public string TimesPerDay { get; set; }
        [Display(Name ="starting day")]
        public DateTime? DateStart { get; set; }
        [Display(Name ="finishing day")]
        public DateTime? DateFinish { get; set; }

        [NotMapped]
        [Display(Name ="Medicament Name")]
        public string MedicamentName { get; set; }

        public Medicaments Medicament { get; set; }
        public PatientDisease PatientDisease { get; set; }
    }
}
