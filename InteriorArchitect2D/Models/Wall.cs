using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace InteriorArchitect2D.Models
{
    public enum WallType
    {
        Partition,
        External
    }

    public class Wall : ProjectElement
    {
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double Thickness { get; set; }
        public WallType Type { get; set; }

        public override void Draw(Canvas canvas, double scale)
        {
            double length = Math.Sqrt(Math.Pow(X2 - X, 2) + Math.Pow(Y2 - Y, 2));
            string dimensionText = $"{Math.Round(length, 1)} cm";


            Line wallLine = new Line
            {
                X1 = X * scale,
                Y1 = Y * scale,
                X2 = X2 * scale,
                Y2 = Y2 * scale,
                StrokeThickness = Thickness * scale,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,

                Stroke = IsSelected ? Brushes.Cyan : (Type == WallType.External ? Brushes.DarkSlateGray : Brushes.LightSlateGray),

                Tag = this
            };

            ToolTip tt = new ToolTip();
            tt.Content = dimensionText;

            ToolTipService.SetInitialShowDelay(tt, 1000);

            wallLine.ToolTip = tt;

            canvas.Children.Add(wallLine);

            if (IsSelected)
            {
                double centerX = (X + X2) / 2;
                double centerY = (Y + Y2) / 2;

                double angleRadians = Math.Atan2(Y2 - Y, X2 - X);
                double angleDegrees = angleRadians * (180.0 / Math.PI);

                if (angleDegrees > 90 || angleDegrees < -90) angleDegrees += 180;

                TextBlock dimensionTextBlock = new TextBlock
                {
                    Text = dimensionText,
                    Foreground = Brushes.Cyan, 
                    Background = new SolidColorBrush(Color.FromArgb(200, 10, 10, 10)), 
                    Padding = new Thickness(4, 2, 4, 2),
                    FontSize = 14 / scale, 
                    FontWeight = FontWeights.Bold
                };

                dimensionTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                dimensionTextBlock.RenderTransformOrigin = new Point(0.5, 0.5); 

                TransformGroup textTransform = new TransformGroup();
                textTransform.Children.Add(new RotateTransform(angleDegrees));
                textTransform.Children.Add(new TranslateTransform(0, -15 / scale));
                dimensionTextBlock.RenderTransform = textTransform;

                Canvas.SetLeft(dimensionTextBlock, (centerX * scale) - (dimensionTextBlock.DesiredSize.Width / 2));
                Canvas.SetTop(dimensionTextBlock, (centerY * scale) - (dimensionTextBlock.DesiredSize.Height / 2));



                canvas.Children.Add(dimensionTextBlock);
            }
        }

        public override void Move(double dx, double dy)
        {
            base.Move(dx, dy); 
            X2 += dx;
            Y2 += dy;
        }

    }
}