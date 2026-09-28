using AtomPluginServices;
using AtomRoomList.Revit;
using System;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AtomRoomList.Services
{
    public class Monitoring
    {
        public static void PostPluginLaunchAsync()
        {
            var request = GetPluginLaunchRequest();
            MonitoringService.PostPluginLaunchAsync(request);
        }

        private static AtomPluginLaunchRequest GetPluginLaunchRequest()
        {
            var pluginGuid = new Guid(Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>()?.Value ?? string.Empty);
            var pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var userName = RevitData.RevitUIApplication.Application.Username;
            var projectName = RevitData.RevitDocument.Title;
            var userDisplayName = UserPrincipal.Current?.DisplayName;
            var programName = "Revit";
            var programVersion = RevitData.RevitUIApplication.Application.VersionNumber;

            return new AtomPluginLaunchRequest(pluginGuid, pluginVersion, projectName, userName, userDisplayName, programName, programVersion);
        }
    }
}