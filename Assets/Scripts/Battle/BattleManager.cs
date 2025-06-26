using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘に関する機能を管理するクラスです。
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        /// <summary>
        /// 戦闘開始の処理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleStarter _battleStarter;

        /// <summary>
        /// 戦闘関連のウィンドウ全体を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleWindowManager _battleWindowManager;

        /// <summary>
        /// 戦闘関連のスプライトを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleSpriteController _battleSpriteController;

        /// <summary>
        /// 戦闘のフェーズです。
        /// </summary>
        public BattlePhase BattlePhase { get; private set; }

        /// <summary>
        /// 選択されたコマンドです。
        /// </summary>
        public BattleCommand SelectedCommand { get; private set; }

        /// <summary>
        /// 選択されたコマンドです。
        /// </summary>
        public AttackCommand SelectedAttackCommand { get; private set; }

        /// <summary>
        /// エンカウントした敵キャラクターのIDです。
        /// </summary>
        public List<int> EnemyId { get; private set; } = new List<int>();

        /// <summary>
        /// 戦闘開始からのターン数です。
        /// </summary>
        public int TurnCount { get; private set; }

        /// <summary>
        /// 戦闘のフェーズを変更します。
        /// </summary>
        /// <param name="battlePhase">変更後のフェーズ</param>
        public void SetBattlePhase(BattlePhase battlePhase)
        {
            BattlePhase = battlePhase;
        }

        /// <summary>
        /// 敵キャラクターのステータスをセットします。
        /// </summary>
        /// <param name="enemyId">敵キャラクターのID</param>
        public void SetUpEnemyStatus(List<int> enemyId)
        {
            EnemyId = enemyId;
        }

        /// <summary>
        /// 戦闘の開始処理を行います。
        /// </summary>
        public void StartBattle()
        {
            SimpleLogger.Instance.Log("戦闘を開始します。");
            GameStateManager.ChangeToBattle();
            SetBattlePhase(BattlePhase.ShowEnemy);

            _battleWindowManager.SetUpWindowControllers(this);
            _battleStarter.StartBattle(this);
        }

        /// <summary>
        /// ウィンドウの管理を行うクラスへの参照を取得します。
        /// </summary>
        public BattleWindowManager GetWindowManager()
        {
            return _battleWindowManager;
        }

        /// <summary>
        /// 戦闘関連のスプライトを制御するクラスへの参照を取得します。
        /// </summary>
        public BattleSpriteController GetBattleSpriteController()
        {
            return _battleSpriteController;
        }

        /// <summary>
        /// コマンド入力を開始します。
        /// </summary>
        public void StartInputCommandPhase()
        {
            SimpleLogger.Instance.Log($"コマンド入力のフェーズを開始します。現在のターン数: {TurnCount}");
            BattlePhase = BattlePhase.InputCommand_Main;

            var commandWindow = GetWindowManager().GetCommandWindowController();
            commandWindow.InitializeCommand();
            commandWindow.ShowWindow();
        }

        /// <summary>
        /// コマンドが選択された時のコールバックです。
        /// </summary>
        public void OnCommandSelected(BattleCommand selectedCommand)
        {
            //SimpleLogger.Instance.Log($"コマンドが選択されました: {selectedCommand}");
            //SelectedCommand = selectedCommand;
            //HandleCommand();

            switch (selectedCommand)
            {
                case BattleCommand.Attack: // 「たたかう」が選ばれたら…
                                          
                    // 1. メインコマンドウィンドウを隠す
                    //_mainCommandWindowController.HideWindow();

                    // 2. 攻撃コマンドウィンドウを表示する
                    //_attackCommandWindowController.InitializeCommand();
                    //_attackCommandWindowController.ShowWindow();

                    // 3. 戦闘フェーズを「攻撃コマンド入力」に切り替える
                    BattlePhase = BattlePhase.InputCommand_Attack;
                    break;

                    // ...他のコマンドが選ばれた場合の処理...
            }
        }

        /// <summary>
        /// コマンドが選択された時のコールバックです。
        /// </summary>
        public void OnCommandAttackSelected(AttackCommand selectedAttackCommand)
        {
            SimpleLogger.Instance.Log($"コマンドが選択されました: {selectedAttackCommand}");
            //SelectedAttackCommand = selectedAttackCommand;
            //HandleAttackCommand();

            // 次のフェーズ（ターゲット選択など）に進む処理
            BattlePhase = BattlePhase.SelectEnemy;
        }

        /// <summary>
        /// コマンド入力に応じた処理を行います。
        /// 行動中にステータスを変更
        /// </summary>
        void HandleCommand()
        {
            SimpleLogger.Instance.Log($"入力されたコマンドに応じた処理を行います。選択されたコマンド: {SelectedCommand}");
            BattlePhase = BattlePhase.Action;
        }
    }
}