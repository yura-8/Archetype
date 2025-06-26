using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するUIを制御するクラスです。
    /// </summary>
    public class EnemyNameUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// 敵キャラクターの名前を表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _enemyNameText;

        /// <summary>
        /// 敵キャラクターの名前をセットします。
        /// </summary>
        /// <param name="enemyName">敵キャラクターの名前</param>
        public void SetEnemyName(string enemyName)
        {
            _enemyNameText.text = enemyName;
        }

        /// <summary>
        /// 敵キャラクターの名前を空欄にします。
        /// </summary>
        public void ClearEnemyName()
        {
            _enemyNameText.text = "";
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