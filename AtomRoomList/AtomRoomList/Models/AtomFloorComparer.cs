using System.Collections.Generic;

namespace AtomRoomList.Models
{
    public class AtomFloorComparer : IEqualityComparer<AtomFloor>
    {
        public bool Equals(AtomFloor x, AtomFloor y)
        {
            if (x.TypeName.Equals(y.TypeName))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(AtomFloor obj)
        {
            return GetHashCode();
        }
    }
}
