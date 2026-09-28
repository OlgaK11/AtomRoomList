namespace AtomRoomList.Models
{
    public class AtomLevel : AtomItemBase
    {
        public string Name { get; set; }
        public double Elevation { get; set; }

        public AtomLevel(string name, double elevation) : base()
        {
            Name = name;
            Elevation = elevation;
        }
    }
}