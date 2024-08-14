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
        private BattlePage battlePage;
        private SignalRServer server;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBattlePage();

        }

        private async void InitializeBattlePage()
        {
            server = new SignalRServer();
            await server.ConnectToServer();
            battlePage = new BattlePage(server, this);
            gameField.Content = battlePage;

        }
    }
}
