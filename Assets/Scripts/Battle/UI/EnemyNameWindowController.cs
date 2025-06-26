using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するウィンドウを制御するクラスです。
    /// </summary>
    public class EnemyNameWindowController : MonoBehaviour, IBattleWindowController
    {
        /// <summary>
        /// 敵キャラクターの名前を表示するUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        EnemyNameUIController _uiController;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
        public void SetUpController(BattleManager battleManager)
        {

        }

        /// <summary>
        /// 敵キャラクターの名前をセットします。
        /// </summary>
        /// <param name="enemyName">敵キャラクターの名前</param>
        public void SetEnemyName(string enemyName)
        {
            _uiController.SetEnemyName(enemyName);
        }

        /// <summary>
        /// 敵キャラクターの名前を空欄にします。
        /// </summary>
        public void ClearEnemyName()
        {
            _uiController.ClearEnemyName();
        }

        /// <summary>
        /// コマンドウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
        }

        /// <summary>
        /// コマンドウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }
    }
}