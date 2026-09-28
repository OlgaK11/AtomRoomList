using AtomAPI.MVVM;
using AtomRevitAPI.RevitAsync;
using AtomRoomList.Models;
using AtomRoomList.Revit;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AtomRoomList.ViewModels
{
    public class UndefinedFloorsVM : BindableBase
    {
        #region ПОЛЯ

        private IList<AtomFloor> _selectedAtomFloors = new ObservableCollection<AtomFloor>();
        private bool _isDynamicSelection;

        #endregion ПОЛЯ

        #region СВОЙСТВА

        public IList<AtomFloor> AtomFloors { get; }

        public IList<AtomFloor> SelectedAtomFloors
        {
            get => _selectedAtomFloors;
            set
            {
                SetProperty(ref _selectedAtomFloors, value);
                SelectInModelDelegateCommand?.RaiseCanExecuteChanged();
                SelectionBoxDelegateCommand?.RaiseCanExecuteChanged();
            }
        }

        public bool IsDynamicSelection
        {
            get => _isDynamicSelection;
            set
            {
                _isDynamicSelection = value;
                if (_isDynamicSelection == true && SelectedAtomFloors.Any())
                {
                    SelectInModel();
                }
            }
        }

        #endregion СВОЙСТВА

        #region КОМАНДЫ

        public DelegateCommand SelectInModelDelegateCommand { get; private set; }
        public DelegateCommand SelectionBoxDelegateCommand { get; private set; }

        #endregion КОМАНДЫ

        #region СОБЫТИЯ

        public event Action CloseEvent;

        #endregion СОБЫТИЯ

        public UndefinedFloorsVM(IList<AtomFloor> atomFloors)
        {
            InitializeCommands();

            AtomFloors = atomFloors;
        }

        public void SelectInModel()
        {
            try
            {
                var elementIds = SelectedAtomFloors.Select(x => x.ElementId).ToList();
                RevitData.RevitUIApplication.ActiveUIDocument.Selection.SetElementIds(elementIds);
            }
            catch { }
        }

        public async void SelectionBoxAsync()
        {
            await RevitTask.RunAsync(() =>
            {
                try
                {
                    var elementIds = SelectedAtomFloors.Select(x => x.ElementId).ToList();
                    RevitData.RevitUIApplication.ActiveUIDocument.Selection.SetElementIds(elementIds);

                    var selectionBoxPostableCommand = RevitCommandId.LookupPostableCommandId(PostableCommand.SelectionBox);
                    RevitData.RevitUIApplication.PostCommand(selectionBoxPostableCommand);
                }
                catch { }
            });
        }

        public void Close(object sender, EventArgs e) => CloseEvent?.Invoke();

        private void InitializeCommands()
        {
            SelectInModelDelegateCommand = new DelegateCommand(SelectInModel, () => SelectedAtomFloors?.Count > 0);
            SelectionBoxDelegateCommand = new DelegateCommand(SelectionBoxAsync, () => SelectedAtomFloors?.Count > 0);
        }
    }
}