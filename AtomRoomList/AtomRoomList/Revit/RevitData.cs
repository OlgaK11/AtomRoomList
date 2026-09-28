using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AtomRoomList.Revit
{
    public static class RevitData
    {
        public static UIApplication RevitUIApplication { get; private set; }
        public static UIDocument RevitActiveUIDocument { get; private set; }
        public static Document RevitDocument { get; private set; }

        public static void Initialize(ExternalCommandData commandData)
        {
            RevitUIApplication = commandData.Application;
            RevitActiveUIDocument = commandData.Application.ActiveUIDocument;
            RevitDocument = commandData.Application.ActiveUIDocument.Document;
        }

        public static void Refresh(UIApplication uiapp)
        {
            RevitUIApplication = uiapp;
            RevitActiveUIDocument = uiapp.ActiveUIDocument;
            RevitDocument = uiapp.ActiveUIDocument.Document;
        }
    }
}