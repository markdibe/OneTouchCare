using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneToucheCare.Models
{
    public class SurgeryImages
    {
        public int SurgeryImageId { get; set; }

        public PatientSurgeries PatientSurgeries { get; set; }

        public int? SurgeryId { get; set; }

        public string ImageName { get; set; }

        public byte[] SurgeryImage { get; set; }
    }
}
