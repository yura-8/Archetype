using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// 確認ウィンドウのUI部品を制御するクラスです。
    /// </summary>
    public class ConfirmationUIController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _questionText; // 「〇〇しますか？」のテキスト
        [SerializeField]
        private GameObject _cursorYes; // 「はい」のカーソル
        [SerializeField]
        private GameObject _cursorNo;  // 「いいえ」のカーソル

        /// <summary>
        /// 全てのカーソルを非表示にします。
        /// </summary>
        private void HideAllCursors()
        {
            if (_cursorYes != null)
            {
                _cursorYes.SetActive(false);
            }
            if (_cursorNo != null)
            {
                _cursorNo.SetActive(false);
            }
        }

        /// <summary>
        /// 選択中のカーソルを表示します。
        /// </summary>
        /// <param name="isYesSelected">「はい」が選択されているか</param>
        public void ShowSelectedCursor(bool isYesSelected)
        {
            HideAllCursors();
            if (isYesSelected && _cursorYes != null)
            {
                _cursorYes.SetActive(true);
            }
            else if (!isYesSelected && _cursorNo != null)
            {
                _cursorNo.SetActive(true);
            }
        }

        /// <summary>
        /// 質問文のテキストを設定します。
        /// </summary>
        public void SetQuestionText(string question)
        {
            if (_questionText != null)
            {
                _questionText.text = question;
            }
            else
            {
                Debug.LogError("ConfirmationUIController: _questionText is not assigned in the Inspector.");
            }
        }

        /// <summary>
        /// UIを表示します。
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// UIを非表示にします。
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}