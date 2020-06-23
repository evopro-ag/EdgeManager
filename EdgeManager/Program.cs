using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using EdgeManager.Gui;
using EdgeManager.Gui.ViewModels;
using EdgeManager.Gui.Views;
using EdgeManager.Interfaces.Commons;
using EdgeManager.Interfaces.Logging;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using EdgeManager.Logic;
using log4net;
using Microsoft.VisualBasic;
using Ninject;

namespace EdgeManager
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using (IKernel kernel = new StandardKernel())
            {
                try
                {
                    CreateLogger();

                    var logger = LoggerFactory.GetLogger();

                    logger.Debug("#######################################");
                    logger.Debug("Loding modules...");
                    LoadModules(kernel);
                    logger.Debug("Modules loaded!");

                    logger.Debug("Creating application ...");

                    var viewModelFactory = kernel.Get<ViewModelLocator>();
                    var application = CreateApplication(viewModelFactory);

                    var mainWindow = new MainWindow();
                    logger.Debug("Application created!");


                    logger.Debug("#######################################");

                    logger.Debug("application starts ...");

                    application.Run(mainWindow);
                    application.Shutdown();

                    logger.Debug("application ended.\n\n\n\n");

                }
                catch (Exception e)
                {
                    LoggerFactory.GetLogger(typeof(Program)).Error("Unhandled exeption", e);
                }
            }

            //    LoadModules(kernel);

            //    ////create azure clie
            //    //            IAzureService azure = kernel.Get<IAzureService>();

            //    //GetAllIoTHubInfo(azure).Wait();
            //}

            //using (var azure = new AzureCliHost())
            //{
            //var list = await azure.Run<IoTHubInfo[]>("iot hub list");

            //await azure.CallMethod("ping", "evoproTestHub", "IoT_Edge_One", "$edgeAgent", new DirectMethodPayloadBase()).Dump();
            //return;

        }

        private static Application CreateApplication(IViewModelFactory viewModelLocator)
        {
            var application = new App() { ShutdownMode = ShutdownMode.OnLastWindowClose };

            application.InitializeComponent();
            application.ReplaceViewModelLocator(viewModelLocator);

            return application;
        }

        private static void CreateLogger()
        {
            var loggerRepository = LogManager.CreateRepository("EdgeManager");
            log4net.Config.XmlConfigurator.Configure(loggerRepository, new FileInfo("log4net.config"));
            
        }

        //private static async Task GetAllIoTHubInfo(IAzureService azure)
        //{
        //    var list = await azure.GetIoTHubs();
        //    Console.WriteLine(list);
        //    foreach (var hub in list)
        //    {
        //        var deviceList = await azure.GetIoTDevices(hub.Name);
        //        deviceList.Dump($"Devices for {hub.Name}");
        //        foreach (var device in deviceList)
        //        {
        //            if (device.Capabilities.IoTEdge)
        //            {
        //                var moduleList = await azure.GetIoTModules(hub.Name, device.DeviceId);
        //                moduleList.Dump($"Modules for {device.DeviceId}");
        //                foreach (var modul in moduleList)
        //                {
        //                    var methodPayload = await azure.CallMethod("ping", hub.Name, device.DeviceId, modul.ModuleId, new DirectMethodPayloadBase());
        //                    methodPayload.Dump($"Ping Status for {modul.ModuleId}");
        //                }
        //            }
        //        }
        //    }
        //}

        private static void LoadModules(IKernel kernel)
        {
            kernel.Load<LogicModuleCatalog>();
        }
    }
}
