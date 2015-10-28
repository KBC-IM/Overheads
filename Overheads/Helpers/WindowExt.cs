using System.Linq;
using System.Windows;

namespace Overheads.Helpers
{
    static public class WindowExt
    {
        public static void MaximizeToPrimary(this Window window)
        {
            var primaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

            if (primaryScreen != null)
            {
                if (!window.IsLoaded)
                    window.WindowStartupLocation = WindowStartupLocation.Manual;

                var workingArea = primaryScreen.WorkingArea;

                window.Left = workingArea.Left;
                window.Top = workingArea.Top;
                window.Width = workingArea.Width;
                window.Height = workingArea.Height;

                if (window.IsLoaded)
                {
                    window.WindowState = WindowState.Maximized;
                    window.WindowStyle = WindowStyle.None;
                }
            }
        }
        public static void MaximizeToSecondary(this Window window)
        {
            var secondaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                if (!window.IsLoaded)
                    window.WindowStartupLocation = WindowStartupLocation.Manual;

                var workingArea = secondaryScreen.WorkingArea;

                window.Left = workingArea.Left;
                window.Top = workingArea.Top;
                window.Width = workingArea.Width;
                window.Height = workingArea.Height;

                if (window.IsLoaded)
                {
                    window.WindowState = WindowState.Maximized;
                    window.WindowStyle = WindowStyle.None;
                }
            }
        }
        public static void ToggleFullscreen(this Window window)
        {
            if (Properties.Settings.Default.Fullscreen)
            {
                window.WindowState = WindowState.Maximized;
                window.WindowStyle = WindowStyle.None;
                Properties.Settings.Default.Fullscreen = false;
            }
            else
            {
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                Properties.Settings.Default.Fullscreen = true;
            }
        }
    }
}
