using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ThemeTutorial;

namespace PasswordGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Fill_TextBoxes(object sender, RoutedEventArgs e)
        {
            foreach(var control in passPanel.Children.OfType<TextBox>())
            {
                control.Text = GeneratePassword();
                control.Style = FindResource("PassBoxStyle2") as Style;
            }

            isCopyable = true;
        }


        // Generation settings
        private bool includeNumbers = false;
        private bool includeSymbols = false;
        private bool includeCapitals = false;
        private int length = 15;
        private bool isCopyable = false;

        // Characters for password generator
        private const string chars = "abcdefghijklmnopqrstuvwxyz";
        private const string capitals = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string numbers = "0123456789";
        private const string symbols = "!@#$%^&*()_+-=[]{}|\\:;<,>./?";



        private string GeneratePassword()
        {
            string charsCopy = new string(chars);

            if(includeCapitals)
                charsCopy = string.Concat(charsCopy, capitals);

            if (includeNumbers)
                charsCopy = string.Concat(charsCopy, numbers);

            if (includeSymbols)
                charsCopy = string.Concat(charsCopy,symbols);

            char[] password = new char[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                for (int i = 0; i < length; i++)
                {
                    password[i] = charsCopy[randomBytes[i] % charsCopy.Length];
                }
            }

            return new string(password);
        }
        
        // Allowing to drag the window
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }


        // Window manipulation buttons
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }



        private void Theme_Toggle(object sender, RoutedEventArgs e)
        {
            if(sender is ToggleButton themeToggle)
            {
                if (themeToggle.IsChecked == true)
                {
                    AppTheme.ChangeTheme(new Uri("Themes/Dark.xaml", UriKind.Relative));

                } else
                {
                    AppTheme.ChangeTheme(new Uri("Themes/Light.xaml", UriKind.Relative));
                }
            }

            // This needed to eliminate toggle button's style not matching it's state
            CheckBox[] CheckBoxes = new CheckBox[3] { Capitals, Numbers, SpecialCharacters };

            foreach (CheckBox CheckBox in CheckBoxes)
            {
                if (CheckBox.IsChecked == true)
                {
                    CheckBox.Style = FindResource("ToggleButtonChecked") as Style;
                } 
                else if (CheckBox.IsChecked == false)
                {
                    CheckBox.Style = FindResource("ToggleButtonUnchecked") as Style;
                }
            }
            
        }

        private void Toggle_Capitals(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                includeCapitals = checkBox.IsChecked ?? false;
            }
        }

        private void Toggle_SC(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                includeSymbols = checkBox.IsChecked ?? false;
            }
        }

        private void Toggle_Numbers(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                includeNumbers = checkBox.IsChecked ?? false;
            }
        }

        private void Slider_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            length = (int)e.NewValue;
        }


        private void TextBox_Copy(object sender, MouseButtonEventArgs e)
        {
            if(isCopyable && sender is TextBox TextBox)
            {
                if(e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
                {
                    Clipboard.SetText(TextBox.Text);

                    //Storyboard to show user popup (Password copied to clipboard)
                    Storyboard showPopup = Popup.Resources["Copied_Popup"] as Storyboard;
                    showPopup.Begin();
                }
            }
        }
    }
}