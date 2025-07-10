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
        SelectionEnemyWindowController _selectionEnemyWindowController;

        /// <summary>
        /// パーティーメンバーの名前を表示するウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionPartyWindowController _selectionPartyWindowController;

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

        // 説明文ウィンドウのコントローラへの参照
        [SerializeField]
        DescriptionWindowController _descriptionWindowController;

        // 説明文ウィンドウのコントローラへの参照
        [SerializeField]
        ConfirmationWindowController _confirmationWindowController;

        /// <summary>
        /// メッセージウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MessageWindowController _messageWindowController;

        /// <summary>
        /// ウィンドウのコントローラのリストです。
        /// </summary>
        List<IBattleWindowController> _battleWindowControllers = new();

        void Awake()
        {
            SetControllerList();
        }

        /// <summary>
        /// UIコントローラのリストをセットアップします。
        /// </summary>
        public void SetControllerList()
        {
            _battleWindowControllers = new List<IBattleWindowController>();
            //_battleWindowControllers.Clear();

            if (_statusWindowController != null)
            {
                _battleWindowControllers.AddRange(_statusWindowController);
            }
            _battleWindowControllers.Add(_mainCommandWindowController);
            _battleWindowControllers.Add(_attackCommandWindowController);
            _battleWindowControllers.Add(_selectItemWindowController);
            _battleWindowControllers.Add(_selectionEnemyWindowController);
            _battleWindowControllers.Add(_selectionPartyWindowController);
            _battleWindowControllers.Add(_descriptionWindowController);
            _battleWindowControllers.Add(_confirmationWindowController);
            _battleWindowControllers.Add(_messageWindowController);
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
                    var monoBehaviour = controller as MonoBehaviour;
                    Debug.Log($"  -> {monoBehaviour.GetType().Name} (ID:{monoBehaviour.GetInstanceID()}) に SetUpController を呼び出します。");
                    controller.SetUpController(battleManager);
                }
                else
                {
                    // もしnullが見つかったら、コンソールにエラーメッセージを出す
                    Debug.LogError("BattleWindowManagerのリストに null のコントローラが検出されました。インスペクターの設定を確認してください。");
                }
            }
        }

        /// <summary>
        /// 各UIを非表示にします。
        /// </summary>
        public void HideAllWindow()
        {
            foreach (var c in _battleWindowControllers)
                if (c != null) c.HideWindow();
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
        public SelectionEnemyWindowController GetSelectionEnemyWindowController()
        {
            return _selectionEnemyWindowController;
        }

        /// <summary>
        /// パーティーメンバーの名前を表示するウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public SelectionPartyWindowController GetSelectionPartyWindowController()
        {
            return _selectionPartyWindowController;
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

        // 説明文ウィンドウのコントローラを取得するメソッド
        public DescriptionWindowController GetDescriptionWindowController()
        {
            return _descriptionWindowController;
        }

        // 説明文ウィンドウのコントローラを取得するメソッド
        public ConfirmationWindowController GetConfirmationWindowController()
        {
            return _confirmationWindowController;
        }

        /// <summary>
        /// メッセージウィンドウの動作を制御するクラスへの参照を取得します。
        /// </summary>
        public MessageWindowController GetMessageWindowController()
        {
            return _messageWindowController;
        }
    }

}

