using System;
namespace ONVIF
{
    public static class Camera
    {
        public static string Manufacturer { get; set; }
        public static string Model { get; set; }        
        public static string IpAddress { get; set; }

        public static string Name { get {
                return $"{Manufacturer} {Model}";
            }
        }

        public static string GetRTSPAddress(string login, string password) {
            return $"rtsp://{login}:{password}@{IpAddress}:554/cam/realmonitor?channel=1&subtype=0";
        }
    }
}
