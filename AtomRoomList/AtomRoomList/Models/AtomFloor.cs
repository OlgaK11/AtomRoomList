using Autodesk.Revit.DB;

namespace AtomRoomList.Models
{
    public class AtomFloor
    {
        public ElementId ElementId { get; set; }
        public string TypeName { get; set; }
        public AtomLevel AtomLevel { get; set; }
        public string RoomNumbers { get; set; }
        public GeometryElement GeometryElement { get; set; }
    }
}
