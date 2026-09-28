using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AtomRoomList.Commands
{
    public class CommandAvailability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return IsCommandAvailable(applicationData, true, false, false);
        }

        private bool IsCommandAvailable(
            UIApplication applicationData,
            bool disableInFamilyDocument,
            bool disableInProjectDocument,
            bool disableInSchedules)
        {
            UIDocument uiDocument;

            try
            {
                uiDocument = (applicationData?.ActiveUIDocument);
            }
            catch
            {
                return false;
            }

            if (uiDocument == null)
            {
                return false;
            }

            if (uiDocument == null
                || uiDocument.Document.IsLinked
                || disableInFamilyDocument
                && uiDocument.Document.IsFamilyDocument
                || disableInProjectDocument
                && !uiDocument.Document.IsFamilyDocument)
            {
                return false;
            }

            var flag = disableInSchedules;
            if (flag)
            {
                var viewType = uiDocument.ActiveGraphicalView.ViewType;
                flag = (int)viewType == 5 || (int)viewType == 122 || (int)viewType == 123;
            }

            if (flag)
            {
                return false;
            }

            return true;
        }
    }
}