using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

        [Header("カーソル色設定")]
        private Image _cursorImageComponent;

        [Header("属性別のカーソル色")]
        [SerializeField] private Color _noneAttributeColor = Color.white;
        [SerializeField] private Color _plasmaColor = new Color(0.5f, 1f, 1f, 0.5f);
        [SerializeField] private Color _cryoColor = new Color(0.5f, 0.8f, 1f, 0.5f);
        [SerializeField] private Color _pulseColor = new Color(1f, 0.5f, 1f, 0.5f);

        private void Awake()
        {
            // _cursorObjにアタッチされているImageコンポーネントを取得して、変数に保存しておく
            if (_cursorObj != null)
            {
                _cursorImageComponent = _cursorObj.GetComponent<Image>();
            }
        }

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

        public void SetCursorColor(ElementAttribute attribute)
        {
            if (_cursorImageComponent == null) return;

            switch (attribute)
            {
                case ElementAttribute.Plasma: _cursorImageComponent.color = _plasmaColor; break;
                case ElementAttribute.Cryo: _cursorImageComponent.color = _cryoColor; break;
                case ElementAttribute.Pulse: _cursorImageComponent.color = _pulseColor; break;
                default: _cursorImageComponent.color = _noneAttributeColor; break;
            }
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