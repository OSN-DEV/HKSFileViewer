using System.Collections.Generic;
using System.Windows;

namespace HKSFileViewer.AppUtil {
    internal class ErrorMessage {
        #region Declaration
        public enum ErrMsgId {
            HKSFileNotFound,
            DipSwitchDefIsWrong,
            NormalKeyMappingDefIsWrong,
            WithFnKeyMappingDefIsWrong,
            HKSFileParseException,
            InvalidFileType,
            MappingKeyIsDuplicated,
        }
        private static Dictionary<ErrMsgId, string> _message = new Dictionary<ErrMsgId, string>() {
            { ErrMsgId.HKSFileNotFound, "HSKファイルが見つかりません。"},
            { ErrMsgId.DipSwitchDefIsWrong,"Dipスイッチの定義が不正です。" },
            { ErrMsgId.NormalKeyMappingDefIsWrong,"キーマピングの定義が不正です。" },
            { ErrMsgId.WithFnKeyMappingDefIsWrong,"キーマピング(Fn)の定義が不正です。" },
            { ErrMsgId.HKSFileParseException,"HKSファイルの読込時に予期せぬエラーが発生しました。" },
            { ErrMsgId.InvalidFileType,"HKS形式のファイルを指定してください。" },
            { ErrMsgId.MappingKeyIsDuplicated,"mapping.tsvにキーの重複あります。" },

        };
        #endregion

        #region Public Method
        /// <summary>
        /// show error message
        /// </summary>
        /// <param name="id"></param>
        public static void Show(ErrMsgId id) {
            MessageBox.Show(_message[id], "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        #endregion

    }
}
