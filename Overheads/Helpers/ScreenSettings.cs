using System.Windows.Media;
using Caliburn.Micro;

namespace Overheads.Helpers
{
    public class ScreenSettings : PropertyChangedBase
    {
        private SolidColorBrush _titleColor;
        private SolidColorBrush _foregroundColor;
        private SolidColorBrush _backgroundColor;
        private SolidColorBrush _searchTextColor;
        private SolidColorBrush _specialColor;
        private SolidColorBrush _instructionColor;
        private SolidColorBrush _subtitleColor;

        public SolidColorBrush SpecialColor
        {
            get { return _specialColor; }
            set
            {
                if (value.Equals(_specialColor)) return;
                _specialColor = value;
                NotifyOfPropertyChange(() => SpecialColor);
            }
        }

        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (value.Equals(_backgroundColor)) return;
                _backgroundColor = value;
                NotifyOfPropertyChange(() => BackgroundColor);
            }
        }

        public SolidColorBrush ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                if (value.Equals(_foregroundColor)) return;
                _foregroundColor = value;
                NotifyOfPropertyChange(() => ForegroundColor);
            }
        }

        public SolidColorBrush TitleColor
        {
            get { return _titleColor; }
            set
            {
                if (value.Equals(_titleColor)) return;
                _titleColor = value;
                NotifyOfPropertyChange(() => TitleColor);
            }
        }

        public SolidColorBrush SearchTextColor
        {
            get { return _searchTextColor; }
            set
            {
                if (Equals(value, _searchTextColor)) return;
                _searchTextColor = value;
                NotifyOfPropertyChange(() => SearchTextColor);
            }
        }

        public SolidColorBrush InstructionColor
        {
            get { return _instructionColor; }
            set
            {
                if (Equals(value, _instructionColor)) return;
                _instructionColor = value;
                NotifyOfPropertyChange(() => InstructionColor);
            }
        }

        public SolidColorBrush SubtitleColor
        {
            get { return _subtitleColor; }
            set
            {
                if (Equals(value, _subtitleColor)) return;
                _subtitleColor = value;
                NotifyOfPropertyChange(() => SubtitleColor);
            }
        }

        public ScreenSettings()
        {
            SetDarkTheme();
        }

        public void InvertColors()
        {
            if (BackgroundColor.Color == Colors.Black)
            {
                SetLightTheme();
            }
            else
            {
                SetDarkTheme();
            }
        }

        private void SetLightTheme()
        {
            BackgroundColor = new SolidColorBrush(Colors.WhiteSmoke);
            ForegroundColor = new SolidColorBrush(Colors.Black); 
            TitleColor = new SolidColorBrush(Colors.DarkGoldenrod);
            SearchTextColor = new SolidColorBrush(Colors.DarkSlateGray);
            SpecialColor = new SolidColorBrush(Color.FromRgb(255,153,255));
            SubtitleColor = new SolidColorBrush(Colors.Aquamarine);
            InstructionColor = new SolidColorBrush(Colors.Gray);
        }

        private void SetDarkTheme()
        {
            BackgroundColor = new SolidColorBrush(Colors.Black);
            ForegroundColor = new SolidColorBrush(Colors.WhiteSmoke); 
            TitleColor = new SolidColorBrush(Colors.Gold);
            SearchTextColor = new SolidColorBrush(Colors.Cyan);
            SpecialColor = new SolidColorBrush(Color.FromRgb(255, 153, 255));
            SubtitleColor = new SolidColorBrush(Colors.Aquamarine);
            InstructionColor = new SolidColorBrush(Colors.Gray);
        }
    }
}
