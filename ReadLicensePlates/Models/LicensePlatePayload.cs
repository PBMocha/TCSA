using System;
using System.Collections.Generic;
using System.Text;

namespace GenetecChallenge.N1.Models
{
    public class LicensePlatePayload
    {
        public DateTime LicensePlateCaptureTime { get; set; }
        public string LicensePlate { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string? ContextImageJpg { get; set; }
        public string? LicensePlateImageJpg { get; set; }
    }
}
