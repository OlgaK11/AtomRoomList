using AtomPluginServices;
using AtomRevitAPI.RevitAsync;
using AtomRoomList.Revit;
using AtomRoomList.Services;
using AtomRoomList.ViewModels;
using AtomRoomList.Views;
using AtomStyle.Controls;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace AtomRoomList.Commands
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class AtomRoomList : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitTask.Initialize(commandData.Application);

            RevitData.Initialize(commandData);

            Monitoring.PostPluginLaunchAsync();

            var logger = Logger.CreateLogger();

            try
            {
                if (!RevitModelChecker.HasData(RevitData.RevitDocument))
                {
                    AtomWindows.ShowAtomResultBox("Полы в модели не найдены", "Предупреждение", AtomWindow.MessageTypes.Warning);
                    return Result.Cancelled;
                }

                var baseVM = new BaseVM();

                var mainWindow = new MainWindow(baseVM);
                mainWindow.Show();
            }
            catch (Exception e)
            {
                TaskDialog.Show("ОШИБКА", e.Message);

                logger.LogCritical("Ошибка при запуске", e);
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}