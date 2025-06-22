using System.Collections.Generic;
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
            _battleWindowControllers.AddRange(_statusWindowController);

            //_battleWindowControllers = new()
            //{
            //    _statusWindowController,
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
                controller.SetUpController(battleManager);
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
    }
}