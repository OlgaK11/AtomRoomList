using System.Collections.Generic;

namespace AtomRoomList.Models
{
    public class AtomFloorType : AtomItemBase
    {
        public string Name { get; set; }
        public IList<AtomFloor> AtomFloors { get; set; }
        
        public AtomFloorType() : base()
        {
            AtomFloors = new List<AtomFloor>();
        }
    }
}
