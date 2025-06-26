using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// コマンドのUIを制御するクラスです。
    /// </summary>
    public class MainCommandUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// 攻撃コマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjAttack;

        /// <summary>
        /// 防御コマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjGuard;

        /// <summary>
        /// アイテムコマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjItem;

        /// <summary>
        /// ステータスコマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjStatus;

        /// <summary>
        /// 逃げるコマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjEscape;

        /// <summary>
        /// コマンドのカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _cursorObjAttack.SetActive(false);
            _cursorObjGuard.SetActive(false);
            _cursorObjItem.SetActive(false);
            _cursorObjStatus.SetActive(false);
            _cursorObjEscape.SetActive(false);
        }

        /// <summary>
        /// 選択中のコマンドのカーソルを表示します。
        /// </summary>
        public void ShowSelectedCursor(BattleCommand command)
        {
            HideAllCursor();

            switch (command)
            {
                case BattleCommand.Attack:
                    _cursorObjAttack.SetActive(true);
                    break;
                case BattleCommand.Guard:
                    _cursorObjGuard.SetActive(true);
                    break;
                case BattleCommand.Item:
                    _cursorObjItem.SetActive(true);
                    break;
                case BattleCommand.Status:
                    _cursorObjStatus.SetActive(true);
                    break;
                case BattleCommand.Escape:
                    _cursorObjEscape.SetActive(true);
                    break;
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