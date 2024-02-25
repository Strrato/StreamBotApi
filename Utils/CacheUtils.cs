using Newtonsoft.Json;
using StreamBot.Api.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StreamBot.Api.Utils
{
    public class CacheUtils
    {
        public static readonly string AppCacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StreamBotApi");

        public static List<T> GetCache<T>(string fileName)
        {
            if (!Directory.Exists(AppCacheDir))
            {
                Directory.CreateDirectory(AppCacheDir);
            }

            string fileNamePath = Path.Combine(AppCacheDir, fileName);

            List<T>? cache = new();

            if (File.Exists(fileNamePath))
            {
                string content = File.ReadAllText(fileNamePath);
                cache = JsonConvert.DeserializeObject<List<T>>(content);
                if (cache == null)
                {
                    cache = new List<T>();
                }
            }

            return cache;
        }

        public static void AddToCache<T>(string fileName, T content)
        {
            if (!Directory.Exists(AppCacheDir))
            {
                Directory.CreateDirectory(AppCacheDir);
            }

            string fileNamePath = Path.Combine(AppCacheDir, fileName);

            List<T> cache = GetCache<T>(fileName);
            cache.Add(content);

            string json = JsonConvert.SerializeObject(cache, Formatting.Indented);

            File.WriteAllText(fileNamePath, json);
        }
    }
}
