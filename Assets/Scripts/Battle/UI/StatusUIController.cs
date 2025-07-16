// StatusUIController.cs

using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SimpleRpg
{
    public class StatusUIController : MonoBehaviour, IBattleUIController
    {
        // --- 既存の変数は変更なし ---
        [SerializeField] TextMeshProUGUI _characterNameText;
        [SerializeField] TextMeshProUGUI _currentHpText;
        [SerializeField] TextMeshProUGUI _maxHpText;
        [SerializeField] TextMeshProUGUI _currentBtText;
        [SerializeField] TextMeshProUGUI _maxBtText;
        [SerializeField] private Image _highlightImage;

        [Header("特殊状態の色設定")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _overheatColor = new Color(255, 191, 127);
        [SerializeField] private Color _overchargeColor = new Color(0.8f, 0.4f, 1f); // 紫色
        [SerializeField] private Color _stunColor = new Color(0.5f, 0.8f, 1f); // 水色

        [Header("属性別のハイライト色設定")]
        [SerializeField] private Color _noneAttributeColor = Color.white;// デフォルト色 (半透明の白)
        [SerializeField] private Color _plasmaColor = new Color(0.5f, 1f, 1f); // 黄緑
        [SerializeField] private Color _cryoColor = new Color(0.5f, 0.8f, 1f);   // 水色
        [SerializeField] private Color _pulseColor = new Color(1f, 0.5f, 1f);    // 赤

        /// <summary>
        /// 特殊状態に応じてキャラクター名の色を変更します。
        /// </summary>
        public void SetStatusColor(SpecialStatusType status)
        {
            switch (status)
            {
                case SpecialStatusType.Overheat:
                    _currentBtText.color = _overheatColor;
                    break;
                case SpecialStatusType.Overcharge:
                    _currentBtText.color = _overchargeColor;
                    break;
                case SpecialStatusType.Stun:
                    _currentBtText.color = _stunColor;
                    break;
                case SpecialStatusType.None:
                default:
                    _currentBtText.color = _normalColor;
                    break;
            }
        }

        /// <summary>
        /// 属性に応じてハイライトの色を変更します。
        /// </summary>
        public void SetAttributeHighlightColor(ElementAttribute attribute)
        {
            if (_highlightImage == null) return;

            switch (attribute)
            {
                case ElementAttribute.Plasma:
                    _highlightImage.color = _plasmaColor;
                    break;
                case ElementAttribute.Cryo:
                    _highlightImage.color = _cryoColor;
                    break;
                case ElementAttribute.Pulse:
                    _highlightImage.color = _pulseColor;
                    break;
                case ElementAttribute.None:
                default:
                    _highlightImage.color = _noneAttributeColor;
                    break;
            }
        }

        /// <summary>
        /// ハイライトカーソルの表示・非表示を切り替えます。
        /// </summary>
        /// <param name="isActive">表示する場合はtrue</param>
        public void SetHighlightActive(bool isActive)
        {
            if (_highlightImage != null)
            {
                _highlightImage.gameObject.SetActive(isActive);
            }
            else
            {
                // もしハイライト画像が設定されていなかったら、警告を出す
                Debug.LogWarning("StatusUIControllerにハイライト用のImageが設定されていません。");
            }
        }
        // ★★★★★ ここまで追加 ★★★★★

        // --- SetCharacterName などの他のメソッドは変更ありません ---
        public void SetCharacterName(string characterName)
        {
            _characterNameText.text = characterName;
        }

        public void SetCurrentHp(int currentHp)
        {
            _currentHpText.text = currentHp.ToString();
        }

        public void SetMaxHp(int maxHp)
        {
            _maxHpText.text = maxHp.ToString();
        }

        public void SetCurrentBt(int currentBt)
        {
            _currentBtText.text = currentBt.ToString();
        }

        public void SetMaxBt(int maxBt)
        {
            _maxBtText.text = maxBt.ToString();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}