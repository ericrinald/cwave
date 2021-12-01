using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CWave;

// ColorWave Color Picker UserControl

namespace ColorWave_Example
{

    public class BrushSampleItem
    {
        public string Name { get; set; }
        public Brush SampleBrush { get; set; }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<BrushSampleItem> BrushSamples = new ObservableCollection<BrushSampleItem>();
        public BrushSampleItem _brushItem;

        private static readonly Regex regex = new Regex("[^0-9]");

        private bool _showingNonBindingProps = true;
        private bool _brushChanged = false;

        public bool ShowingNonBindingProps
        {
            get { return _showingNonBindingProps; }
            set
            {
                _showingNonBindingProps = value;
                NotifyPropertyChanged("ShowingNonBindingProps");
            }
        }

        public bool BrushChanged
        {
            get { return _brushChanged; }
            set
            {
                _brushChanged = value;
                NotifyPropertyChanged("BrushChanged");
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {

            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        public MainWindow()
        {

            InitializeComponent();

            // Add UnhandledException event handler
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            PopulateSampleBrushes();
            lvBrushes.ItemsSource = BrushSamples;


            /* Set EyeDropperCursor property with a valid path to a custom cursor file when ColorCaptureArea
             * property is set to (CaptureArea.Screen). If EyeDropperCursor is empty when 'BeginColorCapture()
             * is called, ColorCaptureArea will revert back to (CaptureArea.Window).
             * 
             * The cursor that is pointed to the EyeDropperCursor replaces all Windows system cursors during
             * color capture. When color capture is finished, the cursors will be set back to what they were
             * before the capture process began. It is important to refer to the RestoreSystemCursor method
             * example in the Dispatcher_UnhandledException event handler.*/
            colorWave.EyeDropperCursor = AppDomain.CurrentDomain.BaseDirectory + "eyedropper_cursor.cur";


            /* Create a new instance of the control at runtime and set the SelectedBrush property.

            //  Color colorToLoad = new Color() { R = 255, G = 165, B = 0, A = 150 };
            //  ColorWave newColorWave = new ColorWave() { SelectedBrush = new SolidColorBrush(colorToLoad) };
            //  <ParentControl>.Children.Add(newColorWave);


            /* By default the SelectedBrush property is Brushes.Black unless otherwise set in the xaml
             * editor or c# code editor. The SelectedBrushChanged event is raised once, but not until
             * reaching the control's Loaded event. After the control has rendered, the SelectedBrushChanged
             * event will be raised each time a color-influencing property has changed. Initially, you can
             * use the SelectedBrush and Alpha/AlphaPercetage properties when loading a color from the Window's
             * constructor or from the Window's Loaded event. All other color-influencing properties will be
             * ignored until after the content has been rendered. */


            Color colorToLoad = new Color() { R = 30, G = 144, B = 255, A = 220 }; // DodgerBlue
            colorWave.SelectedBrush = new SolidColorBrush(colorToLoad);

            ////  Alternative:
            // colorWave.SelectedBrush = Brushes.DodgerBlue;
            // colorWave.AlphaPercentage = 86.3;


        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            /* Call RestoreSystemCursor() in this UnhandledException event handler as well as any
             * place in the code where an exception can be thrown, causing the application to exit
             * runtime mode, otherwise the alternative is to manually go into the Windows mouse
             * settings and reset the scheme of your pointers. If ColorCaptureArea is set to
             * CaptureArea.Window then calling this method is not neccessary. */

            if (colorWave.IsColorCaptureActive &&
                colorWave.ColorCaptureArea == ColorWave.CaptureArea.Screen)
            {
                colorWave.RestoreSystemCursor();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // ContentRendered event handler. 
            PresentationSource ps = PresentationSource.FromVisual(this);
            ps.ContentRendered += Ps_ContentRendered;

            // Start with showing non-binding properties.
            ShowProperties();

        }

        private void Ps_ContentRendered(object sender, EventArgs e)
        {

            // colorWave.HexValue = "#BE80FF80"; // HexValue

            /* The HexValue above is equivalent to the Hue, Saturation and
             * Brightness settings below. These properties can be changed
             * as soon as the content has rendered. */

            //// Alternative:
            // colorWave.Hue = 120; // Degrees
            // colorWave.Saturation = 50; // Percent
            // colorWave.Brightness = 100; // Percent
            // colorWave.AlphaPercentage = 74.5; // Percent

            /* When the content has rendered, Red, Green and Blue values
             * can also be changed */

            //// Alternative:
            // colorWave.Red = 128;
            // colorWave.Green = 255;
            // colorWave.Blue = 128;
            // colorWave.AlphaPercentage = 74.5;

        }

        private void PopulateSampleBrushes()
        {

            _brushItem = new BrushSampleItem()
            {
                Name = "Custom",
                SampleBrush = Brushes.Black
            };
            BrushSamples.Add(_brushItem);

            foreach (System.Reflection.PropertyInfo info in typeof(Colors).GetProperties())
            {

                _brushItem = new BrushSampleItem()
                {
                    Name = info.Name,
                    SampleBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(info.Name)
                };
                BrushSamples.Add(_brushItem);

            }

        }

        private void ColorWave_ColorCaptureChanged(object sender, EyeDropperEventArgs e)
        {

            /* This event is raised each time the color has made a change while color capture is active.
             * IsColorCaptureActive is returns True after the EyeDropper tool button is pressed or after
             * calling BeginColorCapture(). */

            // byte red = e.R;
            // byte green = e.G;
            // byte blue = e.B;

        }

        private void ColorWave_SelectedBrushChanged(object sender, SelectedBrushEventArgs e)
        {

            /* SelectedBrushChanged event is raised when any one of the color-influencing property values
             * has changed. SelectedBrush, Red, Green, Blue, Alpha, AlphaPercentage, HexValue, Hue, Saturation or
             * Brightness are all color-influencing properties. */

            if (ShowingNonBindingProps && e.CurrentCaptureStatus != ColorWave.CaptureStatus.Canceled)
            {

                textRedValue.Text = colorWave.Red.ToString();
                textGreenValue.Text = colorWave.Green.ToString();
                textBlueValue.Text = colorWave.Blue.ToString();
                textAlphaValue.Text = colorWave.Alpha.ToString();
                textAlphaPercentage.Text = colorWave.AlphaPercentage.ToString("0.#");

                textHue.Text = colorWave.Hue.ToString("0.000");
                textSaturation.Text = colorWave.Saturation.ToString("0.00");
                textBrightness.Text = colorWave.Brightness.ToString("0.00");

                textHexValue.Text = colorWave.HexValue;
                canvasSelectedBrush.Background = colorWave.SelectedBrush;

            }


            /* When capture is successfully completed on Left-Button MouseDown, SelectedBrushChanged event
             * will be raised and CaptureStatus.Complete will be passed through SelectedBrushEventArgs.
             * If CancelColorCapture() is called, or Escape key is pressed, or the parent window has lost
             * focus, then SelectedBrushChanged event will be raised with CurrentCaptureStatus = CaptureStatus.Canceled. */

            /* SelectedBrushChanged can also be raised upon completion or cancellation of the color capture
             * process. */

            // Example:

            if (e.CurrentCaptureStatus == ColorWave.CaptureStatus.Complete)
            {

                // MessageBox.Show("Color capture complete.", "From SelectedBrushChanged Event Handler",
                //                MessageBoxButton.OK, MessageBoxImage.Information);

                // Toggle MainWindow's BrushChanged property to trigger DataTrigger on lvBrushes
                BrushChangedPropToggle();

            }
            else if (e.CurrentCaptureStatus == ColorWave.CaptureStatus.Canceled)
            {

                // MessageBox.Show("Color capture canceled.", "From SelectedBrushChanged Event Handler", MessageBoxButton.OK, MessageBoxImage.Stop);

            }

        }

        private void BrushChangedPropToggle()
        {

            /* xaml DataTrigger binding to BrushChanged property on (ListView)lvBrushes selects
             * index 0: 'Custom'. The MainWindow's BrushChanged property is set to true then false
             * again in order to trigger DataTrigger. */
            BrushChanged = true;
            BrushChanged = false;

        }

        private void comboFieldsShown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboFieldsShown.SelectedItem != null && colorWave != null)
            {

                ComboBoxItem cbi = (ComboBoxItem)comboFieldsShown.SelectedItem;
                switch (cbi.Content.ToString())
                {

                    case "RGB":
                        colorWave.FieldsShown = ColorWave.Fields.RGB;
                        break;

                    case "HSB":
                        colorWave.FieldsShown = ColorWave.Fields.HSB;
                        break;

                }

            }

        }

        private void comboDisplaySelectedBrush_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboSelectedBrushPanel.SelectedItem != null && colorWave != null)
            {

                ComboBoxItem cbi = (ComboBoxItem)comboSelectedBrushPanel.SelectedItem;
                switch (cbi.Content.ToString())
                {

                    case "Collapsed":
                        colorWave.SelectedBrushPanel = ColorWave.DisplayPanel.Collapsed;
                        break;

                    case "Visible":
                        colorWave.SelectedBrushPanel = ColorWave.DisplayPanel.Visible;
                        break;

                }

            }

        }

        private void comboRgbHsbPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboInputPanel.SelectedItem != null && colorWave != null)
            {

                ComboBoxItem cbi = (ComboBoxItem)comboInputPanel.SelectedItem;
                switch (cbi.Content.ToString())
                {

                    case "Collapsed":
                        colorWave.InputPanel = ColorWave.DisplayPanel.Collapsed;
                        break;

                    case "Visible":
                        colorWave.InputPanel = ColorWave.DisplayPanel.Visible;
                        break;

                }

            }

        }

        private void lvBrushes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (lvBrushes.SelectedIndex == -1) { return; }
            if (lvBrushes.SelectedIndex == 0) { lvBrushes.ScrollIntoView(lvBrushes.SelectedItem); }
            colorWave.SelectedBrush = BrushSamples[lvBrushes.SelectedIndex].SampleBrush;

        }

        private void ShowProperties()
        {

            // Clear all bindings
            BindingOperations.ClearBinding(textHexValueBinding, TextBox.TextProperty);
            BindingOperations.ClearBinding(comboFieldsShownBinding, ComboBox.SelectedValueProperty);
            BindingOperations.ClearBinding(comboInputPanelBinding, ComboBox.SelectedValueProperty);
            BindingOperations.ClearBinding(comboSelectedBrushPanelBinding, ComboBox.SelectedValueProperty);
            BindingOperations.ClearBinding(canvasSelectedBrushBinding, Canvas.BackgroundProperty);
            BindingOperations.ClearBinding(textRedValueBinding, TextBox.TextProperty);
            BindingOperations.ClearBinding(textGreenValueBinding, TextBox.TextProperty);
            BindingOperations.ClearBinding(textBlueValueBinding, TextBox.TextProperty);
            BindingOperations.ClearBinding(textAlphaValueBinding, TextBox.TextProperty);
            BindingOperations.ClearBinding(textAlphaPercentageBinding, TextBox.TextProperty);
            BindingOperations.ClearBinding(textHueBinding, TextBox.TextProperty);
            BindingOperations.ClearBinding(textSaturationBinding, TextBox.TextProperty);
            BindingOperations.ClearBinding(textBrightnessBinding, TextBox.TextProperty);

            // Fill textboxes with property values
            textRedValue.Text = colorWave.Red.ToString();
            textGreenValue.Text = colorWave.Green.ToString();
            textBlueValue.Text = colorWave.Blue.ToString();
            textAlphaValue.Text = colorWave.Alpha.ToString();
            textAlphaPercentage.Text = colorWave.AlphaPercentage.ToString("0.#");
            textHexValue.Text = colorWave.HexValue;
            textHue.Text = colorWave.Hue.ToString("0.000");
            textSaturation.Text = colorWave.Saturation.ToString("0.00");
            textBrightness.Text = colorWave.Brightness.ToString("0.00");

            // Clear Combobox ItemsSource
            comboFieldsShownBinding.ItemsSource = null;
            comboInputPanelBinding.ItemsSource = null;
            comboSelectedBrushPanelBinding.ItemsSource = null;

            // Fill ComboBoxes with property values
            ComboBoxItem cbiRGB = new ComboBoxItem { Content = "RGB" };
            ComboBoxItem cbiHSB = new ComboBoxItem { Content = "HSB" };
            comboFieldsShown.Items.Add(cbiRGB);
            comboFieldsShown.Items.Add(cbiHSB);

            ComboBoxItem cbiRgbPanelVisible = new ComboBoxItem { Content = "Visible" };
            ComboBoxItem cbiRgbPanelCollapsed = new ComboBoxItem { Content = "Collapsed" };
            comboInputPanel.Items.Add(cbiRgbPanelVisible);
            comboInputPanel.Items.Add(cbiRgbPanelCollapsed);

            ComboBoxItem cbiBrushPanelVisible = new ComboBoxItem { Content = "Visible" };
            ComboBoxItem cbiBrushPanelCollapsed = new ComboBoxItem { Content = "Collapsed" };
            comboSelectedBrushPanel.Items.Add(cbiBrushPanelVisible);
            comboSelectedBrushPanel.Items.Add(cbiBrushPanelCollapsed);

            // Select current property values in ComboBoxes
            comboFieldsShown.SelectedIndex = Convert.ToInt32(colorWave.FieldsShown); // RGB = 0, HSB = 1
            comboInputPanel.SelectedIndex = Convert.ToInt32(colorWave.InputPanel); // Hidden = 0, Visible = 1
            comboSelectedBrushPanel.SelectedIndex = Convert.ToInt32(colorWave.SelectedBrushPanel); // Hidden = 0, Visible = 1

            // Display SelectedBrush on canvas control
            canvasSelectedBrush.Background = colorWave.SelectedBrush;

            // Disable gridControlPropertyBindings grid and enable gridControlProperties
            gridControlPropertyBindings.IsEnabled = false;
            gridControlPropertyBindings.Opacity = 0.5;
            gridControlProperties.IsEnabled = true;
            gridControlProperties.Opacity = 1;

            ShowingNonBindingProps = true;

        }

        private void ShowBindings()
        {

            // Clear property values from Textboxes and ComboBoxes
            textRedValue.Clear();
            textGreenValue.Clear();
            textBlueValue.Clear();
            textAlphaValue.Clear();
            textAlphaPercentage.Clear();
            textHexValue.Clear();
            textHue.Clear();
            textSaturation.Clear();
            textBrightness.Clear();
            comboFieldsShown.Items.Clear();
            comboInputPanel.Items.Clear();
            comboSelectedBrushPanel.Items.Clear();
            canvasSelectedBrush.Background = Brushes.Transparent;

            // Set combobox ItemSources
            comboFieldsShownBinding.ItemsSource = Enum.GetValues(typeof(ColorWave.Fields));
            comboInputPanelBinding.ItemsSource = Enum.GetValues(typeof(ColorWave.DisplayPanel));
            comboSelectedBrushPanelBinding.ItemsSource = Enum.GetValues(typeof(ColorWave.DisplayPanel));

            // HexValue binding
            Binding hexValueBinding = new Binding();
            hexValueBinding.Path = new PropertyPath("HexValue");
            hexValueBinding.Source = colorWave;
            textHexValueBinding.SetBinding(TextBox.TextProperty, hexValueBinding);

            // FieldsShown binding
            Binding fieldsShownBinding = new Binding();
            fieldsShownBinding.Path = new PropertyPath("FieldsShown");
            fieldsShownBinding.Source = colorWave;
            comboFieldsShownBinding.SetBinding(ComboBox.SelectedValueProperty, fieldsShownBinding);

            // InputPanel binding
            Binding inputPanelBinding = new Binding();
            inputPanelBinding.Path = new PropertyPath("InputPanel");
            inputPanelBinding.Source = colorWave;
            comboInputPanelBinding.SetBinding(ComboBox.SelectedValueProperty, inputPanelBinding);

            // SelectedBrushPanel binding
            Binding selectedBrushPanelBinding = new Binding();
            selectedBrushPanelBinding.Path = new PropertyPath("SelectedBrushPanel");
            selectedBrushPanelBinding.Source = colorWave;
            comboSelectedBrushPanelBinding.SetBinding(ComboBox.SelectedValueProperty, selectedBrushPanelBinding);

            // SelectedBrush binding
            Binding selectedBrushBinding = new Binding();
            selectedBrushBinding.Path = new PropertyPath("SelectedBrush");
            selectedBrushBinding.Source = colorWave;
            canvasSelectedBrushBinding.SetBinding(Canvas.BackgroundProperty, selectedBrushBinding);

            // Red binding
            Binding redValueBinding = new Binding();
            redValueBinding.Path = new PropertyPath("Red");
            redValueBinding.Source = colorWave;
            textRedValueBinding.SetBinding(TextBox.TextProperty, redValueBinding);

            // Green binding
            Binding greenValueBinding = new Binding();
            greenValueBinding.Path = new PropertyPath("Green");
            greenValueBinding.Source = colorWave;
            textGreenValueBinding.SetBinding(TextBox.TextProperty, greenValueBinding);

            // Blue binding
            Binding blueValueBinding = new Binding();
            blueValueBinding.Path = new PropertyPath("Blue");
            blueValueBinding.Source = colorWave;
            textBlueValueBinding.SetBinding(TextBox.TextProperty, blueValueBinding);

            // Alpha binding
            Binding alphaValueBinding = new Binding();
            alphaValueBinding.Path = new PropertyPath("Alpha");
            alphaValueBinding.Source = colorWave;
            textAlphaValueBinding.SetBinding(TextBox.TextProperty, alphaValueBinding);

            // AlphaPercentage binding
            Binding alphaPercentageBinding = new Binding();
            alphaPercentageBinding.Path = new PropertyPath("AlphaPercentage");
            alphaPercentageBinding.StringFormat = "{0:0.#}";
            alphaPercentageBinding.Source = colorWave;
            textAlphaPercentageBinding.SetBinding(TextBox.TextProperty, alphaPercentageBinding);

            // Hue binding
            Binding hueBinding = new Binding();
            hueBinding.Path = new PropertyPath("Hue");
            hueBinding.StringFormat = "{0:F3}";
            hueBinding.Source = colorWave;
            textHueBinding.SetBinding(TextBox.TextProperty, hueBinding);

            // Saturation binding
            Binding saturationBinding = new Binding();
            saturationBinding.Path = new PropertyPath("Saturation");
            saturationBinding.StringFormat = "{0:F2}";
            saturationBinding.Source = colorWave;
            textSaturationBinding.SetBinding(TextBox.TextProperty, saturationBinding);

            // Brightness binding
            Binding brightnessBinding = new Binding();
            brightnessBinding.Path = new PropertyPath("Brightness");
            brightnessBinding.StringFormat = "{0:F2}";
            brightnessBinding.Source = colorWave;
            textBrightnessBinding.SetBinding(TextBox.TextProperty, brightnessBinding);

            // Disable gridControlProperties grid and enable gridControlPropertyBindings
            gridControlProperties.IsEnabled = false;
            gridControlProperties.Opacity = 0.5;
            gridControlPropertyBindings.IsEnabled = true;
            gridControlPropertyBindings.Opacity = 1;

            ShowingNonBindingProps = false;

        }

        private void UpdateTextboxBinding(object sender)
        {

            TextBox tboxSender = sender as TextBox;

            BindingExpression textBinding = BindingOperations.GetBindingExpression(tboxSender, TextBox.TextProperty);

            /* If there is not a binding then textBinding will be null and will not be updated.
             * This is in place so that this method can be used in the event handlers that are
             * being shared by textboxes whether they are binding or non-binding. */
            if (textBinding != null)
            {

                textBinding.UpdateSource();

            }

        }

        private double KeepWithinRange(double value, int min, int max)
        {

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }

            return value;

        }

        private bool UpdateProperty(object sender)
        {

            TextBox tboxSender = sender as TextBox;

            // Get textbox's tag
            string tboxTag = tboxSender.Tag.ToString();

            if (tboxSender.Text == "")
            {
                tboxSender.Text = "0";
            }

            // Set ColorWave control's property when it's cooresponding texbox receives Enter key input
            if (tboxTag == "Hue")
            {

                tboxSender.Text = KeepWithinRange(Convert.ToDouble(tboxSender.Text), 0, 360).ToString();

                double hueVal = Convert.ToDouble(tboxSender.Text);

                if (colorWave.Hue == hueVal) { return false; }
                if (ShowingNonBindingProps) { colorWave.Hue = hueVal; }

            }
            else if (tboxTag == "Saturation")
            {

                tboxSender.Text = KeepWithinRange(Convert.ToDouble(tboxSender.Text), 0, 100).ToString();

                double satVal = Convert.ToDouble(tboxSender.Text);

                if (colorWave.Saturation == satVal) { return false; }
                if (ShowingNonBindingProps) { colorWave.Saturation = satVal; }

            }
            else if (tboxTag == "Brightness")
            {

                tboxSender.Text = KeepWithinRange(Convert.ToDouble(tboxSender.Text), 0, 100).ToString();

                double brightVal = Convert.ToDouble(tboxSender.Text);

                if (colorWave.Brightness == brightVal) { return false; }
                if (ShowingNonBindingProps) { colorWave.Brightness = brightVal; }

            }
            else if (tboxTag == "Red")
            {

                tboxSender.Text = KeepWithinRange(Convert.ToDouble(tboxSender.Text), 0, 255).ToString();

                byte redVal = Convert.ToByte(tboxSender.Text);

                if (colorWave.Red == redVal) { return false; }
                if (ShowingNonBindingProps) { colorWave.Red = redVal; }

            }
            else if (tboxTag == "Green")
            {

                tboxSender.Text = KeepWithinRange(Convert.ToDouble(tboxSender.Text), 0, 255).ToString();

                byte greenVal = Convert.ToByte(tboxSender.Text);

                if (colorWave.Green == greenVal) { return false; }
                if (ShowingNonBindingProps) { colorWave.Green = greenVal; }

            }
            else if (tboxTag == "Blue")
            {

                tboxSender.Text = KeepWithinRange(Convert.ToDouble(tboxSender.Text), 0, 255).ToString();

                byte blueVal = Convert.ToByte(tboxSender.Text);

                if (colorWave.Blue == blueVal) { return false; }
                if (ShowingNonBindingProps) { colorWave.Blue = blueVal; }

            }
            else if (tboxTag == "HexValue")
            {

                if (ShowingNonBindingProps) { colorWave.HexValue = tboxSender.Text; }

            }
            else if (tboxTag == "Alpha")
            {

                tboxSender.Text = KeepWithinRange(Convert.ToDouble(tboxSender.Text), 0, 255).ToString();

                byte alphaVal = Convert.ToByte(tboxSender.Text);

                if (colorWave.Alpha == alphaVal) { return false; }
                if (ShowingNonBindingProps) { colorWave.Alpha = alphaVal; }

            }
            else if (tboxTag == "AlphaPercentage")
            {

                tboxSender.Text = KeepWithinRange(Convert.ToDouble(tboxSender.Text), 0, 100).ToString();

                double alphaPercentVal = Convert.ToDouble(tboxSender.Text);

                if (colorWave.AlphaPercentage == alphaPercentVal) { return false; }
                if (ShowingNonBindingProps) { colorWave.AlphaPercentage = alphaPercentVal; }

            }

            return true;

        }

        private void TBoxChangeValueUpDown(TextBox textbox, MouseWheelEventArgs e, double increment, int min, int max)
        {

            double newValue = 0;
            double currentValue;

            if (textbox.Text == "")
            {
                currentValue = 0;
            }
            else
            {
                currentValue = Convert.ToDouble(textbox.Text);
            }

            if (e.Delta > 0) // Mouse wheel Up
            {
                newValue = KeepWithinRange(currentValue + increment, min, max);
            }
            else if (e.Delta < 0) // Mouse wheel down
            {

                newValue = KeepWithinRange(currentValue - increment, min, max);
            }

            textbox.Text = newValue.ToString();

        }

        private void OnMouseWheelChanged(object sender, MouseWheelEventArgs e)
        {

            TextBox tboxSender = sender as TextBox;

            // Get textbox's tag
            string tboxTag = tboxSender.Tag.ToString();

            // Change value in TextBox when receiving MouseWheel input
            if (tboxTag == "Hue")
            {

                TBoxChangeValueUpDown(tboxSender, e, 0.5, 0, 360);

            }
            else if (tboxTag == "Saturation" || tboxTag == "Brightness" || tboxTag == "AlphaPercentage")
            {
                TBoxChangeValueUpDown(tboxSender, e, 0.5, 0, 100);

            }
            else if (tboxTag == "Red" || tboxTag == "Green" || tboxTag == "Blue" || tboxTag == "Alpha")
            {

                TBoxChangeValueUpDown(tboxSender, e, 1, 0, 255);

            }

        }

        private static bool IsCharacterAllowed(string text)
        {
            return !regex.IsMatch(text);
        }

        private void AllTextBoxes_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {

                bool propUpdated = UpdateProperty(sender);
                if (propUpdated)
                {

                    // Toggle MainWindow's BrushChanged property to trigger DataTrigger on lvBrushes
                    BrushChangedPropToggle();

                    UpdateTextboxBinding(sender);

                }

            }

        }

        private void AllTextBoxes_LostFocus(object sender, RoutedEventArgs e)
        {

            bool propUpdated = UpdateProperty(sender);
            if (propUpdated)
            {

                // Toggle MainWindow's BrushChanged property to trigger DataTrigger on lvBrushes
                BrushChangedPropToggle();

                UpdateTextboxBinding(sender);
                colorWave.ColorCaptureArea = ColorWave.CaptureArea.Screen;
            }

        }

        private void AllTextBoxes_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            bool propUpdated = UpdateProperty(sender);
            if (propUpdated)
            {

                // Toggle MainWindow's BrushChanged property to trigger DataTrigger on lvBrushes
                BrushChangedPropToggle();

                UpdateTextboxBinding(sender);

            }

        }

        private void AllTextBoxes_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            OnMouseWheelChanged(sender, e);

            // UpdateProperty returns true if the value entered is different than the property's current value.
            bool propUpdated = UpdateProperty(sender);
            if (propUpdated)
            {

                // Toggle MainWindow's BrushChanged property to trigger DataTrigger on lvBrushes
                BrushChangedPropToggle();

                UpdateTextboxBinding(sender);

            }

        }

        private void AllTextBoxes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            TextBox tboxSender = sender as TextBox;
            string tboxTag = tboxSender.Tag.ToString();

            if (tboxTag == "Red" || tboxTag == "Green" || tboxTag == "Blue" || tboxTag == "Alpha")
            {
                if (e.Text == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = !IsCharacterAllowed(e.Text);
                }
            }
            else if (tboxTag == "Hue" || tboxTag == "Saturation" || tboxTag == "Brightness" || tboxTag == "AlphaPercentage")
            {
                if (tboxSender.SelectedText.Length > 0 && tboxSender.SelectedText.Contains(".") == true && e.Text == ".")
                {
                    e.Handled = false;
                }
                if (tboxSender.Text.Contains(".") == false && e.Text == ".")
                {
                    e.Handled = false;
                }
                else if (tboxSender.SelectionLength == tboxSender.Text.Length && e.Text == ".")
                {
                    e.Handled = true;
                }
                else if (tboxSender.SelectedText.Contains(".") && e.Text == ".")
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = !IsCharacterAllowed(e.Text);
                }
            }

        }

        private void buttonProperties_Click(object sender, RoutedEventArgs e)
        {

            ShowProperties();

        }

        private void buttonPropertyBindings_Click(object sender, RoutedEventArgs e)
        {

            ShowingNonBindingProps = false;
            ShowBindings();

        }

        private void buttonColorCapture_Click(object sender, RoutedEventArgs e)
        {

            /* Calling BeginColorCapture() will run in Window mode or Screen mode based
             * on the ColorCaptureArea property. */

            // Example:
            colorWave.BeginColorCapture();

            /* DataTrigger binding to IsColorCaptureActive property in xaml handles
             * buttonColorCapture Content, Background and Foreground changes. */

        }

    }
}
