using System;
using System.Management;
using System.Windows.Media;
using AgnesAIImageEdit.Models;

namespace AgnesAIImageEdit.Services
{
    public static class GpuProbe
    {
        private static readonly string[] DenyList =
        {
            "intel hd graphics 2000",
            "intel hd graphics 3000",
            "intel hd graphics 4000"
        };

        public static void ApplyRenderingMode()
        {
            bool force = AppSettings.Current.AlwaysSoftwareRender;
            bool detected = false;

            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                foreach (ManagementObject mo in searcher.Get())
                {
                    var name = mo["Name"]?.ToString()?.ToLowerInvariant() ?? "";
                    foreach (var bad in DenyList)
                    {
                        if (name.Contains(bad)) { detected = true; break; }
                    }
                    if (detected) break;
                }
            }
            catch { /* WMI unavailable; fall back to tier check */ }

            bool lowTier = (RenderCapability.Tier >> 16) < 2;

            if (force || detected || lowTier)
            {
                System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Media.RenderMode.SoftwareOnly;
            }
        }
    }
}
