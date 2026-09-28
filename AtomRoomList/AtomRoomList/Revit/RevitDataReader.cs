using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AtomRoomList.Revit
{
    public static class RevitDataReader
    {
        public static IEnumerable<Element> GetSystemLevels(Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Levels);
        }

        public static IEnumerable<Element> GetRooms(Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Rooms);
        }

        public static IEnumerable<Element> GetFloors(Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Floors);
        }

        public static IEnumerable<Element> GetFinishingFloors(IList<ElementId> elementIds, Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Floors)
                .Where(x => elementIds.Contains(x.GetTypeId()));
        }

        public static IEnumerable<ElementId> GetFinishingFloorTypeIds(Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .OfCategory(BuiltInCategory.OST_Floors)
                .WherePasses(GetFloorGroupModelParameterFilter())
                .ToElementIds();
        }

        private static ElementParameterFilter GetFloorGroupModelParameterFilter()
        {
            var builtInParameter = BuiltInParameter.ALL_MODEL_MODEL;
            var parameterValueProvider = new ParameterValueProvider(new ElementId(builtInParameter));
            var filterStringRuleEvaluator = new FilterStringEquals();
            var ruleString = "Полы";

            FilterRule filterRule = new FilterStringRule(parameterValueProvider, filterStringRuleEvaluator, ruleString, false);

            return new ElementParameterFilter(filterRule);
        }
    }
}