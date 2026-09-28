using AtomRoomList.Models;
using AtomRoomList.ViewModels;
using AtomStyle.Controls;
using AtomStyle.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AtomRoomList.Views
{
    public partial class UndefinedFloorsBox : AtomWindow
    {
        private readonly UndefinedFloorsVM _undefinedFloorsVM;

        public UndefinedFloorsBox(UndefinedFloorsVM undefinedFloorsVM)
        {
            InitializeComponent();
            DataContext = undefinedFloorsVM;
            _undefinedFloorsVM = undefinedFloorsVM;

            WindowHandleSearch.MainWindowHandle.SetAsOwner(this);
        }

        private void AtomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CloseButton.Click -= CloseButton_Click;
            CloseButton.Click += CloseButton_Click;

            _undefinedFloorsVM.CloseEvent += Close;
        }

        private void FloorsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItems = floorsDataGrid.SelectedItems?.Cast<AtomFloor>();
                _undefinedFloorsVM.SelectedAtomFloors = new ObservableCollection<AtomFloor>(selectedItems);

                if (_undefinedFloorsVM.IsDynamicSelection && _undefinedFloorsVM.SelectedAtomFloors.Any())
                {
                    _undefinedFloorsVM.SelectInModel();
                }
            }
            catch { }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e) => floorsDataGrid.SelectAll();

        private void UnselectAllButton_Click(object sender, RoutedEventArgs e) => floorsDataGrid.UnselectAll();

        protected override void CloseButton_Click(object sender, RoutedEventArgs e) => _undefinedFloorsVM.Close(sender, e);
    }
}