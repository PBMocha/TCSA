using GenetecChallenge.N1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenetecChallenge.N1.Extensions
{
    public static class LicenseExtensions
    {

        public static LicensePlateDTO ToDto(this LicensePlatePayload lp)
        {
            return new LicensePlateDTO
            {
                LicensePlate = lp.LicensePlate,
                LicensePlateCaptureTime = lp.LicensePlateCaptureTime,
                Longitude = lp.Longitude,
                Latitude = lp.Latitude,
                ContextImageReference = lp.ContextImageReference
            };
        }
    }
}
