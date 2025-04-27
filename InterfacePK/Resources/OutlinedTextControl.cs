using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DQB2IslandEditor.InterfacePK.Resources
{
    public class OutlinedTextControl : FrameworkElement
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(16.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(new FontFamily("Segoe UI"), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(OutlinedTextControl),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextWeightProperty =
    DependencyProperty.Register(nameof(TextWeight), typeof(FontWeight), typeof(OutlinedTextControl),
        new FrameworkPropertyMetadata(FontWeights.DemiBold, FrameworkPropertyMetadataOptions.AffectsRender));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public FontWeight TextWeight
        {
            get => (FontWeight) GetValue(TextWeightProperty);
            set => SetValue(TextWeightProperty, value);
        }

        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        private List<FormattedText> GetFormattedTextLines(double availableWidth, string Text)
        {
            var typeface = new Typeface(FontFamily, FontStyles.Normal, TextWeight, FontStretches.Normal);
            var lines = new List<FormattedText>();
            string[] words = Text.Split(' ');

            string currentLine = string.Empty;
            string currentText = string.Empty;

            foreach (var word in words)
            {
                currentText = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;

                var formattedText = new FormattedText(
                    currentText,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    FontSize,
                    Brushes.Black, // dummy brush, won't be used
                    VisualTreeHelper.GetDpi(this).PixelsPerDip
                );

                // If the line is wider than the available width, start a new line
                if (formattedText.Width > availableWidth)
                {
                    lines.Add(new FormattedText(
                        currentLine,
                        System.Globalization.CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        FontSize,
                        Brushes.Black,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip
                    ));
                    currentLine = word;  // Start a new line with the current word
                }
                else
                {
                    currentLine = currentText;  // Add to the current line
                }
            }

            if (!string.IsNullOrEmpty(currentLine)) // Add the last line
            {
                lines.Add(new FormattedText(
                    currentLine,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    FontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip
                ));
            }

            return lines;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (string.IsNullOrEmpty(Text))
                return;

            var lines = GetFormattedTextLines(ActualWidth - 20, (string)Text.Clone()); // leave some padding

            double totalHeight = lines.Sum(line => line.Height);
            double yOffset = (ActualHeight - totalHeight) / 2;  // Center vertically

            foreach (var formattedText in lines)
            {
                // Calculate the X offset for centering
                double xOffset = (ActualWidth - formattedText.Width) / 2;

                var pen = new Pen(Stroke, StrokeThickness)
                {
                    LineJoin = PenLineJoin.Round,      // round corners between segments
                    StartLineCap = PenLineCap.Round,   // round cap at the start
                    EndLineCap = PenLineCap.Round      // round cap at the end
                };

                var geometry = formattedText.BuildGeometry(new Point(xOffset+0.5, yOffset+0.5));
                // Draw stroke
                drawingContext.DrawGeometry(null, pen, geometry);

                geometry = formattedText.BuildGeometry(new Point(xOffset, yOffset));;
                // Draw fill
                drawingContext.DrawGeometry(Fill, null, geometry);

                // Move the vertical offset for the next line
                yOffset += formattedText.Height * 0.7;
            }
        }
    }
}