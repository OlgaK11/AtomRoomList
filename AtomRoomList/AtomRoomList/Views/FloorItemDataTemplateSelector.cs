using AtomRoomList.Models;
using System.Windows;
using System.Windows.Controls;

namespace AtomRoomList.Views
{
    public class FloorItemDataTemplateSelector : DataTemplateSelector
    {
        public HierarchicalDataTemplate HeaderDataTemplate { get; set; }
        public HierarchicalDataTemplate ItemDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AtomFloorType floorItemType)
            {
                if (floorItemType.Items.Count == 0)
                {
                    return ItemDataTemplate;
                }

                return HeaderDataTemplate;
            }

            return HeaderDataTemplate;
        }
    }
}
