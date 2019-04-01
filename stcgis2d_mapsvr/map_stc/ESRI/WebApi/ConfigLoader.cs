using System.Linq;
using System;

namespace ESRI.WebApi
{
    public class ConfigLoader
    {
        private static string app_root_path = AppDomain.CurrentDomain.BaseDirectory;
        private static string item_data_file_suffix = "_DATA";

        public static string loadItem(string id)
        {
            string path = app_root_path + "configs/portal/items/" + id + ".json";

            if (System.IO.File.Exists(path))
                return System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
            else
                return "";
        }
        public static string loadJsonFile(string name)
        {
            string path = app_root_path + "configs/" + name + ".json";

            if (System.IO.File.Exists(path))
                return System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
            else
                return "";
        }

        public static string loadItemData(string id)
        {
            string path = app_root_path + "configs/portal/items/" + id + item_data_file_suffix + ".json";

            if (System.IO.File.Exists(path))
            {
                var fileContent = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);

                string popupsDir = app_root_path + "configs/portal/popups/";
                var files = System.IO.Directory.GetFiles(popupsDir).Where(t => t.ToLower().EndsWith(".html")).ToList();

                foreach (var f in files)
                {
                    var fName = System.IO.Path.GetFileNameWithoutExtension(f);
                    fileContent = fileContent.Replace("[" + fName + "]", getPopupsContent(fName));
                }
                return replaceRootUrl(fileContent);
            }
            else
                return "";
        }

        public static string getPopupsContent(string name)
        {
            string path = app_root_path + "configs/portal/popups/" + name + ".html";
            string fileContent = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);

            return System.Text.RegularExpressions.Regex.Replace(fileContent, @">\s+<", "><");
        }

        private static string _ags_root = "";
        private static string _pbs_root = "";
        private static string _das_root = "";

        public static string replaceRootUrl(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return str;

            if (String.IsNullOrWhiteSpace(_ags_root))
                _ags_root = System.Configuration.ConfigurationManager.AppSettings["ags_root"];
            if (String.IsNullOrWhiteSpace(_pbs_root))
                _pbs_root = System.Configuration.ConfigurationManager.AppSettings["pbs_root"];
            if (String.IsNullOrWhiteSpace(_das_root))
                _das_root = System.Configuration.ConfigurationManager.AppSettings["das_root"];

            string ret = str.Replace("[ags_root]", _ags_root);
            ret = ret.Replace("[pbs_root]", _pbs_root);
            ret = ret.Replace("[das_root]", _das_root);

            return ret;
        }
    }
}