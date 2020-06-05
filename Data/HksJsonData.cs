using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
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
            return JsonSerializer.Deserialize<HksJsonData>(json);
        }
        #endregion


    }
}
