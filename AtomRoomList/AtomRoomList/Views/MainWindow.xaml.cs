using AtomRoomList.ViewModels;
using AtomStyle.Controls;
using AtomStyle.Helpers;
using System.Windows;

namespace AtomRoomList.Views
{
    public partial class MainWindow : AtomWindow
    {
        private readonly BaseVM _baseVM;

        public MainWindow(BaseVM baseVM)
        {
            InitializeComponent();
            _baseVM = baseVM;
            DataContext = baseVM;

            WindowHandleSearch.MainWindowHandle.SetAsOwner(this);
        }

        private void AtomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CloseButton.Click -= CloseButton_Click;
            CloseButton.Click += CloseButton_Click;

            _baseVM.OwnerWindow = this;
            _baseVM.CloseEvent += Close;
        }

        protected override void CloseButton_Click(object sender, RoutedEventArgs e) => _baseVM.Close(sender, e);

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) => e.Handled = true;
    }
}