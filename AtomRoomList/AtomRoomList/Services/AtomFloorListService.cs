using AtomAPI.Models;
using AtomRoomList.Models;
using AtomRoomList.UserSettings;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AtomRoomList.Services
{
    public static class AtomFloorListService
    {
        public static void SetRoomsForFloors(
            IEnumerable<AtomFloor> atomFloors,
            IEnumerable<AtomRoom> atomRooms,
            AtomUserSettings atomUserSettings)
        {
            foreach (var group in atomFloors.GroupBy(x => x.TypeName))
            {
                var intersectedRoomsTotal = new List<AtomRoom>();

                foreach (var atomFloor in group)
                {
                    var intersectedRooms = atomRooms
                        .Where(x => IsFloorIntersectedRoom(atomFloor, x))
                        .ToList();

                    if (intersectedRooms.Count > 0)
                    {
                        intersectedRoomsTotal.AddRange(intersectedRooms);
                    }
                }

                foreach (var atomFloor in group)
                {
                    if (atomUserSettings.OptionsType == RoomNumbersOptionsType.Number)
                    {
                        var roomNumbers = intersectedRoomsTotal
                            .Distinct(new AtomRoomEqualityComparer())
                            .Where(x => !string.IsNullOrEmpty(x.Number))
                            .Select(x => x.Number)
                            .OrderBy(x => x, new AtomNumberComparer());

                        atomFloor.RoomNumbers = string.Join(", ", roomNumbers);
                    }
                    else if (atomUserSettings.OptionsType == RoomNumbersOptionsType.NameNumber)
                    {
                        var groupedAtomRooms = intersectedRoomsTotal
                            .Distinct(new AtomRoomEqualityComparer())
                            .Where(x => !string.IsNullOrEmpty(x.Name))
                            .GroupBy(x => x.Name);

                        var roomNumbers = new List<string>();

                        foreach (var groupedAtomRoom in groupedAtomRooms)
                        {
                            var numbers = groupedAtomRoom
                                .Where(x => !string.IsNullOrEmpty(x.Number))
                                .Select(x => x.Number)
                                .Distinct()
                                .OrderBy(x => x, new AtomNumberComparer());

                            var str = string.Join(", ", numbers);
                            if (string.IsNullOrEmpty(str))
                            {
                                roomNumbers.Add($"{groupedAtomRoom.Key}");
                            }
                            else
                            {
                                roomNumbers.Add($"{groupedAtomRoom.Key} ({str})");
                            }
                        }

                        atomFloor.RoomNumbers = string.Join(", ", roomNumbers);
                    }
                    else
                    {
                        var roomNumbers = intersectedRoomsTotal
                            .Distinct(new AtomRoomEqualityComparer())
                            .Where(x => !string.IsNullOrEmpty(x.Comments))
                            .Select(x => x.Comments);

                        atomFloor.RoomNumbers = string.Join(", ", roomNumbers);
                    }
                }
            }
        }

        private static bool IsFloorIntersectedRoom(AtomFloor atomFloor, AtomRoom atomRoom)
        {
            if (atomFloor.GeometryElement?.Any() == false || atomRoom.GeometryElement?.Any() == false)
            {
                return false;
            }

            var floorSolids = ExtractSolids(atomFloor.GeometryElement);
            var roomSolids = ExtractSolids(atomRoom.GeometryElement);

            foreach (var floorSolid in floorSolids)
            {
                foreach (var roomSolid in roomSolids)
                {
                    if (GetSolidsIntersection(floorSolid, roomSolid)?.Volume > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static IList<Solid> ExtractSolids(GeometryElement geometryElement)
        {
            double minSolidVolume = 0.0001;
            var solids = new List<Solid>();

            foreach (var geometryObject in geometryElement)
            {
                if (geometryObject is Solid solid && solid.Volume > minSolidVolume)
                {
                    solids.Add(solid);
                }
                else if (geometryObject is GeometryInstance instance)
                {
                    var instanceGeometry = instance.GetInstanceGeometry();
                    foreach (var instanceGeometryObject in instanceGeometry)
                    {
                        if (instanceGeometryObject is Solid instSolid && instSolid.Volume > minSolidVolume)
                        {
                            solids.Add(instSolid);
                        }
                    }
                }
            }

            return solids;
        }

        private static Solid GetSolidsIntersection(Solid roomSolid, Solid floorSolid)
        {
            try
            {
                var intersection = BooleanOperationsUtils.ExecuteBooleanOperation(roomSolid, floorSolid, BooleanOperationsType.Intersect);
                if (intersection?.Volume > 0)
                {
                    return intersection;
                }

                return null;
            }
            catch (Autodesk.Revit.Exceptions.InvalidOperationException)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}