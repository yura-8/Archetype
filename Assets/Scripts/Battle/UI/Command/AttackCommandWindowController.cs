using UnityEngine;
using UnityEngine.InputSystem;

namespace SimpleRpg
{
    /// <summary>
    /// コマンドウィンドウを制御するクラスです。
    /// </summary>
    public class AttackCommandWindowController : MonoBehaviour, IBattleWindowController
    {
        /// <summary>
        /// コマンドウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        AttackCommandUIController uiController;

        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 現在選択中のコマンドです。
        /// </summary>
        AttackCommand _selectedAttackCommand;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        void Update()
        {
            if (_battleManager == null || _battleManager.BattlePhase != BattlePhase.InputCommand_Attack)
            {
                return;
            }
            Debug.Log("[AttackCmd] 入力受付中");
            SelectCommand();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectCommand()
        {
            if (_battleManager == null)
            {
                return;
            }

            if (_battleManager.BattlePhase != BattlePhase.InputCommand_Attack)
            {
                return;
            }

            if (Keyboard.current.upArrowKey.wasReleasedThisFrame)
            {
                SetPreCommand();
                uiController.ShowSelectedCursor(_selectedAttackCommand);
            }
            else if (Keyboard.current.downArrowKey.wasReleasedThisFrame)
            {
                SetNextCommand();
                uiController.ShowSelectedCursor(_selectedAttackCommand);
            }
            else if (InputGameKey.ConfirmButton())
            {
                _battleManager.OnCommandAttackSelected(_selectedAttackCommand);
            }
            else if (InputGameKey.CancelButton())
            {
                _battleManager.OnAttackCanceled();
            }
        }

        /// <summary>
        /// ひとつ前のコマンドを選択します。
        /// </summary>
        void SetPreCommand()
        {
            int currentCommand = (int)_selectedAttackCommand;
            int nextCommand = currentCommand - 1;
            if (nextCommand < 0)
            {
                nextCommand = 1;
            }
            _selectedAttackCommand = (AttackCommand)nextCommand;
        }

        /// <summary>
        /// ひとつ後のコマンドを選択します。
        /// </summary>
        void SetNextCommand()
        {
            int currentCommand = (int)_selectedAttackCommand;
            int nextCommand = currentCommand + 1;
            if (nextCommand > 1)
            {
                nextCommand = 0;
            }
            _selectedAttackCommand = (AttackCommand)nextCommand;
        }

        /// <summary>
        /// コマンド選択を初期化します。
        /// </summary>
        public void InitializeCommand()
        {
            _selectedAttackCommand = AttackCommand.Normal;
            uiController.ShowSelectedCursor(_selectedAttackCommand);
        }

        /// <summary>
        /// コマンドウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            uiController.Show();
        }

        /// <summary>
        /// コマンドウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            uiController.Hide();
        }
    }
}