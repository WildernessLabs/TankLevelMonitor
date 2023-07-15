﻿using Meadow;
using Meadow.Foundation.Displays;

namespace TankLevelMonitor_UI
{
    public class MeadowApp : App<Windows>
    {
        private WinFormsDisplay _display = default!;
        private DisplayController _displayController;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");

            _display = new WinFormsDisplay(width: 320, height: 240);

            _displayController = new DisplayController(_display);

            return Task.CompletedTask;
        }

        public override Task Run()
        {
            Console.WriteLine("Run...");

            //_displayController.ShowSplashScreen();
            _displayController.VolumePercent = 100;
            //_displayController.Update();

            Application.Run(_display);

            return Task.CompletedTask;
        }

        public static async Task Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            await MeadowOS.Start(args);
        }
    }
}