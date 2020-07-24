using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using EdgeManager.Gui;
using EdgeManager.Gui.ViewModels;
using EdgeManager.Gui.Views;
using EdgeManager.Interfaces.Commons;
using EdgeManager.Interfaces.Logging;
using EdgeManager.Logic;
using log4net;
using log4net.Repository.Hierarchy;
using Ninject;
using Ninject.Activation.Strategies;

namespace EdgeManager
{
    public static class Program
    {
        private static IDisposable subscription;

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
                    logger.Debug("Loading modules...");
                    LoadModules(kernel);
                    logger.Debug("Modules loaded!");

                    logger.Debug("Creating application ...");

                    var viewModelFactory = kernel.Get<ViewModelLocator>();
                    var application = CreateApplication(viewModelFactory);

                    var mainWindow = new MainWindow();
                    logger.Debug("Application created!");


                    logger.Debug("#######################################");

                    logger.Debug("application starts ...");

                    var mainWindowViewModel = viewModelFactory.MainWindowViewModel;
                    subscription = mainWindowViewModel.RestartApplication
                        .Do(_ => Console.WriteLine("Trigger"))
                        .Subscribe(_ => Restart(application));

                    application.Run(mainWindow);
                    application.Shutdown();


                    logger.Debug("application ended.\n\n\n\n");

                }
                catch (Exception e)
                {
                    LoggerFactory.GetLogger(typeof(Program)).Error("Unhandled exception", e);
                }
                finally
                {
                    
                    subscription?.Dispose();
                }
            }
        }

        private static void Restart(Application app)
        {
            //ToDo: Start new application
            Process.Start(Path.Combine(Path.GetDirectoryName(Application.ResourceAssembly.Location), "EdgeManager.exe"));
            //Logger.Debug("Application restarted after install AzureCli");
            try
            {
                app.Shutdown();
                //Application.Current.Shutdown();
            }
            catch (Exception e)
            {
                //Logger.Error("Error while restarting: did not shutdown", e);
            }
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

        private static void LoadModules(IKernel kernel)
        {
            kernel.Load<LogicModuleCatalog>();
        }
    }
}
