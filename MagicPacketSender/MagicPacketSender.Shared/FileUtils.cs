using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;
using System.Linq;
using Newtonsoft.Json;

namespace MagicPacketSender
{
    public static class FileUtils
    {
        private static StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private const string FILE_NAME = "Requests.json";

        public static async Task SaveRequestInfo(List<RequestInfo> info)
        {
            StorageFile file = await localFolder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
            var json = JsonConvert.SerializeObject(info);
            await FileIO.WriteTextAsync(file, json);
        }

        public static async Task<List<RequestInfo>> GetRequestInfo()
        {
            if (await localFolder.FileExistsAsync(FILE_NAME))
            {
                StorageFile file = await localFolder.GetFileAsync(FILE_NAME);
                var json = await FileIO.ReadTextAsync(file);
                return JsonConvert.DeserializeObject<List<RequestInfo>>(json);
            }
            return null;
        }

        #region StorageFolder extension methods

        public static async Task<IStorageItem> TryGetItemAsync(this StorageFolder folder, string fileName)
        {
            var files = await folder.GetItemsAsync();
            return files.FirstOrDefault(r => r.Name == fileName);

        }

        public static async Task<bool> FileExistsAsync(this StorageFolder folder, string fileName)
        {
            var item = await folder.TryGetItemAsync(fileName);
            return item != null;
        }

        #endregion
    }
}
