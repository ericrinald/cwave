# CWave
ColorWave WPF Color Picker UserControl, CWave library v1.0.0.

## Description and Documentation:
The ColorWave WPF Color Picker is a .NET FrameWork(v4.8) UserControl, part of the CWave library. This repo includes an example solution with the ColorWave WPF Color Picker in demo-mode. Documentation can be found at: https://www.cwavelib.net/documentation

#### Example XAML:
Set SelectedBrush to the desired brush to load with or leave the property unset to load with the default Brushes.Black. SelectedBrush and Alpha/AlphaPercentage properties may be set in the xaml editor at design-time. Other color-influencing properties will not load in run-time if they were set at design-time in the xaml editor.
```
<Window x:Class="CWaveWPF.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:CWaveWPF"
        xmlns:CWave="clr-namespace:CWave;assembly=CWave"
        mc:Ignorable="d"
        Title="MainWindow" Height="364.916" Width="471.954">
        
    <Grid>
        <CWave:ColorWave SelectedBrush="DodgerBlue"/>
    </Grid>
        
</Window>
```

#### Example C#:
You can set the SelectedBrush and Alpha/AlphaPercentage properties from the Window's constructor or Loaded event. All other color-influencing properties can be set after the control has been rendered. When creating a new instance of the control or loading the parent window of the control, the control's SelectedBrushChanged event will be raised once after the control's Loaded event has been raised. After the control has been rendered, SelectedBrushChanged will be raised each time any color-influencing property changes. 

```
        public MainWindow()
        {
        
            Color colorToLoad = new Color() { R = 30, G = 144, B = 255, A = 220 }; // DodgerBlue
            colorWave.SelectedBrush = new SolidColorBrush(colorToLoad);
            
            ////  Alternative:
            // colorWave.SelectedBrush = Brushes.DodgerBlue;
            // colorWave.AlphaPercentage = 86.3;
            
        }
```

### Nuget Package
```
Install-Package CWave -Version 1.0.0
```
