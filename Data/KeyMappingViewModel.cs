using System.Collections.Generic;

namespace HKSFileViewer.Data {
    public class KeyMappingViewModel : BindableBase {

        #region Public Property
        private Dictionary<int, string> _keyMapping = new Dictionary<int, string>();
        public Dictionary<int, string> KeyMapping {
            get { return this._keyMapping; }
            set { this.SetProperty(ref this._keyMapping, value); }
        }
        #endregion

        #region Public Method
        public void Update() {
            this.SetProperty(nameof(KeyMapping));
        }
        #endregion

    }
}
