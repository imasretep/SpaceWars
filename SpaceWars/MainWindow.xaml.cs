using SpaceWars.SignalRService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SpaceWars
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginPage loginPage;
        private BattlePage battlePage;
        private SignalRServer server;
        private string userName;
        public MainWindow()
        {
            InitializeComponent();
            InitializeLoginPage();
            //InitializeBattlePage();
            

        }

        private async void InitializeBattlePage()
        {
            userName = loginPage.userName;
            server = new SignalRServer();
            await server.ConnectToServer();
            battlePage = new BattlePage(server, this, userName);
            gameField.Content = battlePage;

        }

        private void InitializeLoginPage()
        {
            loginPage = new LoginPage();
            loginPage.LoginButtonClicked += OnLoginButtonClicked;
            gameField.Content = loginPage;
        }

        private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            InitializeBattlePage();
        }
    }
}
