using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public static class ColorPaletteManager
    {
        private static ResourceDictionary? _activePalette;
        private static readonly Dictionary<string, Uri> Palettes = new()
        {
            ["Citrus"] = new Uri("/Styles/Palettes/CitrusPalette.xaml", UriKind.Relative),
            ["Pastel"] = new Uri("/Styles/Palettes/PastelPalette.xaml", UriKind.Relative)
        };

        public static void ApplyPalette(string paletteName)
        {
            if (!Palettes.TryGetValue(paletteName, out Uri? paletteUri)) 
            {
                return;
            }

            var dictionaries = Application.Current.Resources.MergedDictionaries;

            // Remove previous palette
            if (_activePalette != null) 
            {
                dictionaries.Remove(_activePalette);
            }

            // Load new palette
            _activePalette = new ResourceDictionary
            {
                Source = paletteUri
            };

            dictionaries.Add(_activePalette);
        }
    }
}
