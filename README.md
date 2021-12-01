# cwave
ColorWave WPF Color Picker UserControl, CWave library v1.0.0.

CWave Library Documentation:
https://colorwavecontrol.square.site/documentation

Example:

`<Window x:Class="CWaveWPF.MainWindow"
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
