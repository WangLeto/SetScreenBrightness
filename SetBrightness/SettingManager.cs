using System;
using System.Collections.Generic;
using System.Text;
using SetBrightness.Properties;

namespace SetBrightness
{
    internal static class SettingManager
    {
        public static bool UseContrast
        {
            get { return Settings.Default.use_contrast; }
            set
            {
                Settings.Default.use_contrast = value;
                Settings.Default.Save();
            }
        }

        public static bool UseHotKey
        {
            get { return Settings.Default.use_hotkey; }
            set
            {
                Settings.Default.use_hotkey = value;
                Settings.Default.Save();
            }
        }

        public static string PreferMonitor
        {
            get { return Settings.Default.prefer_monitor; }
            set
            {
                Settings.Default.prefer_monitor = value;
                Settings.Default.Save();
            }
        }

        public static Dictionary<string, string> GetReNameMonitors()
        {
            var str = Settings.Default.rename_monitor;
            return StrToDictionary(str);
        }

        private const char Delimiter = (char) 1;

        public static void AddRenameMonitor(string id, string name)
        {
            // id, name, id, name；id 和 name 使用 ,, 替换 ,
            var str = id + Delimiter + name;
            // 注意：将新增的内容拼接在后面，以覆盖已经出现过的值
            if (!string.IsNullOrWhiteSpace(Settings.Default.rename_monitor))
            {
                str = Settings.Default.rename_monitor + Delimiter + str;
            }

            var dictionary = StrToDictionary(str);
            Settings.Default.rename_monitor = DictionnaryToStr(dictionary);
            Settings.Default.Save();
        }

        public static void RemoveMonitorName(string id)
        {
            var str = Settings.Default.rename_monitor;
            var dictionary = StrToDictionary(str);
            if (!dictionary.ContainsKey(id))
            {
                return;
            }

            dictionary.Remove(id);
            Settings.Default.rename_monitor = DictionnaryToStr(dictionary);
            Settings.Default.Save();
        }

        private static string DictionnaryToStr(Dictionary<string, string> dictionary)
        {
            var str = new StringBuilder();
            var first = true;
            foreach (var pair in dictionary)
            {
                var id = pair.Key;
                var name = pair.Value;
                if (!first)
                {
                    str.Append(Delimiter);
                }
                else
                {
                    first = false;
                }

                str.Append(id + Delimiter + name);
            }

            return str.ToString();
        }

        /// <summary>
        /// 去除重复：以较后的为准
        /// </summary>
        /// <param name="str">原始字符串+新增字符串</param>
        /// <returns>更新过的字典</returns>
        private static Dictionary<string, string> StrToDictionary(string str)
        {
            var dictionary = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(str))
            {
                return dictionary;
            }
            var strList = str.Split(Delimiter);
            for (var i = 0; i < strList.Length; i += 2)
            {
                if (dictionary.ContainsKey(strList[i]))
                {
                    dictionary[strList[i]] = strList[i + 1];
                }
                else
                {
                    dictionary.Add(strList[i], strList[i + 1]);
                }
            }

            return dictionary;
        }
    }
}