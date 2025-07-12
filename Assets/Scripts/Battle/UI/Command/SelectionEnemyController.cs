using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するUIを制御するクラスです。
    /// </summary>
    public class SelectionEnemyController : MonoBehaviour
    {
        /// <summary>
        /// コマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObj;

        [SerializeField]
        GameObject _targetObj;

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
        /// 敵キャラクターの名前の文字色を設定します。
        /// </summary>
        /// <param name="color">設定したい色</param>
        public void SetTextColor(Color color)
        {
            if (_enemyNameText != null)
            {
                _enemyNameText.color = color;
            }
        }

        /// <summary>
        /// 選択項目のカーソルを表示します。
        /// </summary>
        public void ShowCursor()
        {
            _cursorObj.SetActive(true);
            _targetObj.SetActive(true);
        }

        /// <summary>
        /// 選択項目のカーソルを非表示にします。
        /// </summary>
        public void HideCursor()
        {
            _cursorObj.SetActive(false);
            _targetObj.SetActive(false);
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