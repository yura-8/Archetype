using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘関連のウィンドウ全体を管理するクラスです。
    /// </summary>
    public class BattleWindowManager : MonoBehaviour
    {
        /// <summary>
        /// ステータス表示のウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        List<StatusWindowController> _statusWindowController;

        /// <summary>
        /// 敵キャラクターの名前を表示するウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        List<EnemyNameWindowController> _enemyNameWindowController;

        /// <summary>
        /// コマンドウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MainCommandWindowController _mainCommandWindowController;

        /// <summary>
        /// コマンドウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        AttackCommandWindowController _attackCommandWindowController;

        /// <summary>
        /// 選択ウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionWindowController _selectItemWindowController;

        /// <summary>
        /// ウィンドウのコントローラのリストです。
        /// </summary>
        List<IBattleWindowController> _battleWindowControllers = new();

        void Start()
        {
            SetControllerList();
        }

        /// <summary>
        /// UIコントローラのリストをセットアップします。
        /// </summary>
        public void SetControllerList()
        {
            _battleWindowControllers = new List<IBattleWindowController>();


            _battleWindowControllers.AddRange(_statusWindowController);
            _battleWindowControllers.AddRange(_enemyNameWindowController);
            _battleWindowControllers.Add(_mainCommandWindowController);
            _battleWindowControllers.Add(_attackCommandWindowController);
            _battleWindowControllers.Add(_selectItemWindowController);

            //_battleWindowControllers = new()
            //{
            //    _statusWindowController,
            //    _enemyNameWindowController,
            //    _mainCommandWindowController,
            //    _attackCommandWindowController,
            //};
        }

        /// <summary>
        /// 各ウィンドウのコントローラをセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
        public void SetUpWindowControllers(BattleManager battleManager)
        {
            foreach (var controller in _battleWindowControllers)
            {
                // controller が null でないことを確認してから呼び出す
                if (controller != null)
                {
                    controller.SetUpController(battleManager);
                }
                else
                {
                    // もしnullが見つかったら、コンソールにエラーメッセージを出す
                    Debug.LogError("BattleWindowManagerのリストに null のコントローラが含まれています！インスペクターの設定を確認してください。");
                }
            }
        }

        /// <summary>
        /// 各UIを非表示にします。
        /// </summary>
        public void HideAllWindow()
        {
            foreach (var controller in _battleWindowControllers)
            {
                controller.HideWindow();
            }
        }

        /// <summary>
        /// ステータス表示のウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public List<StatusWindowController> GetStatusWindowController()
        {
            return _statusWindowController;
        }

        /// <summary>
        /// 敵キャラクターの名前を表示するウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public List<EnemyNameWindowController> GetEnemyNameWindowController()
        {
            return _enemyNameWindowController;
        }

        /// <summary>
        /// コマンドウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public MainCommandWindowController GetCommandWindowController()
        {
            return _mainCommandWindowController;
        }

        /// <summary>
        /// 攻撃コマンドウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public AttackCommandWindowController GetAttackCommandWindowController()
        {
            return _attackCommandWindowController;
        }

        /// <summary>
        /// 選択ウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public SelectionWindowController GetSelectionWindowController()
        {
            return _selectItemWindowController;
        }
    }
}