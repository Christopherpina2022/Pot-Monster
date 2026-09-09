using System.Windows;

namespace FGC_Stat_Analyzer_wpf.Views.ChildWindows
{
    public partial class helpWindow : Window
    {
        private static readonly string[] FunMessages =
        {
            "DOOM footdive Combos they keep dropping.",
            "GIFs they post of their TO drinking SunnyD which gets them 5.",
            "Dragon Installs they accidently trigger.",
            "Twitlongers they were specifically mentioned in.",
            "1+2 combos they don't break.",
            "Leo Whitefang match-ups it takes.",
            "Raw CA's they throw on the start of phase shift",
            "times they ducked on a local to play that fucking RPG again.",
            "DLC characters they buy, yet are never seen playing them.",
            "0-2s.",
            "TJUs they throw knowing damn well they won't hit it."
        };

        private static readonly Random Random = new();

        public helpWindow()
        {
            InitializeComponent();

            punchLine.Text = FunMessages[Random.Next(FunMessages.Length)];
        }
    }
}
