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
    public enum FurnitureType
    {
        Rectangular,
        Round,
        Door
    }

    public class Furniture : ProjectElement
    {
        public string Name { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Rotation { get; set; } // in deg
        public FurnitureType ObjectType { get; set; } = FurnitureType.Rectangular;
        public bool MirrorX { get; set; } = false;
        public bool MirrorY { get; set; } = false;

        public override void Draw(Canvas canvas, double scale)
        {
            Grid furnitureGrid = new Grid
            {
                Width = Width,
                Height = Height,
                Tag = this
            };

            Shape contour;

            if (ObjectType == FurnitureType.Round)
            {
                contour = new Ellipse();
            }
            else if (ObjectType == FurnitureType.Door)
            {
             
                contour = new Path
                {
                    Data = Geometry.Parse($"M 0,{Height} L 0,0 A {Width},{Height} 0 0,1 {Width},{Height}")
                };
            }
            else
            {
                contour = new Rectangle();
            }

            contour.Fill = ObjectType == FurnitureType.Door ? Brushes.Transparent : new SolidColorBrush(Color.FromArgb(50, 100, 149, 237));
            contour.Stroke = IsSelected ? Brushes.Cyan : (ObjectType == FurnitureType.Door ? Brushes.Orange : Brushes.CornflowerBlue);
            contour.StrokeThickness = IsSelected ? 3 : 2;
            contour.Tag = this;

            furnitureGrid.Children.Add(contour);

            if (ObjectType != FurnitureType.Door)
            {
                TextBlock label = new TextBlock
                {
                    Text = Name,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Tag = this
                };
                furnitureGrid.Children.Add(label);
            }

            TransformGroup transforms = new TransformGroup();

            double pivotX = ObjectType == FurnitureType.Door ? 0 : Width / 2;
            double pivotY = ObjectType == FurnitureType.Door ? 0 : Height / 2;

            transforms.Children.Add(new RotateTransform(Rotation, pivotX, pivotY));

            double scaleX = MirrorX ? -scale : scale;
            double scaleY = MirrorY ? -scale : scale;
       
            transforms.Children.Add(new ScaleTransform(scaleX, scaleY, pivotX, pivotY));

            furnitureGrid.RenderTransform = transforms;

            Canvas.SetLeft(furnitureGrid, (X - pivotX) * scale);
            Canvas.SetTop(furnitureGrid, (Y - pivotY) * scale);

            canvas.Children.Add(furnitureGrid);
        }
    }
}