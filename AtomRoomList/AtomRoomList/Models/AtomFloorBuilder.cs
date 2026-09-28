using AtomRoomList.Revit;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AtomRoomList.Models
{
    public class AtomFloorBuilder
    {
        private readonly IList<Element> _systemLevels;
        private readonly Options _options;

        public AtomFloorBuilder(IList<Element> systemLevels)
        {
            _systemLevels = systemLevels;
            _options = new Options()
            {
                DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = false
            };
        }

        public AtomFloor BuildAtomFloor(Element systemFloor)
        {
            var atomFloor = new AtomFloor();
            atomFloor.ElementId = systemFloor.Id;
            atomFloor.TypeName = GetFloorTypeName(systemFloor);
            atomFloor.AtomLevel = GetAtomLevel(systemFloor);
            atomFloor.GeometryElement = systemFloor.get_Geometry(_options);

            return atomFloor;
        }

        private string GetFloorTypeName(Element systemFloor)
        {
            var floorType = RevitData.RevitDocument.GetElement(systemFloor.GetTypeId());
            if (floorType == null)
            {
                return null;
            }

            return floorType.Name;
        }

        private AtomLevel GetAtomLevel(Element element)
        {
            var levelParamValue = element.get_Parameter(Guids.LevelParamGuid)?.AsString();

            if (levelParamValue != null && levelParamValue != string.Empty)
            {
                var elevation = GetLevelElevation(levelParamValue);
                return new AtomLevel(levelParamValue, elevation);
            }

            return new AtomLevel(Constants.EmptyResult, double.MinValue);
        }

        private double GetLevelElevation(string levelParamValue)
        {
            var level = _systemLevels.FirstOrDefault(l => l.get_Parameter(Guids.LevelParamGuid)?.AsString() == levelParamValue);
            if (level == null)
            {
                return double.MinValue;
            }

            var levelElevation = (level as Level).Elevation;
            return levelElevation;
        }
    }
}