using AtomAPI.Abstractions;
using AtomAPI.Models;
using System.Collections.Generic;

namespace AtomRoomList
{
    public class AtomConnector : IAtomPlugin, IAtomPluginGeneral, IAtomRibbonSettings
    {
        private static AtomConnector _instance;

        public static AtomConnector Instance => _instance ?? (_instance = new AtomConnector());

        public AtomProduct Product => AtomProduct.Revit;
        public string Name => "AtomRoomList";
        public string FullClassName => "AtomRoomList.Commands.AtomRoomList";
        public string AvailabilityClassName => "AtomRoomList.Commands.CommandAvailability";
        public string ToolTipHelpImage => string.Empty;
        public List<string> SubCommandNames => new List<string>();
        public List<string> SubCommandFullClassNames => new List<string>();
        public List<string> SubCommandAvailabilityClassNames => new List<string>();
        public List<string> SubCommandHelpImages => new List<string>();
        public bool BuildOnlySubCommands => false;
        public bool CanAddToRibbon => true;
        public bool IsPaid => false;
        public bool IsSynchronizedWithCurrentItem => false;
    }
}