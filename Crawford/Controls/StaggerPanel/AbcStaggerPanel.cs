using System;
using System.Windows;
using System.Windows.Controls;

namespace Crawford.Controls

{
    public partial class StaggerPanel : Panel
    {
        #region Properties

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(StaggerPanel), new PropertyMetadata(double.NaN, OnDimensionChanged));
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(StaggerPanel), new PropertyMetadata(double.NaN, OnDimensionChanged));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StaggerPanel),  new PropertyMetadata(Orientation.Vertical, OnOrientationPropertyChanged));

        private bool _ignoreDimensionChange;

        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }
            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        public double ItemWidth
        {
            get
            {
                return (double)GetValue(ItemWidthProperty);
            }
            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private static void OnOrientationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            StaggerPanel panel = (StaggerPanel)sender;
            panel.InvalidateMeasure();
        }

        private static void OnDimensionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            StaggerPanel panel = (StaggerPanel)sender;
            if (panel._ignoreDimensionChange)
                return;

            
            double value = (double)e.NewValue;
            if (value <= 0.0 || double.IsInfinity(value))
            {
                panel._ignoreDimensionChange = true;
                panel.SetValue(e.Property, (double)e.OldValue);
                panel._ignoreDimensionChange = false;

                throw new ArgumentException("Invalid parameter", "value");
            }

            panel.InvalidateMeasure();
        }

        #endregion

        public StaggerPanel() { }

        protected override Size MeasureOverride(Size constraint)
        {
            double[] linePoss = new double[] { 0, 0 };

            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;

            Size itemSize = new Size(
                itemWidth.IsNumeric() ? ((constraint.Width < itemWidth) ? constraint.Width : itemWidth) : constraint.Width,
                itemHeight.IsNumeric() ? ((constraint.Height < itemHeight) ? constraint.Height : itemHeight) : constraint.Height);

            double maxSize = 0;
            Orientation orient = Orientation;
            foreach (UIElement element in Children)
            {               
                double offset = linePoss[0];
                int lineIndex = 0;
                for (int i = 1; i < linePoss.Length; i++)
                {
                    if (linePoss[i] < offset)
                    {
                        offset = linePoss[i];
                        lineIndex = i;
                    }
                }

                element.Measure(new Size(itemSize.Width / linePoss.Length, itemSize.Height / linePoss.Length));
                Size desiredSize = element.DesiredSize;

                linePoss[lineIndex] += (orient == Orientation.Horizontal) ? desiredSize.Width : desiredSize.Height;

                if (linePoss[lineIndex] > maxSize)
                    maxSize = linePoss[lineIndex];
            }

            return new Size((orient == Orientation.Horizontal) ? maxSize : itemSize.Width,
                            (orient == Orientation.Vertical) ? maxSize : itemSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double[] linePoss = new double[] { 0, 0 };

            Orientation orient = Orientation;
            double split = (orient == Orientation.Horizontal) ? ActualHeight / linePoss.Length : ActualWidth / linePoss.Length;

            foreach (UIElement element in Children)
            {
                double offset = linePoss[0];
                int lineIndex = 0;
                for (int i = 1; i < linePoss.Length; i++)
                {
                    if (linePoss[i] < offset)
                    {
                        offset = linePoss[i];
                        lineIndex = i;
                    }
                }

                Size desiredSize = element.DesiredSize;

                Rect bounds = (orient == Orientation.Horizontal) ?
                    new Rect(offset, lineIndex * split, element.DesiredSize.Width, element.DesiredSize.Height) :
                    new Rect(lineIndex * split, offset, element.DesiredSize.Width, element.DesiredSize.Height);
                element.Arrange(bounds);

                linePoss[lineIndex] += (orient == Orientation.Horizontal) ? desiredSize.Width : desiredSize.Height;
            }

            return finalSize;
        }
    }
}