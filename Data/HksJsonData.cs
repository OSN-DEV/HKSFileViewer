using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Text.Json;
using OsnCsLib.File;

namespace HKSFileViewer.Data {
    public class HksJsonData {
        #region Declaration
        public string ToolVersion { set; get; }
        public int KeyboardLayout { set; get; }
        public string TypeNumber { set; get; }
        public bool[] DipSwitch { set; get; }
        public KeymapData Keymap { set; get; }

        public class KeymapData {
            public int[] Normal { set; get; }
            public int[] WithFn { set; get; }
        }
        #endregion

        #region Public Method
        public static HksJsonData Desialize(string json) {
            // System.Text.Jsonは大量のDLLが付随するので自前でパースする。
            // なんとなく汎用性があるように見えるけど、HKSの形式以外だと多分まともに動かないと思う。
            // return JsonSerializer.Deserialize<HksJsonData>(json);
            var jsonData = new HksJsonData();
            jsonData.Parse(json);
            return jsonData;
        }
        #endregion

        #region Private Method
        private void Parse(string json) {
            var rest = json.Replace("\n", "");

            var data = new Dictionary<string, object>();
            this.ParseData(data, ref rest);

            this.ToolVersion = data["ToolVersion"].ToString();
            this.KeyboardLayout = (int)data["KeyboardLayout"];
            this.TypeNumber = data["TypeNumber"].ToString();
            this.DipSwitch = (bool[])data["DipSwitch"];
            this.Keymap = new KeymapData();
            this.Keymap.Normal = (int[])(((Dictionary<string, object>)data["Keymap"])["Normal"]);
            this.Keymap.WithFn = (int[])(((Dictionary<string, object>)data["Keymap"])["WithFn"]);
        }


        private void ParseData(Dictionary<string, object> data, ref string rest) {
            bool searchKey = true;
            var key = "";
            while (0 < rest.Length) {
                // search key
                if (searchKey) {
                    key = this.FindKey(ref rest);
                    if (0 == key.Length) {
                        break;
                    }
                    if (data.ContainsKey(key)) {
                        throw new Exception("duplicate key:" + key);
                    }
                    data.Add(key, null);
                    searchKey = false;
                    continue;
                }

                if (rest[0] == '{') {
                    var child = new Dictionary<string, object>();
                    data[key] = child;
                    searchKey = true;
                    this.ParseData(child, ref rest);
                    continue;
                }
                if (rest[0] == '}') {
                    rest = rest.Substring(1).Trim();
                    if (rest[0] == ',') {
                        rest = rest.Substring(1).Trim();
                    }
                    searchKey = true;
                }
                    
                if (rest[0] == '[') {
                    object[] list = FindArray(ref rest);
                    if (null == list) {
                        throw new Exception("json format is maybe wrong");
                    }
                    if (key == "DipSwitch") {
                        var tmp = new bool[list.Length];
                        list.CopyTo(tmp, 0);
                        data[key] = tmp;
                    } else {
                        var tmp = new int[list.Length];
                        list.CopyTo(tmp, 0);
                        data[key] = tmp;
                    }

                    searchKey = true;
                    continue;
                }

                object val = this.FindValue(ref rest);
                if (null == val) {
                    throw new Exception("json format is maybe wrong");
                }
                data[key] = val;
                searchKey = true;
            }
        }

        private string FindKey(ref string data) {
            data = data.Trim();

            var key = "";
            var posS = data.IndexOf("\"", 0);
            var posE = data.IndexOf("\"", posS + 1);
            var nextPos = data.IndexOf(":", posS + 1);
            if (-1 == posS || -1 == posE || -1 ==  nextPos) {
                return key;
            }

            key = data.Substring(posS + 1, posE - posS - 1);
            data = data.Substring(nextPos+1).Trim();
            return key;
        }

        private object FindValue(ref string data) {
            object ret;
            int pos = data.FindeIndex(new string[] { ",", "}", "]" });
            if (-1 == pos) {
                return null;
            }
            ret = data.Substring(0, pos).Convert();
            data = data.Substring(pos + 1).Trim();
            return ret;
        }

        private object[] FindArray(ref string data) {
            int posS = data.IndexOf("[");
            int posE = data.IndexOf("]");
            if (-1 == posS || -1 == posE) {
                return null;
            }

            var list = data.Substring(posS + 1, posE - posS - 1).Split(',');
            var ret = new object[list.Length];
            for(int i=0; i < list.Length;i++) {
                ret[i] = list[i].Trim().Convert();
            }
            data = data.Substring(posE + 1).Trim();
            var pos = data.IndexOf(",");
            if (-1 < pos) {
                data = data.Substring(pos + 1).Trim();
            }
            return ret;
        }

        private string Remove(string data, string find) {
            int pos = data.FindeIndex(new string[] { ",", "}", "]" });
            if (-1 == pos) {
                return "";
            } else {
                return data.Substring(pos);
            }
        }

        #endregion
    }


    static class Extension {
        public static int FindeIndex(this string str, params string[] keys) {
            int pos = -1;
            foreach(var key in keys) {
                pos = str.IndexOf(key);
                if (-1 < pos) {
                    break;
                }
            }
            return pos;
        }

        public static string TrimSurround(this string str) {
            return str.Substring(1, str.Length - 2);
        }

        public static object Convert(this string str) {
            object ret;
            if (str == "true" || str == "false") {
                ret = Boolean.Parse(str);
            } else if (str.StartsWith("\"")) {
                ret = str.TrimSurround();
            } else {
                ret = int.Parse(str);
            }
            return ret;
        }
    }
}
