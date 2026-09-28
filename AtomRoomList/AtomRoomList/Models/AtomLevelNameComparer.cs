using System.Collections.Generic;

namespace AtomRoomList.Models
{
    public class AtomLevelNameComparer : IEqualityComparer<AtomLevel>
    {
        public bool Equals(AtomLevel x, AtomLevel y)
        {
            if (x.Name.Equals(y.Name))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(AtomLevel obj)
        {
            return GetHashCode();
        }
    }
}
