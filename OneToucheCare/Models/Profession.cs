using System;
using System.Collections.Generic;

namespace OneToucheCare.Models
{
    public partial class Profession
    {
        public Profession()
        {
            Doctor = new HashSet<Doctor>();
        }

        public int ProfessionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Doctor> Doctor { get; set; }
    }
}
