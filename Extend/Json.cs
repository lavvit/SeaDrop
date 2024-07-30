using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace SeaDrop
{
    /// <summary>
    /// 設定ファイル入出力クラス。(Json形式)
    /// </summary>
    public class Json
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter() }
        };

        /// <summary>
        /// 書き込みを行います。
        /// </summary>
        /// <param name="obj">シリアライズするインスタンス。</param>
        /// <param name="filePath">ファイル名。</param>
        public static void Save(object obj, string filePath)
        {
            using (var stream = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                stream.Write(JsonConvert.SerializeObject(obj, Formatting.Indented, Settings));
            }
        }

        /// <summary>
        /// 読み込みを行います。ファイルが存在しなかった場合、そのクラスの新規インスタンスを返します。
        /// </summary>
        /// <typeparam name="T">シリアライズしたクラス。</typeparam>
        /// <param name="filePath">ファイル名。</param>
        /// <returns>デシリアライズ結果。</returns>
        public static T Get<T>(string filePath) where T : new()
        {
            var json = "";
            if (!File.Exists(filePath))
            {
                // ファイルが存在しないので
                return new T();
            }
            using (var stream = new StreamReader(filePath, Encoding.UTF8))
            {
                json = stream.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public static object? GetValue(string path, string key)
        {
            string json = string.Join("\r\n", Text.Read(path));
            return GetValueFromText(json, key);
        }
        public static object? GetValue(string path, string key, params object?[] values)
        {
            string json = string.Join("\r\n", Text.Read(path));
            json = "{" + string.Format(json.Substring(1, json.Length - 2), values) + "}";
            var j = JObject.Parse(json);
            return GetValueFromText(json, key);
        }
        public static object? GetValueFromText(string json, string key, params object?[] values)
        {
            json = "{" + string.Format(json.Substring(1, json.Length - 2), values) + "}";
            return GetValueFromText(json, key);
        }

        public static object? GetValueFromText(string json, string key)
        {
            try
            {
                var j = JObject.Parse(json);
                var arr = j[key]?.ToArray();
                if (arr?.Length > 1)
                {
                    string[] strings = new string[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        strings[i] = arr[i].ToString();
                    }
                    return strings;
                }
                return j[key];
            }
            catch { return null; }
        }

        public static string GetJsonText(string path)
        {
            string json = "";
            if (!File.Exists(path))
            {
                // ファイルが存在しないので
                return "";
            }
            using (var stream = new StreamReader(path, Encoding.UTF8))
            {
                json = stream.ReadToEnd();
            }
            return json;
        }
    }
}