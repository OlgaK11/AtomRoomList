using AtomRoomList.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AtomRoomList.UserSettings
{
    public class AtomUserSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RoomNumbersOptionsType OptionsType { get; set; } = RoomNumbersOptionsType.NameNumber;
    }
}
