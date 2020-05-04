using System;
using System.Collections.Generic;

namespace OneToucheCare.Models
{
    public partial class Country
    {
        public Country()
        {
            Doctor = new HashSet<Doctor>();
            Patient = new HashSet<Patient>();
        }

        public int CountryId { get; set; }
        public string Name { get; set; }

        public ICollection<Doctor> Doctor { get; set; }
        public ICollection<Patient> Patient { get; set; }
    }
}
