using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 敵1体分のステータスUI（HP/BTのイラスト）を制御します。
    /// </summary>
    public class EnemyStatusUIController : MonoBehaviour
    {
        [Header("UIパーツ")]
        [SerializeField] private Image _hpImage; // HPイラストを表示するImageコンポーネント
        [SerializeField] private Image _btImage; // BTイラストを表示するImageコンポーネント

        [Header("HPスプライト（4段階）")]
        [Tooltip("HP残量に応じて表示するスプライトを4つ設定（100%→75%→50%→25%の順で）")]
        [SerializeField] private List<Sprite> _hpSprites;

        [Header("BTスプライト（7段階）")]
        [Tooltip("BT残量に応じて表示するスプライトを7つ設定（過充電→100%→80%→...→0%の順で）")]
        [SerializeField] private List<Sprite> _btSprites;

        /// <summary>
        /// 敵のステータスに応じてHPとBTの表示を更新します。
        /// </summary>
        public void UpdateDisplay(EnemyStatus status)
        {
            Debug.Log($"UI更新: {status.enemyData.enemyName} | 現在BT: {status.currentBt}, 最大BT: {status.enemyData.bt}");

            // HPの割合を計算 (0.0～1.0)
            float hpRate = (float)status.currentHp / status.enemyData.hp;
            UpdateHpSprite(hpRate);

            // BTの割合を計算 (0.0～1.0)
            float btRate = (float)status.currentBt / status.enemyData.bt;
            bool isOvercharged = status.currentBt > status.enemyData.bt;
            UpdateBtSprite(btRate, isOvercharged);
        }

        private void UpdateHpSprite(float rate)
        {
            if (_hpSprites == null || _hpSprites.Count != 4) return;

            if (rate > 0.75f) _hpImage.sprite = _hpSprites[0]; // 100-76%
            else if (rate > 0.50f) _hpImage.sprite = _hpSprites[1]; // 75-51%
            else if (rate > 0.25f) _hpImage.sprite = _hpSprites[2]; // 50-26%
            else _hpImage.sprite = _hpSprites[3]; // 25-0%
        }

        private void UpdateBtSprite(float rate, bool isOvercharged)
        {
            if (_btSprites == null || _btSprites.Count != 7) return;

            if (isOvercharged) _btImage.sprite = _btSprites[0]; // 過充電
            else if (rate >= 1.0f) _btImage.sprite = _btSprites[1]; // 100%
            else if (rate >= 0.8f) _btImage.sprite = _btSprites[2]; // 80%
            else if (rate >= 0.6f) _btImage.sprite = _btSprites[3]; // 60%
            else if (rate >= 0.4f) _btImage.sprite = _btSprites[4]; // 40%
            else if (rate > 0f) _btImage.sprite = _btSprites[5]; // 20%
            else _btImage.sprite = _btSprites[6]; // 0%
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}