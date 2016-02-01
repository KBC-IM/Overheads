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

                var workingArea = primaryScreen.Bounds;

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

                var workingArea = secondaryScreen.Bounds;

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

                var screen = System.Windows.Forms.Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();
                if (Properties.Settings.Default.MaximizeToSecondary)
                    screen = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

                var workingArea = screen.Bounds;

                window.Left = workingArea.Left;
                window.Top = workingArea.Top;
                window.Width = workingArea.Width;
                window.Height = workingArea.Height;
            }
            window.Topmost = true;
        }

        public static System.Windows.Forms.IWin32Window GetIWin32Window(this System.Windows.Media.Visual visual)
        {
            var source = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }
    }
}
