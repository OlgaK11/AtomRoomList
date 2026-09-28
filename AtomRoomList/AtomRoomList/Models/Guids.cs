using AtomAPI.Models;
using System;

namespace AtomRoomList.Models
{
    public class Guids
    {
        public static readonly Guid LevelParamGuid = AtomSharedParameters.Parameters["_Этаж"];
        public static readonly Guid RoomListParamGuid = AtomSharedParameters.Parameters["_Список помещений"];
    }
}