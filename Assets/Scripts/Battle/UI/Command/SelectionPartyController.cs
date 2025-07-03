using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するUIを制御するクラスです。
    /// </summary>
    public class SelectionPartyController : MonoBehaviour
    {
        /// <summary>
        /// コマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObj;

        /// <summary>
        /// キャラクターの名前を表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _characterNameText;

        /// <summary>
        /// キャラクターの名前をセットします。
        /// </summary>
        /// <param name="characterName">敵キャラクターの名前</param>
        public void SetCharacterName(string characterName)
        {
            _characterNameText.text = characterName;
        }

        /// <summary>
        /// キャラクターの名前を空欄にします。
        /// </summary>
        public void ClearCharacterName()
        {
            _characterNameText.text = "";
        }

        /// <summary>
        /// 選択項目のカーソルを表示します。
        /// </summary>
        public void ShowCursor()
        {
            _cursorObj.SetActive(true);
        }

        /// <summary>
        /// 選択項目のカーソルを非表示にします。
        /// </summary>
        public void HideCursor()
        {
            _cursorObj.SetActive(false);
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