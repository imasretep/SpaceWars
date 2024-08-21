using System;
using System.Windows;
using System.Windows.Controls;


namespace SpaceWars
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public string userName { get; set; }
        public event EventHandler LoginButtonClicked;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Login_Button(object sender, RoutedEventArgs e)
        {
            if (txtName.Text != string.Empty)
            {
                userName = txtName.Text;
                LoginButtonClicked?.Invoke(this, EventArgs.Empty);
                btnEnter.Visibility = Visibility.Collapsed;
                lblLoading.Visibility = Visibility.Visible;
            }
        }
    }
}
