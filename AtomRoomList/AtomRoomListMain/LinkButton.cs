using Autodesk.Revit.UI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace AtomRoomListMain
{
    public class LinkButton : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string curentAssemblyName = assembly.GetName().Name;
            string proxyAssemblyName = assembly.Location.Replace("Main", "Proxy");
            string curentFullClassName = GetType().FullName;
            string proxyFullClassName = curentFullClassName.Replace("Main", "Proxy").Replace("LinkButton", "AtomRoomListStartCommand");

            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            string tabName = "Атом";
            string panelName = "Архитектура";
            string pluginName = "Список\nпомещений";
            string toolTip = "Заполнение списка помещений для каждого пола.";
            string longDescription = "Автоматическое заполнение списка помещений для каждого пола.";
            string largeImageName = "AtomRoomList_32x32.png";
            string imageName = "AtomRoomList_16x16.png";
            string newLargeImageName = null;
            string newImageName = null;

            // Проверяем наличие настроек плагина
            var atomPluginSettingsFile = Path.Combine(Path.GetDirectoryName(assembly.Location), "AtomPluginSettings.json");
            if (File.Exists(atomPluginSettingsFile))
            {
                var data = File.ReadAllText(atomPluginSettingsFile);
                var atomPluginSettings = JsonConvert.DeserializeObject<AtomPluginSettings>(data);

                tabName = atomPluginSettings.TabName;
                panelName = atomPluginSettings.PanelName;
                pluginName = atomPluginSettings.PluginName;
                toolTip = atomPluginSettings.ToolTip;
                longDescription = atomPluginSettings.LongDescription;
                newLargeImageName = atomPluginSettings.LargeImageName;
                newImageName = atomPluginSettings.ImageName;
            }

            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { }

            var ribbonPanels = application.GetRibbonPanels(tabName);
            var ribbonPanel = ribbonPanels.FirstOrDefault(panel => panel.Name == panelName) ?? application.CreateRibbonPanel(tabName, panelName);

            PushButtonData pushButtonData = new PushButtonData("AtomRoomListStartCommand", pluginName, proxyAssemblyName, proxyFullClassName);

            PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;
            pushButton.ToolTip = toolTip;
            pushButton.LongDescription = longDescription;
            pushButton.LargeImage = new BitmapImage(new Uri($"pack://application:,,,/AtomRoomListMain;component/Resources/{largeImageName}"));
            pushButton.Image = new BitmapImage(new Uri($"pack://application:,,,/AtomRoomListMain;component/Resources/{imageName}"));

            try
            {
                var largeImagePath = Path.Combine(Path.GetDirectoryName(assembly.Location), "Icons", newLargeImageName);
                var imagePath = Path.Combine(Path.GetDirectoryName(assembly.Location), "Icons", newImageName);

                if (File.Exists(largeImagePath))
                {
                    var largeImageBytes = File.ReadAllBytes(largeImagePath);
                    pushButton.LargeImage = ToImage(largeImageBytes);
                }

                if (File.Exists(imagePath))
                {
                    var imageBytes = File.ReadAllBytes(imagePath);
                    pushButton.Image = ToImage(imageBytes);
                }
            }
            catch { }

            return Result.Succeeded;
        }

        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                try
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}