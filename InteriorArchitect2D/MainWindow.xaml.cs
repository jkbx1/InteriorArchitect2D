using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using InteriorArchitect2D.Models;

namespace InteriorArchitect2D
{
    
    public enum OperationMode
    {
        Pointer,
        DrawWall,
        PlaceFurniture
    }

    public partial class MainWindow : Window
    {
        private bool isDraggingElement = false;
        private ProjectElement? draggedElement;
        private Point lastMousePosition;

        private bool isPanning = false;
        private Point panStartPosition;

        private List<ProjectElement> projectElements = new List<ProjectElement>();

        private OperationMode currentMode = OperationMode.Pointer;

        private bool isDrawing = false;
        private Point startPoint;
        private Line? tempLine; 
        private TextBlock? tempDimension;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnDrawWall_Click(object sender, RoutedEventArgs e)
        {
            currentMode = OperationMode.DrawWall;
            Workspace.Cursor = Cursors.Pen; 
        }

        private void Workspace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (currentMode == OperationMode.Pointer)
            {
                if (e.OriginalSource is FrameworkElement clickedUiElement && clickedUiElement.Tag is ProjectElement clickedModel)
                {
                    foreach (var element in projectElements) element.IsSelected = false;
                    clickedModel.IsSelected = true;

                    isDraggingElement = true;
                    draggedElement = clickedModel;
                    lastMousePosition = e.GetPosition(Workspace);
                    Workspace.CaptureMouse();
                }
                else
                {
                    foreach (var element in projectElements) element.IsSelected = false;
                }
                RefreshCanvas();
                return;
            }

            else if (currentMode == OperationMode.DrawWall)
            {
                isDrawing = true;
                startPoint = e.GetPosition(Workspace);

                tempLine = new Line
                {
                    X1 = startPoint.X,
                    Y1 = startPoint.Y,
                    X2 = startPoint.X,
                    Y2 = startPoint.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 4,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Opacity = 0.7
                };
                Workspace.Children.Add(tempLine);

                tempDimension = new TextBlock
                {
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)),
                    Padding = new Thickness(4, 2, 4, 2),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold
                };
                Workspace.Children.Add(tempDimension);
            }

            else if (currentMode == OperationMode.PlaceFurniture)
            {
                Point clickPoint = e.GetPosition(Workspace);

                Furniture newFurniture = new Furniture
                {
                    X = clickPoint.X,
                    Y = clickPoint.Y
                };

                if (CmbFurnitureType.SelectedIndex == 0) 
                {
                    newFurniture.Name = "Bed"; newFurniture.Width = 200; newFurniture.Height = 160; newFurniture.ObjectType = FurnitureType.Rectangular;
                }
                else if (CmbFurnitureType.SelectedIndex == 1) 
                {
                    newFurniture.Name = "Desk"; newFurniture.Width = 120; newFurniture.Height = 60; newFurniture.ObjectType = FurnitureType.Rectangular;
                }
                else if (CmbFurnitureType.SelectedIndex == 2) 
                {
                    newFurniture.Name = "Wardrobe"; newFurniture.Width = 100; newFurniture.Height = 60; newFurniture.ObjectType = FurnitureType.Rectangular;
                }
                else if (CmbFurnitureType.SelectedIndex == 3) 
                {
                    newFurniture.Name = "Table"; newFurniture.Width = 100; newFurniture.Height = 100; newFurniture.ObjectType = FurnitureType.Round; 
                }
                else if (CmbFurnitureType.SelectedIndex == 4) 
                {
                    newFurniture.Name = "Door"; newFurniture.Width = 90; newFurniture.Height = 90; newFurniture.ObjectType = FurnitureType.Door; 
                }

                projectElements.Add(newFurniture);
                RefreshCanvas();
            }

        }

        private void Workspace_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                Point currentPoint = e.GetPosition(this);
                CanvasTranslation.X += (currentPoint.X - panStartPosition.X);
                CanvasTranslation.Y += (currentPoint.Y - panStartPosition.Y);
                panStartPosition = currentPoint;
                return;
            }

            if (currentMode == OperationMode.Pointer && isDraggingElement && draggedElement != null)
            {
                Point currentPoint = e.GetPosition(Workspace);
                double dx = currentPoint.X - lastMousePosition.X;
                double dy = currentPoint.Y - lastMousePosition.Y;

                draggedElement.Move(dx, dy);
                lastMousePosition = currentPoint;

                RefreshCanvas();
                return;
            }

            if (currentMode == OperationMode.DrawWall && isDrawing && tempLine != null)
            {
                Point currentPoint = e.GetPosition(Workspace);

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    double dx = currentPoint.X - startPoint.X;
                    double dy = currentPoint.Y - startPoint.Y;
                    double angleRadians = Math.Atan2(dy, dx);
                    double angleDegrees = angleRadians * (180.0 / Math.PI);

                    double angleStep = GetAngleStep();
                    double roundedAngleDegrees = Math.Round(angleDegrees / angleStep) * angleStep;
                    double newAngleRadians = roundedAngleDegrees * (Math.PI / 180.0);
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    currentPoint.X = startPoint.X + distance * Math.Cos(newAngleRadians);
                    currentPoint.Y = startPoint.Y + distance * Math.Sin(newAngleRadians);
                }

                tempLine.X2 = currentPoint.X;
                tempLine.Y2 = currentPoint.Y;

                if (tempDimension != null)
                {
                    double length = Math.Sqrt(Math.Pow(currentPoint.X - startPoint.X, 2) + Math.Pow(currentPoint.Y - startPoint.Y, 2));
                    tempDimension.Text = $"{Math.Round(length, 1)} cm";

                    Canvas.SetLeft(tempDimension, currentPoint.X + 20);
                    Canvas.SetTop(tempDimension, currentPoint.Y - 25); 
                }
            }
        }

        private void Workspace_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (currentMode == OperationMode.Pointer)
            {
                isDraggingElement = false;
                draggedElement = null;

                Workspace.ReleaseMouseCapture();
            }
            else if (currentMode == OperationMode.DrawWall && isDrawing && tempLine != null)
            {
                isDrawing = false;

                double selectedThickness = 12;
                if (CmbThickness.SelectedIndex == 1) selectedThickness = 25;
                else if (CmbThickness.SelectedIndex == 2) selectedThickness = 40;

                Wall newWall = new Wall
                {
                    X = startPoint.X,
                    Y = startPoint.Y,
                    X2 = tempLine.X2,
                    Y2 = tempLine.Y2,
                    Thickness = selectedThickness,
                    Type = selectedThickness > 25 ? WallType.External : WallType.Partition
                };

                projectElements.Add(newWall);
                RefreshCanvas(); 

                if (tempDimension != null)
                {
                    Workspace.Children.Remove(tempDimension);
                    tempDimension = null;
                }
            }
        }

        private void Workspace_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomStep = 0.05;

            double minScale = 0.2; 
            double maxScale = 5.0; 
            double oldScale = CanvasScale.ScaleX;
            double newScale = oldScale;

            if (currentMode == OperationMode.Pointer)
            {
                var selectedFurniture = projectElements.FirstOrDefault(element => element.IsSelected) as Furniture;
                if (selectedFurniture != null)
                {
                    double step = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)
                                  ? GetAngleStep() : 2.0;

                    if (e.Delta > 0) selectedFurniture.Rotation += step;
                    else selectedFurniture.Rotation -= step;

                    if (step > 2.0)
                    {
                        selectedFurniture.Rotation = Math.Round(selectedFurniture.Rotation / step) * step;
                    }

                    RefreshCanvas();
                    return; 
                }
            }

            if (e.Delta > 0 && oldScale < maxScale)
            {
                newScale += zoomStep;
            }
            else if (e.Delta < 0 && oldScale > minScale)
            {
                newScale -= zoomStep;
            }

            if (oldScale != newScale)
            {
                Point mousePosition = e.GetPosition(Workspace);

                CanvasTranslation.X -= mousePosition.X * (newScale - oldScale);
                CanvasTranslation.Y -= mousePosition.Y * (newScale - oldScale);

                CanvasScale.ScaleX = newScale;
                CanvasScale.ScaleY = newScale;
                TxtZoomValue.Text = $"{Math.Round(newScale * 100)}%";
            }
        }

        private void Workspace_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPanning = true;
            panStartPosition = e.GetPosition(this);
            Workspace.Cursor = Cursors.ScrollAll; 
        }

        private void Workspace_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            isPanning = false;
            Workspace.Cursor = Cursors.Arrow; 
 
            if (currentMode == OperationMode.DrawWall)
            {
                Workspace.Cursor = Cursors.Pen;
            }
        }

        private void ResetView()
        {
            CanvasScale.ScaleX = 1.0;
            CanvasScale.ScaleY = 1.0;
            CanvasTranslation.X = 0;
            CanvasTranslation.Y = 0;
        }

        private void BtnCenterView_Click(object sender, RoutedEventArgs e)
        {
            ResetView();
        }

        private void RefreshCanvas()
        {
            Workspace.Children.Clear();

            var sortedElements = projectElements.OrderBy(element => element is Furniture ? 1 : 0);

            foreach (var element in sortedElements)
            {
                element.Draw(Workspace, 1.0);
            }
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (projectElements.Count > 0)
            {
                projectElements.RemoveAt(projectElements.Count - 1);
                RefreshCanvas();
            }
        }

        private void BtnNewSketch_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete the entire project? Changes will be permanently lost.",
                "New Sketch",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                projectElements.Clear();
                RefreshCanvas();
                ResetView(); 
            }
        }


        private void BtnEditMode_Click(object sender, RoutedEventArgs e)
        {
            currentMode = OperationMode.Pointer;
            Workspace.Cursor = Cursors.Hand; 
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
      
            if (e.Key == Key.Delete)
            {
                int removedCount = projectElements.RemoveAll(element => element.IsSelected);
                if (removedCount > 0)
                {
                    RefreshCanvas();
                }
            }

            if (e.Key == Key.X || e.Key == Key.Y)
            {
             
                var selectedFurniture = projectElements.FirstOrDefault(element => element.IsSelected) as Furniture;

                if (selectedFurniture != null)
                {
                    if (e.Key == Key.X)
                        selectedFurniture.MirrorX = !selectedFurniture.MirrorX;

                    if (e.Key == Key.Y)
                        selectedFurniture.MirrorY = !selectedFurniture.MirrorY;

                    RefreshCanvas();
                }
            }
        }

        private void BtnPlaceFurniture_Click(object sender, RoutedEventArgs e)
        {
            currentMode = OperationMode.PlaceFurniture;
            Workspace.Cursor = Cursors.Cross;
        }

        private double GetAngleStep()
        {
            if (CmbAngles.SelectedItem is ComboBoxItem item && item.Content != null)
            {
                string text = item.Content.ToString()!.Replace("°", "").Trim();
                if (double.TryParse(text, out double value)) return value;
            }
            return 15.0; 
        }


        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.ShowDialog();
        }



        private readonly string saveFilePath = "project_state.json";

      
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (File.Exists(saveFilePath))
            {
                try
                {
                    string json = File.ReadAllText(saveFilePath);
                    var savedElements = JsonSerializer.Deserialize<List<ProjectElement>>(json);

                    if (savedElements != null)
                    {
                        projectElements = savedElements;
                        RefreshCanvas();
                    }
                }
                catch
                {
                    MessageBox.Show("Failed to load previous project state.", "Read Error");
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            var options = new JsonSerializerOptions { WriteIndented = true };

            string json = JsonSerializer.Serialize(projectElements, options);
            File.WriteAllText(saveFilePath, json);
        }
    }
}