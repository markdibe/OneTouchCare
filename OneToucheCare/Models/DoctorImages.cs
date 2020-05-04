using System;
using System.Collections.Generic;

namespace OneToucheCare.Models
{
    public partial class DoctorImages
    {
        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public byte[] Image { get; set; }
        public bool? IsProfile { get; set; }
        public int? DoctorId { get; set; }

        public Doctor Doctor { get; set; }
    }
}
