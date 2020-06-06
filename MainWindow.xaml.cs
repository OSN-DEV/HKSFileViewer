using HKSFileViewer.AppUtil;
using HKSFileViewer.Data;
using OsnCsLib.File;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static HKSFileViewer.AppUtil.ErrorMessage;

namespace HKSFileViewer {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {

        #region Declaration
        private Dictionary<int, string> _keyMapping { set; get; } = new Dictionary<int, string>();
        private SolidColorBrush _onBg;
        private SolidColorBrush _offBg;
        #endregion

        #region Constructor
        public MainWindow() {
            InitializeComponent();
            this.Initialize();
        }
        #endregion

        #region Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_DragOver(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true)) {
                e.Effects = System.Windows.DragDropEffects.Copy;
            } else {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Drop(object sender, DragEventArgs e) {
            var files = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
            if (null == files || !System.IO.File.Exists(files[0])) {
                return;
            }

            var ret = this.ShowKeyMapping(files[0]);
            if (!ret.Result) {
                ErrorMessage.Show(ret.Id);
            } else {
                var settings = AppRepository.GetInstance();
                settings.HskFile = files[0];
                settings.Save();
            }
        }

        #endregion

        #region Private Method
        /// <summary>
        /// initialize app
        /// </summary>
        private void Initialize() {
            // initialize
            var settings = AppRepository.Init(Constants.SettingFile);
            this.cNormalKeyMapping.DataContext = new KeyMappingViewModel();
            this.cFnKeyMapping.DataContext = new KeyMappingViewModel();

            // read mapping information
            using (var reader = new FileOperator(Constants.KeyMappingFIle, FileOperator.OpenMode.Read)) {
                while (!reader.Eof) {
                    var pair = reader.ReadLine().Split('\t');
                    if (this._keyMapping.ContainsKey(int.Parse(pair[0]))) {
                        ErrorMessage.Show(ErrMsgId.MappingKeyIsDuplicated);
                        break;
                    } else {
                        this._keyMapping.Add(int.Parse(pair[0]), pair[1].Replace("@r@", "\r\n"));
                    }
                }
            }

            // set dip siwtch text color
            this._onBg = this.GetColorFromRgb("#7373FF");
            this._offBg = this.GetColorFromRgb("#DDDDDD");

            // restore if possible
            this.ShowKeyMapping(settings.HskFile);
        }

        /// <summary>
        /// show mapping data
        /// </summary>
        /// <param name="file">hks file</param>
        private (bool Result, ErrMsgId Id) ShowKeyMapping(string file) {
            try {
                using (var hks = new FileOperator(file)) {
                    // check hks file
                    if (!hks.Exists()) {
                        return (false, ErrMsgId.HKSFileNotFound);
                    }
                    if ("hks" != hks.Extension) {
                        return (false, ErrMsgId.InvalidFileType);
                    }

                    // desialize json
                    var json = HksJsonData.Desialize(hks.ReadAll());

                    // check hsk files
                    if (6 != json.DipSwitch?.Length) {
                        return (false, ErrMsgId.DipSwitchDefIsWrong);
                    }
                    if (128 != json.Keymap?.Normal?.Length) {
                        return (false, ErrMsgId.NormalKeyMappingDefIsWrong);
                    }
                    if (128 != json.Keymap?.WithFn?.Length) {
                        return (false, ErrMsgId.WithFnKeyMappingDefIsWrong);
                    }

                    // show data
                    this.cToolVersion.Text = json.ToolVersion;

                    var dipSwitches = new TextBlock[] { cDipSwitch1, cDipSwitch2, cDipSwitch3, cDipSwitch4, cDipSwitch5, cDipSwitch6 };
                    for (int i = 0; i < json.DipSwitch.Length; i++) {
                        this.ShowDipSwitch(dipSwitches[i], json.DipSwitch[i]);
                    }

                    var normal = this.cNormalKeyMapping.DataContext as KeyMappingViewModel;
                    normal.KeyMapping.Clear();
                    this.SetKeyMappingData(normal, json.Keymap.Normal);
                    normal.Update();

                    var fn = this.cFnKeyMapping.DataContext as KeyMappingViewModel;
                    fn.KeyMapping.Clear();
                    this.SetKeyMappingData(fn, json.Keymap.WithFn);
                    fn.Update();
                }
            } catch  {
                return (false, ErrMsgId.HKSFileParseException);
            }
            return (true, 0);
        }

        /// <summary>
        /// show dipswitch
        /// </summary>
        /// <param name="dipSwitch">dip switch</param>
        /// <param name="value">value</param>
        private void ShowDipSwitch(TextBlock dipSwitch, bool value) {
            dipSwitch.Text = value ? "ON" : "OFF";
            dipSwitch.Background = value ? this._onBg : this._offBg;
        }

        /// <summary>
        /// get solid brush from rgb string
        /// </summary>
        /// <param name="color">color string</param>
        /// <returns>brush</returns>
        private SolidColorBrush GetColorFromRgb(string color) {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        /// <summary>
        /// set  key mapping data
        /// </summary>
        /// <param name="model">viewmodel</param>
        /// <param name="mapping">mapping list</param>
        private void SetKeyMappingData(KeyMappingViewModel model, int[] mapping) {
            for (int i = 0; i < mapping.Length; i++) {
                if (this._keyMapping.ContainsKey(mapping[i])) {
                    model.KeyMapping.Add(i + 1, this._keyMapping[mapping[i]]);
                } else {
                    model.KeyMapping.Add(i + 1, "N/A");
                }
            }
        }
        #endregion

    }
}
