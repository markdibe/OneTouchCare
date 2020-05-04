using System;
using System.Collections.Generic;

namespace OneToucheCare.Models
{
    public partial class PatientTestImages
    {
        public long PatientTestImageId { get; set; }
        public byte[] Image { get; set; }
        public long? PatientTestId { get; set; }

        public PatientTest PatientTest { get; set; }
    }
}
