using System;
using System.IO;
using EdgeManager.Interfaces;
using EdgeManager.Interfaces.Services;
using log4net;
using Ninject;

namespace EdgeManager.Logic.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly ILog logger;

        public DirectoryService(ILog logger)
        {
            this.logger = logger;
            Initialize();
        }

        public string LocalDataPath { get; set; }

        private void Initialize()
        {
            var localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            LocalDataPath = Path.Combine(localData, Constants.AppDataFolderName);
            if (!Directory.Exists(LocalDataPath))
            {
                logger.Debug($"Creating local data folder: {LocalDataPath}");
                Directory.CreateDirectory(LocalDataPath);
            }
        }
    }
}