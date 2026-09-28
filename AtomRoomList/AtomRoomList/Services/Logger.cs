using AtomPluginServices;
using AtomPluginServices.Interface;
using AtomRoomList.Revit;
using System;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AtomRoomList.Services
{
    public class Logger
    {
        public static IAtomPluginLogger CreateLogger([CallerFilePath] string className = "")
        {
            var loggerFactory = new AtomPluginLoggerFactory<AtomPluginLoggerProvider>();
            var pluginGuid = new Guid(Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>()?.Value ?? string.Empty);
            var pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var userName = RevitData.RevitUIApplication.Application.Username;
            var projectName = RevitData.RevitDocument.Title;
            var userDisplayName = UserPrincipal.Current?.DisplayName;
            var programName = "Revit";
            var programVersion = RevitData.RevitUIApplication.Application.VersionNumber;

            return loggerFactory.CreateLogger(pluginGuid, pluginVersion, projectName, className, userName, userDisplayName, programName, programVersion);
        }
    }
}