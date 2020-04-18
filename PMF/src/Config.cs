﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PMF
{
    public static class Config
    {
        public static string ManifestFileName = "manifest.json";

        public static string PackageInstallationFolder { get; set; }

        public static string RepositoryEndpoint { get; set; }

        public static Version CurrentSdkVersion { get; set; }

        public static bool IsDebugging { get; set; }

        public static string TemporaryFolder = ".pmf-temp";

        public static void DEBUG(string message)
        {
            Console.WriteLine("DEBUG: " + message);
        }
    }
}
