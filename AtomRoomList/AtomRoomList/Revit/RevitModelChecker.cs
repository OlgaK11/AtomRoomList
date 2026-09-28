using Autodesk.Revit.DB;
using System.Linq;

namespace AtomRoomList.Revit
{
    public static class RevitModelChecker
    {
        public static bool HasData(Document doc) => RevitDataReader.GetFloors(doc).Any();
    }
}