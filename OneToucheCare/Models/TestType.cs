using System;
using System.Collections.Generic;

namespace OneToucheCare.Models
{
    public partial class TestType
    {
        public TestType()
        {
            PatientTest = new HashSet<PatientTest>();
        }

        public int TestTypeId { get; set; }
        public string TestTypeName { get; set; }

        public ICollection<PatientTest> PatientTest { get; set; }
    }
}
