using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace AtomRoomList.Models
{
    public class AtomRoom
    {
        private static readonly Options _options;
        private static readonly double _floorOffset;

        public ElementId ElementId { get; }
        public string Name { get; }
        public string Number { get; }
        public string Comments { get; }
        public GeometryElement GeometryElement { get; }

        static AtomRoom()
        {
            _options = new Options()
            {
                DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = false
            };

            _floorOffset = -1.5 * 3.28084;
        }

        public AtomRoom(Element element)
        {
            ElementId = element?.Id;
            Name = GetName(element);
            Number = GetNumber(element);
            Comments = GetComments(element);
            GeometryElement = GetGeometryElement(element);
        }

        private static string GetName(Element element)
        {
            if (element == null)
            {
                return string.Empty;
            }

            var param = element.get_Parameter(BuiltInParameter.ROOM_NAME);
            if (param == null)
            {
                return string.Empty;
            }

            return string.IsNullOrEmpty(param.AsString()) ? string.Empty : param.AsString();
        }

        private static string GetNumber(Element element)
        {
            if (element == null)
            {
                return string.Empty;
            }

            var param = element.get_Parameter(BuiltInParameter.ROOM_NUMBER);
            if (param == null)
            {
                return string.Empty;
            }

            return string.IsNullOrEmpty(param.AsString()) ? string.Empty : param.AsString();
        }

        private static string GetComments(Element element)
        {
            if (element == null)
            {
                return string.Empty;
            }

            var param = element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
            if (param == null)
            {
                return string.Empty;
            }

            return string.IsNullOrEmpty(param.AsString()) ? string.Empty : param.AsString();
        }

        private static GeometryElement GetGeometryElement(Element element)
        {
            var roomGeometryElement = element.get_Geometry(_options);
            var translation = Transform.CreateTranslation(new XYZ(0, 0, _floorOffset));
            var adjustedRoomGeometryElement = roomGeometryElement.GetTransformed(translation);

            return adjustedRoomGeometryElement;
        }
    }

    public class AtomRoomEqualityComparer : IEqualityComparer<AtomRoom>
    {
        public bool Equals(AtomRoom x, AtomRoom y)
        {
            return x.Name == y.Name && x.Number == y.Number;
        }

        public int GetHashCode(AtomRoom obj)
        {
            return GetHashCode();
        }
    }
}