using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// コマンドのUIを制御するクラスです。
    /// </summary>
    public class AttackCommandUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// 通常攻撃コマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjNormal;

        /// <summary>
        /// スキルコマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjSkill;

        /// <summary>
        /// コマンドのカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _cursorObjNormal.SetActive(false);
            _cursorObjSkill.SetActive(false);
        }

        /// <summary>
        /// 選択中のコマンドのカーソルを表示します。
        /// </summary>
        public void ShowSelectedCursor(AttackCommand command)
        {
            HideAllCursor();

            switch (command)
            {
                case AttackCommand.Normal:
                    _cursorObjNormal.SetActive(true);
                    break;
                case AttackCommand.Skill:
                    _cursorObjSkill.SetActive(true);
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