using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// ステータス表示のUIを制御するクラスです。
    /// </summary>
    public class StatusUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// キャラクターの名前を表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _characterNameText;

        /// <summary>
        /// 現在のHPを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _currentHpText;

        /// <summary>
        /// 最大HPを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _maxHpText;

        /// <summary>
        /// 現在のBTを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _currentBtText;

        /// <summary>
        /// 最大BTを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _maxBtText;

        /// <summary>
        /// キャラクターの名前をセットします。
        /// </summary>
        /// <param name="characterName">キャラクターの名前</param>
        public void SetCharacterName(string characterName)
        {
            _characterNameText.text = characterName;
        }

        /// <summary>
        /// 現在のHPをセットします。
        /// </summary>
        /// <param name="currentHp">現在のHP</param>
        public void SetCurrentHp(int currentHp)
        {
            _currentHpText.text = currentHp.ToString();
        }

        /// <summary>
        /// 最大HPをセットします。
        /// </summary>
        /// <param name="maxHp">最大HP</param>
        public void SetMaxHp(int maxHp)
        {
            _maxHpText.text = maxHp.ToString();
        }

        /// <summary>
        /// 現在のBTをセットします。
        /// </summary>
        /// <param name="currentBt">現在のBT</param>
        public void SetCurrentBt(int currentBt)
        {
            _currentBtText.text = currentBt.ToString();
        }

        /// <summary>
        /// 最大MPをセットします。
        /// </summary>
        /// <param name="maxBt">最大BT</param>
        public void SetMaxBt(int maxBt)
        {
            _maxBtText.text = maxBt.ToString();
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