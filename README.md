# InteriorArchitect2D

A desktop application for simple 2D interior design and floor planning, built with C# and WPF.

## Features

- Drawing walls with different thicknesses (partition, load-bearing, external) with real-time dimension display.
- Placing objects: beds, desks, wardrobes, tables, and doors with opening swing.
- Selecting, moving, and deleting placed elements.
- Rotating and mirroring furniture (along X and Y axes).
- Angle snapping while drawing walls (e.g. 15°, 45°, 90°).
- Panning and zooming across the canvas.
- Automatic project state saving to a JSON file and restoring on startup.

## Controls

- **LMB (Left Mouse Button)** – draw walls, place furniture, select and drag elements.
- **RMB (Right Mouse Button)** – pan the view.
- **Mouse Wheel** – zoom in/out (or rotate selected furniture).
- **Shift (Hold)** – snap angle while drawing or rotating.
- **X / Y** – mirror selected furniture horizontally / vertically.
- **Delete** – delete the selected element.
- **F1** – open the controls help window.

## How to Run

### Requirements
- Windows 10 / 11
- .NET SDK (installed with Visual Studio or via [.NET Download](https://dotnet.microsoft.com/download))

### Command Line (.NET CLI)
1. Open a terminal in the folder containing `InteriorArchitect2D.csproj`.
2. Run:
   ```bash
   dotnet build
   dotnet run