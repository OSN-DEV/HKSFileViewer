using OsnCsLib.Data;

namespace HKSFileViewer.Data {
    public class AppRepository : AppDataBase<AppRepository> {

        #region Declaration   
        private static string _settingFile;
        #endregion

        #region Public Property
        public string HskFile { set; get; } = "";
        #endregion

        #region Public Method
        public static AppRepository Init(string file) {
            _settingFile = file;
            GetInstanceBase(file);
            if (!System.IO.File.Exists(file)) {
                _instance.Save();
            }
            return _instance;
        }

        /// <summary>
        /// get instance
        /// </summary>
        /// <returns></returns>
        public static AppRepository GetInstance() {
            return GetInstanceBase();
        }

        /// <summary>
        /// save settings
        /// </summary>
        public void Save() {
            GetInstanceBase().SaveToXml(_settingFile);
        }
        #endregion
    }
}
