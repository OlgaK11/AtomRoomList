using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AtomRoomListProxy
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class ParentProxy : IExternalCommand
    {
        private static string _сommandName;

        public static Result SetParentProxy(string commandName)
        {
            _сommandName = commandName;
            return Result.Succeeded;
        }

        public static void InvokeProcess(ExternalCommandData commandData, ref string message, ElementSet elements, string commandName)
        {
            var revitVersion = commandData.Application.Application.CurrentUserAddinsLocation.ToString();
            revitVersion = "_" + revitVersion.Substring(revitVersion.Length - 4, 4);

            var currentAssembly = Assembly.GetExecutingAssembly();
            var pluginAssemblyName = currentAssembly.Location.Replace("Proxy", revitVersion);

            var assemblyBytes = File.ReadAllBytes(pluginAssemblyName);
            var assemblyPlugin = Assembly.Load(assemblyBytes);

            foreach (var type in assemblyPlugin.GetTypes())
            {
                if ((type.IsClass == true) & (type.Name == commandName))
                {
                    var ibaseObject = Activator.CreateInstance(type);
                    var arguments = new object[] { commandData, message, elements };
                    var result = type.InvokeMember("Execute", BindingFlags.Default | BindingFlags.InvokeMethod, null, ibaseObject, arguments);
                }
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            LoadAssemblies();
            InvokeProcess(commandData, ref message, elements, _сommandName);
            return Result.Succeeded;
        }

        private void LoadAssemblies()
        {
            var pluginExtensions = GetPluginExtensions();

            pluginExtensions.TryGetValue("AtomAPI", out string atomAPIPath);
            pluginExtensions.TryGetValue("AtomStyle", out string atomStylePath);
            pluginExtensions.TryGetValue("AtomPluginServices", out string atomPluginServicesPath);

            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var atomAPIAssembly = assemblies.FirstOrDefault(x => x.GetName().Name == "AtomAPI");
                var atomStyleAssembly = assemblies.FirstOrDefault(x => x.GetName().Name == "AtomStyle");
                var atomPluginServicesAssembly = assemblies.FirstOrDefault(x => x.GetName().Name == "AtomPluginServices");

                if (atomAPIAssembly == null)
                {
                    Assembly.LoadFrom(atomAPIPath ?? @"P:\03_БИБЛИОТЕКА\Revit_5_ПСМ\Скрипты C#\Библиотеки\AtomLibrary\AtomAPI.dll");
                }

                if (atomStyleAssembly == null)
                {
                    Assembly.LoadFrom(atomStylePath ?? @"P:\03_БИБЛИОТЕКА\Revit_5_ПСМ\Скрипты C#\Библиотеки\AtomStyleLibrary\AtomStyle.dll");
                }

                if (atomPluginServicesAssembly == null)
                {
                    Assembly.LoadFrom(atomPluginServicesPath ?? @"P:\03_БИБЛИОТЕКА\Revit_5_ПСМ\Скрипты C#\Библиотеки\AtomLibrary\AtomPluginServices.dll");
                }
            }
            catch { }
        }

        private Dictionary<string, string> GetPluginExtensions()
        {
            var pluginExtensions = new Dictionary<string, string>();

            var pluginExtensionsCommonDirectory = @"P:\03_БИБЛИОТЕКА\Revit_5_ПСМ\Скрипты C#\Библиотеки\AtomExtensionLibraries\";
            var pluginExtensionsFileName = "AtomPluginExtensions.txt";

            var location = Assembly.GetExecutingAssembly().Location;
            var localExtensionsFile = Path.Combine(location, pluginExtensionsFileName);
            var commonExtensionsFile = Path.Combine(pluginExtensionsCommonDirectory, pluginExtensionsFileName);

            // Сначала проверим расположение файла у самой сборки
            if (File.Exists(localExtensionsFile))
            {
                var lines = File.ReadAllLines(localExtensionsFile);
                var extensionLines = lines.Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line)).ToList();

                foreach (var extensionLine in extensionLines)
                {
                    var extensionLineSplitted = extensionLine.Split(';');
                    var name = extensionLineSplitted[0];
                    var path = extensionLineSplitted[1];

                    pluginExtensions.Add(name, path);
                }
            }

            // Если рядом со сборкой файла нет, проверим в общедоступном месте
            else if (File.Exists(commonExtensionsFile))
            {
                var lines = File.ReadAllLines(commonExtensionsFile);
                var extensionLines = lines.Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line)).ToList();

                foreach (var extensionLine in extensionLines)
                {
                    var extensionLineSplitted = extensionLine.Split(';');
                    var name = extensionLineSplitted[0];
                    var path = extensionLineSplitted[1];

                    pluginExtensions.Add(name, path);
                }
            }

            return pluginExtensions;
        }
    }

    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class AtomRoomListStartCommand : ParentProxy
    {
        private Result result = SetParentProxy("AtomRoomListStartCommand");
    }
}