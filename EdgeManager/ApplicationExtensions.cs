using System;
using System.Linq;
using System.Windows;
using EdgeManager.Interfaces.Commons;

namespace EdgeManager
{
    public static class ApplicationExtensions
    {
        public static void ReplaceViewModelLocator(this Application application, IViewModelFactory viewModelLocator, string locatorKey = "Locator")
        {
            if (application.Resources.Contains(locatorKey))
            {
                application.Resources.Remove(locatorKey);
            }

            application.Resources.Add(locatorKey, viewModelLocator);
        }
    }
}