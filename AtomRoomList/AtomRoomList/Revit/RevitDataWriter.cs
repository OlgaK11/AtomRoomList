using AtomPluginServices;
using AtomPluginServices.Interface;
using AtomRoomList.Models;
using AtomRoomList.Services;
using AtomStyle.Controls;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace AtomRoomList.Revit
{
    public class RevitDataWriter
    {
        private readonly IAtomPluginLogger _logger = Logger.CreateLogger();

        public TransactionStatus TransactionStatus;

        public void WriteFloorList(IEnumerable<AtomFloor> atomFloors, IList<string> levelNames)
        {
            using (var t = new Transaction(RevitData.RevitDocument, "АСК: Запись списка помещений"))
            {
                t.Start();

                try
                {
                    foreach (var atomFloor in atomFloors)
                    {
                        if (!string.IsNullOrEmpty(atomFloor.RoomNumbers) && levelNames.Contains(atomFloor.AtomLevel.Name))
                        {
                            WriteParameter(atomFloor.ElementId, Guids.RoomListParamGuid, atomFloor.RoomNumbers);
                        }
                    }

                    t.Commit();
                }
                catch (Exception e)
                {
                    AtomWindows.ShowAtomExceptionBox(e);
                    _logger.LogCritical("Ошибка записи параметров в проект", e);

                    t.RollBack();
                }
                finally
                {
                    TransactionStatus = t.GetStatus();
                }
            }
        }

        private void WriteParameter(ElementId elementId, Guid paramGuid, string valueToWrite)
        {
            RevitData.RevitDocument.GetElement(elementId)
                .get_Parameter(paramGuid)?
                .Set(valueToWrite);
        }
    }
}