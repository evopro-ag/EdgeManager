using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EdgeManager.Interfaces;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;
using EdgeManager.Interfaces.Settings;
using log4net;
using Newtonsoft.Json;
using Ninject;

namespace EdgeManager.Logic.Services
{
    public class SettingsService : IDisposable, ISettingsService
    {
        private readonly ILog logger;
        private string settingsFilepath;

        public SettingsService(ILog logger, IDirectoryService directoryService)
        {
            this.logger = logger;
            settingsFilepath = Path.Combine(directoryService.LocalDataPath, Constants.SettingsFilename);
            Settings = new ApplicationSettings();
            RestoreSettings();
        }

        private void RestoreSettings()
        {
            try
            {

                if (File.Exists(settingsFilepath))
                {
                    logger.Debug($"Restoring settings from file: '{settingsFilepath}'");

                    var text = File.ReadAllText(settingsFilepath);

                    Settings = JsonConvert.DeserializeObject<ApplicationSettings>(text);
                }
                else
                {
                    logger.Debug($"Creating default settings into file: '{settingsFilepath}'");
                    SaveSettings();
                }
            }
            catch(Exception e)
            {
                logger.Error("error while restoring cache", e);
            }
        }

        public ApplicationSettings Settings { get; set; }

        private void SaveSettings()
        {
            try
            {
                logger.Debug($"saving cache into file '{settingsFilepath}'");
                File.WriteAllText(settingsFilepath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
            catch (Exception e)
            {
                logger.Error("error while saving cache", e);
            }
        }

        public void Dispose()
        {
            SaveSettings();
        }
    }
}