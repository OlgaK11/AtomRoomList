using Newtonsoft.Json;
using System;
using System.IO;

namespace AtomRoomList.UserSettings
{
    public static class AtomUserSettingsService
    {
        private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"AtomData\UserData\AtomFloorList\AtomFloorListSettings.json");

        public static AtomUserSettings Read()
        {
            if (!File.Exists(_path))
            {
                return new AtomUserSettings();
            }

            var data = File.ReadAllText(_path);
            var atomUserSettings = JsonConvert.DeserializeObject<AtomUserSettings>(data);
            return atomUserSettings;
        }

        public static void Save(AtomUserSettings atomUserSettings)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path));

                using (var streamWriter = File.CreateText(_path))
                {
                    var serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                    serializer.Serialize(streamWriter, atomUserSettings);
                }
            }
            catch { }
        }
    }
}