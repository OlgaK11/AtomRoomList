using AtomAPI.MVVM;
using AtomPluginServices;
using AtomPluginServices.Interface;
using AtomRevitAPI.RevitAsync;
using AtomRoomList.Models;
using AtomRoomList.Revit;
using AtomRoomList.Services;
using AtomRoomList.UserSettings;
using AtomRoomList.Views;
using AtomStyle.Controls;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtomLevel = AtomRoomList.Models.AtomLevel;

namespace AtomRoomList.ViewModels
{
    public class BaseVM : BindableBase
    {
        #region ПОЛЯ

        private IList<Element> _systemLevels;
        private IList<AtomFloor> _atomFloors;
        private bool _numberOptionType;
        private bool _nameNumberOptionType;
        private bool _commentOptionType;
        private IAtomPluginLogger _logger;
        private bool _isSnackbarActive;
        private string _snackbarText;

        #endregion ПОЛЯ

        #region СВОЙСТВА

        public IList<AtomLevel> AtomLevels { get; private set; }
        public AtomUserSettings AtomUserSettings { get; set; }
        public ObservableCollection<AtomFloorType> SelectedAtomFloorTypes { get; private set; }

        public bool NumberOptionType
        {
            get => _numberOptionType;
            set => SetProperty(ref _numberOptionType, value);
        }

        public bool NameNumberOptionType
        {
            get => _nameNumberOptionType;
            set => SetProperty(ref _nameNumberOptionType, value);
        }

        public bool CommentOptionType
        {
            get => _commentOptionType;
            set => SetProperty(ref _commentOptionType, value);
        }

        public bool IsSnackbarActive
        {
            get => _isSnackbarActive;
            set => SetProperty(ref _isSnackbarActive, value);
        }

        public string SnackbarText
        {
            get => _snackbarText;
            set => SetProperty(ref _snackbarText, value);
        }

        public AtomWindow OwnerWindow { get; set; }

        #endregion СВОЙСТВА

        #region КОМАНДЫ

        public DelegateCommand SelectLevelDelegateCommand { get; private set; }
        public DelegateCommand SelectAllLevelsDelegateCommand { get; private set; }
        public DelegateCommand UnselectAllLevelsDelegateCommand { get; private set; }
        public DelegateCommand WriteDataDelegateCommand { get; private set; }

        #endregion КОМАНДЫ

        #region СОБЫТИЯ

        public event Action CloseEvent;

        #endregion СОБЫТИЯ

        public BaseVM()
        {
            InitializeFields();
            InitializeCommands();
            SetUserSettings();
            SetData();
        }

        public void Close(object sender, EventArgs e) => CloseEvent?.Invoke();

        private void InitializeFields()
        {
            _logger = Logger.CreateLogger();

            SelectedAtomFloorTypes = new ObservableCollection<AtomFloorType>();
        }

        private void InitializeCommands()
        {
            SelectLevelDelegateCommand = new DelegateCommand(() => WriteDataDelegateCommand?.RaiseCanExecuteChanged());
            SelectAllLevelsDelegateCommand = new DelegateCommand(() => SelectLevels(true));
            UnselectAllLevelsDelegateCommand = new DelegateCommand(() => SelectLevels(false));
            WriteDataDelegateCommand = new DelegateCommand(WriteDataAsync, () => AtomLevels.SelectMany(x => x.Items).Count(x => x.IsSelected == true) > 0);
        }

        private void SetUserSettings()
        {
            AtomUserSettings = AtomUserSettingsService.Read();

            if (AtomUserSettings.OptionsType == RoomNumbersOptionsType.Number)
            {
                NumberOptionType = true;
            }
            else if (AtomUserSettings.OptionsType == RoomNumbersOptionsType.NameNumber)
            {
                NameNumberOptionType = true;
            }
            else
            {
                CommentOptionType = true;
            }
        }

        private void SaveUserSettings()
        {
            if (NumberOptionType == true)
            {
                AtomUserSettings.OptionsType = RoomNumbersOptionsType.Number;
            }
            else if (NameNumberOptionType == true)
            {
                AtomUserSettings.OptionsType = RoomNumbersOptionsType.NameNumber;
            }
            else
            {
                AtomUserSettings.OptionsType = RoomNumbersOptionsType.Comment;
            }

            AtomUserSettingsService.Save(AtomUserSettings);
        }

        private void SetData()
        {
            _systemLevels = RevitDataReader.GetSystemLevels(RevitData.RevitDocument).ToList();
            _atomFloors = GetAtomFloors();

            AtomLevels = GetAtomLevels(_atomFloors);
        }

        private IList<AtomFloor> GetAtomFloors()
        {
            var atomFloors = new List<AtomFloor>();
            var finishingFloorTypeIds = RevitDataReader.GetFinishingFloorTypeIds(RevitData.RevitDocument).ToList();
            var atomFloorBuilder = new AtomFloorBuilder(_systemLevels);

            foreach (var floor in RevitDataReader.GetFinishingFloors(finishingFloorTypeIds, RevitData.RevitDocument))
            {
                var atomFloor = atomFloorBuilder.BuildAtomFloor(floor);
                atomFloors.Add(atomFloor);
            }

            return atomFloors;
        }

        private IList<AtomLevel> GetAtomLevels(IList<AtomFloor> atomFloors)
        {
            var atomLevels = atomFloors.Select(x => x.AtomLevel)
                .Distinct(new AtomLevelNameComparer())
                .OrderBy(level => level.Elevation)
                .ToList();

            foreach (var level in atomLevels)
            {
                var groupedItems = _atomFloors.Where(x => x.AtomLevel.Name == level.Name).GroupBy(x => x.TypeName);
                foreach (var groupedItem in groupedItems)
                {
                    var atomFloorType = new AtomFloorType() { Name = groupedItem.Key, AtomFloors = groupedItem.ToList(), ParentItem = level };
                    level.Items.Add(atomFloorType);
                }

                foreach (var floorType in level.Items)
                {
                    floorType.SelectedChangedEvent += (s, e) =>
                    {
                        WriteDataDelegateCommand.RaiseCanExecuteChanged();

                        var selectedAtomFloorType = s as AtomFloorType;
                        if (selectedAtomFloorType.IsSelected == true)
                        {
                            SelectedAtomFloorTypes.Add(selectedAtomFloorType);
                        }
                        else
                        {
                            SelectedAtomFloorTypes.Remove(selectedAtomFloorType);
                        }
                    };
                }

                level.Items = level.Items.OrderBy(x => (x as AtomFloorType).Name).ToList();
            }

            return atomLevels;
        }

        private IList<AtomRoom> GetAtomRooms()
        {
            var atomRooms = new List<AtomRoom>();

            foreach (var room in RevitDataReader.GetRooms(RevitData.RevitDocument))
            {
                atomRooms.Add(new AtomRoom(room));
            }

            return atomRooms;
        }

        private async void WriteDataAsync()
        {
            await RevitTask.RunAsync((app) =>
            {
                RevitData.Refresh(app);
                SaveUserSettings();

                var atomRooms = GetAtomRooms();

                var levelNames = AtomLevels
                    .Where(x => x.IsSelected == true || x.IsSelected == null)
                    .Select(x => x.Name)
                    .ToList();

                AtomFloorListService.SetRoomsForFloors(SelectedAtomFloorTypes.SelectMany(x => x.AtomFloors).Where(x => levelNames.Contains(x.AtomLevel.Name)), atomRooms, AtomUserSettings);

                var revitDataWriter = new RevitDataWriter();
                revitDataWriter.WriteFloorList(SelectedAtomFloorTypes.SelectMany(x => x.AtomFloors), levelNames);

                if (revitDataWriter.TransactionStatus == TransactionStatus.Committed)
                {
                    ShowSnackbarMessage("Списки помещений определены");

                    _logger.LogInformation("Внесены изменения в модель", $"Определены списки помещений для полов. Количество полов: {SelectedAtomFloorTypes.Count}");
                }
                else
                {
                    OwnerWindow?.ShowAtomResultBox("Изменения не внесены", "Предупреждение", AtomWindow.MessageTypes.Alert);
                }

                var undefinedAtomFloors = SelectedAtomFloorTypes
                    .SelectMany(x => x.AtomFloors)
                    .Where(x => levelNames.Contains(x.AtomLevel.Name))
                    .Where(x => string.IsNullOrEmpty(x.RoomNumbers))
                    .ToList();

                if (undefinedAtomFloors.Any())
                {
                    var undefinedFloorsVM = new UndefinedFloorsVM(undefinedAtomFloors.OrderBy(x => x.AtomLevel.Elevation).ThenBy(x => x.TypeName).ToList());

                    var undefinedFloorsBox = new UndefinedFloorsBox(undefinedFloorsVM);
                    undefinedFloorsBox.Show();
                }
            });
        }

        private void SelectLevels(bool isSelected)
        {
            foreach (var atomLevel in AtomLevels)
            {
                if (atomLevel.IsSelected == !isSelected)
                {
                    atomLevel.IsSelected = isSelected;
                }
            }

            WriteDataDelegateCommand?.RaiseCanExecuteChanged();
        }

        private void ShowSnackbarMessage(string text)
        {
            SnackbarText = text;

            Task.Run(() =>
            {
                IsSnackbarActive = true;

                Thread.Sleep(3000);

                IsSnackbarActive = false;
            });
        }
    }
}