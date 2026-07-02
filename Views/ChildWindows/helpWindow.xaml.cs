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
            "1+2 combos they don't break.",
            "times the Start.gg website went down causing the TO's to Pen and Paper that bitch.",
            "Leo Whitefang match-ups it takes.",
            "times they said they were busy but really just wanted to play Persona 3 Reload.",
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
