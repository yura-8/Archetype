using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    public class CharacterDetailStatusUIController : MonoBehaviour, IBattleUIController
    {
        [Header("UI要素への参照")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _currentHpText;
        [SerializeField] private TextMeshProUGUI _maxHpText;
        [SerializeField] private TextMeshProUGUI _currentBtText;
        [SerializeField] private TextMeshProUGUI _maxBtText;

        // ★★★ 変更点: ATK, DEF, DEX のテキストをそれぞれ1つに戻します ★★★
        [SerializeField] private TextMeshProUGUI _atkText;
        [SerializeField] private TextMeshProUGUI _defText;
        [SerializeField] private TextMeshProUGUI _dexText;

        [SerializeField] private TextMeshProUGUI _attributeText;
        [SerializeField] private TextMeshProUGUI _specialStatusText;

        [Header("色設定")]
        [SerializeField] private Color _normalBtColor = Color.white;
        [SerializeField] private Color _plasmaColor = new Color(0.5f, 1f, 1f);
        [SerializeField] private Color _cryoColor = new Color(0.5f, 0.8f, 1f);
        [SerializeField] private Color _pulseColor = new Color(1f, 0.5f, 1f);
        [SerializeField] private Color _overheatColor = new Color(1f, 0.75f, 0.5f);
        [SerializeField] private Color _overchargeColor = new Color(0.8f, 0.4f, 1f);
        [SerializeField] private Color _stunColor = new Color(0.5f, 0.8f, 1f);


        // --- Name, Level, HP, BTのセッターは変更なし ---
        public void SetName(string name) => _nameText.text = name;
        public void SetLevel(int level) => _levelText.text = $"Lv {level}";
        public void SetCurrentHp(int current) => _currentHpText.text = current.ToString();
        public void SetMaxHp(int max) => _maxHpText.text = max.ToString();
        public void SetCurrentBt(int current) => _currentBtText.text = current.ToString();
        public void SetMaxBt(int max) => _maxBtText.text = max.ToString();

        private void SetStatValue(int baseValue, int currentValue, TextMeshProUGUI targetText)
        {
            // 差分を計算
            int diff = currentValue - baseValue;

            if (diff > 0) // バフがかかっている場合
            {
                targetText.text = $"{currentValue}+";
            }
            else if (diff < 0) // デバフがかかっている場合
            {
                targetText.text = $"{currentValue}-";
            }
            else // 増減がない場合
            {
                targetText.text = currentValue.ToString();
            }
        }

        public void SetAtk(int baseValue, int currentValue) => SetStatValue(baseValue, currentValue, _atkText);
        public void SetDef(int baseValue, int currentValue) => SetStatValue(baseValue, currentValue, _defText);
        public void SetDex(int baseValue, int currentValue) => SetStatValue(baseValue, currentValue, _dexText);

        // --- Attribute, SpecialStatus, BTカラーのメソッドは変更なし ---
        public void SetAttribute(ElementAttribute attribute)
        {
            _attributeText.text = attribute.ToString();
            switch (attribute)
            {
                case ElementAttribute.Plasma: _attributeText.color = _plasmaColor; break;
                case ElementAttribute.Cryo: _attributeText.color = _cryoColor; break;
                case ElementAttribute.Pulse: _attributeText.color = _pulseColor; break;
                default: _attributeText.color = Color.white; break;
            }
        }

        public void SetSpecialStatus(SpecialStatusType status)
        {
            if (status == SpecialStatusType.None)
            {
                _specialStatusText.gameObject.SetActive(false);
                return;
            }
            _specialStatusText.gameObject.SetActive(true);
            _specialStatusText.text = status.ToString();
            switch (status)
            {
                case SpecialStatusType.Overheat: _specialStatusText.color = _overheatColor; break;
                case SpecialStatusType.Overcharge: _specialStatusText.color = _overchargeColor; break;
                case SpecialStatusType.Stun: _specialStatusText.color = _stunColor; break;
            }
        }

        public void SetCurrentBtColorByStatus(SpecialStatusType status)
        {
            switch (status)
            {
                case SpecialStatusType.Overheat: _currentBtText.color = _overheatColor; break;
                case SpecialStatusType.Overcharge: _currentBtText.color = _overchargeColor; break;
                case SpecialStatusType.Stun: _currentBtText.color = _stunColor; break;
                default: _currentBtText.color = _normalBtColor; break;
            }
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}