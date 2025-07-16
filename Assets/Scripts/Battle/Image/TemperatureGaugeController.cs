using UnityEngine;
using UnityEngine.UI;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の温度ゲージを制御するクラスです。
    /// </summary>
    public class TemperatureGaugeController : MonoBehaviour, IBattleWindowController
    {
        [Header("UIコンポーネント")]
        [SerializeField]
        private Image _gaugeImage; // 温度画像を表示するImage

        [Header("温度別スプライト")]
        [SerializeField]
        private Sprite _hotSprite;
        [SerializeField]
        private Sprite _normalSprite;
        [SerializeField]
        private Sprite _coldSprite;

        private BattleManager _battleManager;

        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
            HideWindow();
        }

        /// <summary>
        /// 温度に応じてUIの表示を更新します。
        /// </summary>
        public void UpdateTemperatureUI(TemperatureState state)
        {
            switch (state)
            {
                case TemperatureState.HOT:
                    _gaugeImage.sprite = _hotSprite;
                    break;
                case TemperatureState.NORMAL:
                    _gaugeImage.sprite = _normalSprite;
                    break;
                case TemperatureState.COLD:
                    _gaugeImage.sprite = _coldSprite;
                    break;
            }
        }

        public void ShowWindow()
        {
            gameObject.SetActive(true);
        }

        public void HideWindow()
        {
            gameObject.SetActive(false);
        }
    }
}