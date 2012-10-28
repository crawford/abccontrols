using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Crawford.Controls
{
    public partial class AbcImage : UserControl
    {
        public AbcImage()
        {
            InitializeComponent();
            var b = this;
            _image.ImageFailed += (sender, e) =>
            {
                var a = 0;
            };
            _image.ImageOpened += (sender, e) =>
            {
                OnLoadOverride(((Image)sender).Source);
            };
        }

        virtual public void OnLoadOverride(ImageSource b) { }

        protected override Size MeasureOverride(Size availableSize)
        {
            double origWidth = (Double)GetValue(OriginalWidthProperty);
            double origHeight = (Double)GetValue(OriginalHeightProperty);

            Size desired = new Size(origWidth, origHeight);

            if (origWidth > availableSize.Width)
            {
                desired.Height *= (availableSize.Width / origWidth);
                desired.Width = availableSize.Width;
            }
            if (desired.Height > availableSize.Height)
            {
                desired.Width *= (availableSize.Height / desired.Height);
                desired.Height = availableSize.Height;
            }

            _image.Measure(desired);
            if (!desired.Width.IsNumeric() || !desired.Width.IsNumeric())
                desired = _image.DesiredSize;

            return desired;
        }

        #region Properties

        public static readonly DependencyProperty OriginalHeightProperty = DependencyProperty.Register("OriginalHeight", typeof(Double), typeof(AbcImage), null);
        public static readonly DependencyProperty OriginalWidthProperty = DependencyProperty.Register("OriginalWidth", typeof(Double), typeof(AbcImage), null);
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(AbcImage), new PropertyMetadata(OnSourceChanged));
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(AbcImage), new PropertyMetadata(OnStretchChanged));

        public Double OriginalHeight
        {
            get
            {
                return (Double)GetValue(OriginalHeightProperty);
            }
            set
            {
                SetValue(OriginalHeightProperty, value);
                InvalidateMeasure();
            }
        }

        public Double OriginalWidth
        {
            get
            {
                return (Double)GetValue(OriginalWidthProperty);
            }
            set
            {
                SetValue(OriginalWidthProperty, value);
                InvalidateMeasure();
            }
        }

        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
                _image.Source = value;
            }
        }

        public Stretch Stretch
        {
            get
            {
                return _image.Stretch;
            }
            set
            {
                _image.Stretch = value;
            }
        }

        public static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((AbcImage)sender).Source = (ImageSource)e.NewValue;
        }

        public static void OnStretchChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((AbcImage)sender).Stretch = (Stretch)e.NewValue;
        }

        #endregion
    }
}
