﻿using System.Windows.Media;
using Caliburn.Micro;

namespace Overheads.Helpers
{
    public class ScreenSettings : PropertyChangedBase
    {
        private SolidColorBrush _titleColor;
        private SolidColorBrush _foregroundColor;
        private SolidColorBrush _backgroundColor;
        private SolidColorBrush _searchTextColor;

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
        }

        private void SetDarkTheme()
        {
            BackgroundColor = new SolidColorBrush(Colors.Black);
            ForegroundColor = new SolidColorBrush(Colors.WhiteSmoke); 
            TitleColor = new SolidColorBrush(Colors.Gold);
            SearchTextColor = new SolidColorBrush(Colors.Cyan);
        }
    }
}