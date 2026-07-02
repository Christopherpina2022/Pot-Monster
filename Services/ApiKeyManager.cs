using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FGC_Stat_Analyzer_wpf.Services
{
    class ApiKeyManager
    {
        private static readonly string AppFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FGC-Stat-Analyzer-wpf");

        private static readonly string KeyFile = Path.Combine(AppFolder, "apikey.dat");


    }
}
