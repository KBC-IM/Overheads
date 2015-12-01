using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Overheads
{
    /// <summary>
    /// Enum for specifying where the ellipsis should appear.
    /// </summary>
    public enum EllipsisPlacement
    {
        Left,
        Center,
        Right
    }

    /// <summary>
    /// This is a subclass of TextBox with the ability to show an ellipsis 
    /// when the Text doesn't fit in the visible area.
    /// </summary>
    public class TextBoxWithEllipsis : System.Windows.Controls.TextBox
    {
        // Constructor
        public TextBoxWithEllipsis()
        {
            // Initialize inherited stuff as desired.
            IsReadOnlyCaretVisible = true;

            // Initialize stuff added by this class
            IsEllipsisEnabled = true;
            UseLongTextForToolTip = true;
            FudgePix = 3.0;
            _placement = EllipsisPlacement.Right;
            _internalEnabled = true;

            LayoutUpdated += new EventHandler(TextBoxWithEllipsis_LayoutUpdated);
            SizeChanged += new SizeChangedEventHandler(TextBoxWithEllipsis_SizeChanged);
        }

        /// <summary>
        /// The underlying text that gets truncated with ellipsis if it doesn't fit.
        /// Setting this and setting Text has the same effect, but getting Text may
        /// get a truncated version of LongText.
        /// </summary>
        public string LongText
        {
            get { return _longText; }

            set
            {
                _longText = value ?? "";
                PrepareForLayout();
            }
        }

        public EllipsisPlacement EllipsisPlacement
        {
            get { return _placement; }

            set
            {
                if (_placement != value)
                {
                    _placement = value;

                    if (_DoEllipsis)
                    {
                        PrepareForLayout();
                    }
                }
            }
        }

        /// <summary>
        /// If true, Text/LongText will be truncated with ellipsis
        /// to fit in the visible area of the TextBox
        /// (except when it has the focus).
        /// </summary>
        public bool IsEllipsisEnabled
        {
            get { return _externalEnabled; }

            set
            {
                _externalEnabled = value;
                PrepareForLayout();

                if (_DoEllipsis)
                {
                    // Since we didn't change Text or Size, layout wasn't performed 
                    // as a side effect.  Pretend that it was.
                    TextBoxWithEllipsis_LayoutUpdated(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// If true, ToolTip will be set to LongText whenever
        /// LongText doesn't fit in the visible area.  
        /// If false, ToolTip will be set to null unless
        /// the user sets it to something other than LongText.
        /// </summary>
        public bool UseLongTextForToolTip
        {
            get { return _useLongTextForToolTip; }

            set
            {
                if (_useLongTextForToolTip != value)
                {
                    _useLongTextForToolTip = value;

                    if (value)
                    {
                        // When turning it on, set ToolTip to
                        // _longText if the current Text is too long.
                        if (ExtentWidth > ViewportWidth ||
                            Text != _longText)
                        {
                            ToolTip = _longText;
                        }
                    }
                    else
                    {
                        // When turning it off, set ToolTip to null
                        // unless user has set it to something other
                        // than _longText;
                        if (_longText.Equals(ToolTip))
                        {
                            ToolTip = null;
                        }
                    }
                }
            }
        }

        public double FudgePix
        {
            get;
            set;
        }

        // Last length of substring of LongText known to fit.
        // Used while calculating the correct length to fit.
        private int _lastFitLen = 0;

        // Last length of substring of LongText known to be too long.
        // Used while calculating the correct length to fit.
        private int _lastLongLen;

        // Length of substring of LongText currently assigned to the Text property.
        // Used while calculating the correct length to fit.
        private int _curLen;

        // Used to detect whether the OnTextChanged event occurs due to an
        // external change vs. an internal one.
        private bool _externalChange = true;

        // Used to disable ellipsis internally (primarily while
        // the control has the focus).
        private bool _internalEnabled = true;

        // Backer for LongText.
        private string _longText = "";

        // Backer for IsEllipsisEnabled
        private bool _externalEnabled = true;

        // Backer for UseLongTextForToolTip.
        private bool _useLongTextForToolTip;

        // Backer for EllipsisPlacement
        private EllipsisPlacement _placement;

        // OnTextChanged is overridden so we can avoid 
        // raising the TextChanged event when we change 
        // the Text property internally while searching 
        // for the longest substring that fits.
        // If Text is changed externally, we copy the
        // new Text into LongText before we overwrite Text 
        // with the truncated version (if IsEllipsisEnabled).
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (_externalChange)
            {
                _longText = Text ?? "";
                if (UseLongTextForToolTip) ToolTip = _longText;
                PrepareForLayout();
                base.OnTextChanged(e);
            }
        }

        // Makes the entire text available for editing, selecting, and scrolling
        // until focus is lost.
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            _internalEnabled = false;
            SetText(_longText);
            base.OnGotKeyboardFocus(e);
        }

        // Returns to trimming and showing ellipsis.
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            _internalEnabled = true;
            PrepareForLayout();
            base.OnLostKeyboardFocus(e);
        }

        // Sets the Text property without raising the TextChanged event.
        private void SetText(string text)
        {
            if (Text != text)
            {
                _externalChange = false;
                Text = text; // Will trigger Layout event.
                _externalChange = true;
            }

        }

        // Arranges for the next LayoutUpdated event to trim _longText and add ellipsis.
        // Also triggers layout by setting Text.
        private void PrepareForLayout()
        {
            _lastFitLen = 0;
            _lastLongLen = _longText.Length;
            _curLen = _longText.Length;

            // This raises the LayoutUpdated event, whose
            // handler does the ellipsis.
            SetText(_longText);
        }

        private void TextBoxWithEllipsis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_DoEllipsis && e.NewSize.Width != e.PreviousSize.Width)
            {
                // We need to recalculate the longest substring of LongText that will fit (with ellipsis).
                // Prepare for the LayoutUpdated event, which does the recalc and is raised after this.
                PrepareForLayout();
            }
        }

        private bool _DoEllipsis { get { return IsEllipsisEnabled && _internalEnabled; } }

        // Called when Text or Size changes (and maybe at other times we don't care about).
        private void TextBoxWithEllipsis_LayoutUpdated(object sender, EventArgs e)
        {
            if (_DoEllipsis)
            {
                // This does a binary search (bisection) to determine the maximum substring
                // of _longText that will fit in visible area.  Instead of a loop, it
                // uses a type of recursion that happens because this event is raised
                // again if we set the Text property in here.

                if (ViewportWidth + FudgePix < ExtentWidth)
                {
                    // The current Text (whose length without ellipsis is _curLen) is too long.
                    _lastLongLen = _curLen;
                }
                else
                {
                    // The current Text is not too long.
                    _lastFitLen = _curLen;
                }

                // Try a new substring whose length is halfway between the last length
                // known to fit and the last length known to be too long.
                int newLen = (_lastFitLen + _lastLongLen) / 2;

                if (_curLen == newLen)
                {
                    // We're done! Usually, _lastLongLen is _lastFitLen + 1.

                    if (UseLongTextForToolTip)
                    {
                        if (Text == _longText)
                        {
                            ToolTip = null;
                        }
                        else
                        {
                            ToolTip = _longText;
                        }
                    }
                }
                else
                {
                    _curLen = newLen;

                    // This sets the Text property without raising the TextChanged event.
                    // However it does raise the LayoutUpdated event again, though
                    // not recursively.
                    CalcText();
                }
            }
            else if (UseLongTextForToolTip)
            {
                if (ViewportWidth < ExtentWidth)
                {
                    // The current Text is too long.
                    ToolTip = _longText;
                }
                else
                {
                    // The current Text is not too long.
                    ToolTip = null;
                }
            }
        }

        // Sets Text to a substring of _longText based on _placement and _curLen.
        private void CalcText()
        {
            switch (_placement)
            {
                case EllipsisPlacement.Right:
                    SetText(_longText.Substring(0, _curLen) + "\u2026");
                    break;

                case EllipsisPlacement.Center:
                    int firstLen = _curLen / 2;
                    int secondLen = _curLen - firstLen;
                    SetText(_longText.Substring(0, firstLen) + "\u2026" + _longText.Substring(_longText.Length - secondLen));
                    break;

                case EllipsisPlacement.Left:
                    int start = _longText.Length - _curLen;
                    SetText("\u2026" + _longText.Substring(start));
                    break;

                default:
                    throw new Exception("Unexpected switch value: " + _placement.ToString());
            }
        }
    }
}
