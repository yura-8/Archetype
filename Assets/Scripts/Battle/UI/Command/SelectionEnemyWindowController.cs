using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するウィンドウを制御するクラスです。
    /// </summary>
    public class SelectionEnemyWindowController : MonoBehaviour, IBattleWindowController
    {
        /// <summary>
        /// 敵キャラクターの名前を表示するUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionEnemyUIController _uiController;

        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;
        private List<EnemyData> _activeEnemies; // 現在選択対象の敵リスト
        private int _selectedIndex;
        private bool _canSelect;
        private bool _pendingEnable;
        private float _timer;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
            if (_battleManager == null)
            {
                Debug.LogError("しかし、渡された battleManager は null でした！");
            }
        }

        void Update()
        {
            // 敵選択フェーズでなければ何もしない
            // 敵選択フェーズ かつ 入力許可時のみ処理
            if (_battleManager == null || _battleManager.BattlePhase != BattlePhase.SelectEnemy)
            {
                return;
            }
            if (_pendingEnable)
            {
                _timer -= Time.unscaledDeltaTime;
                if (_timer > 0) return;

                if (!InputGameKey.ConfirmButton() && !InputGameKey.CancelButton())
                {
                    _canSelect = true;
                    _pendingEnable = false;
                }
                else
                {
                    return; // まだ押しっぱなし
                }
            }
            if (!_canSelect) return;

            Debug.Log("[EnemyCmd] 入力受付中");
            SelectItem();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectItem()
        {
            if (_battleManager == null || _activeEnemies == null)
            {
                return;
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _selectedIndex--;
                // カーソルが一番上を通り越したら、一番下にループ
                if (_selectedIndex < 0)
                {
                    _selectedIndex = _activeEnemies.Count - 1;
                }
                _uiController.ShowSelectedCursor(_selectedIndex);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _selectedIndex++;
                // カーソルが一番下を通り越したら、一番上にループ
                if (_selectedIndex >= _activeEnemies.Count)
                {
                    _selectedIndex = 0;
                }
                _uiController.ShowSelectedCursor(_selectedIndex);
            }
            else if (InputGameKey.ConfirmButton())
            {
                // BattleManagerに「この敵が選ばれました」と通知
                if (_activeEnemies != null && _activeEnemies.Count > 0)
                {
                    _battleManager.OnTargetSelected(_activeEnemies[_selectedIndex]);
                }
            }
            else if (InputGameKey.CancelButton())
            {
                // BattleManagerに「キャンセルされました」と通知
                _battleManager.OnTargetCanceled();
            }
        }

        /// <summary>
        /// 敵選択を開始するための起動メソッドです。
        /// </summary>
        public void InitializeSelect()
        {
            if (_uiController == null)
            {
                Debug.LogError("UI Controllerが設定されていません！");
                return;
            }

            // EnemyDataManagerから現在の敵リストを取得
            _activeEnemies = EnemyDataManager.GetenemySatuses();
            _selectedIndex = 0;

            // UIの表示を更新
            _uiController.UpdateEnemyList(_activeEnemies);
            _uiController.ShowSelectedCursor(_selectedIndex);
            ShowWindow();
            _canSelect = false;
        }

        /// <summary>外部から選択可否を切り替える</summary>
        public void SetCanSelectState(bool state)
        {
            _canSelect = state;
            if (state)
            {
                _pendingEnable = true;  // Confirm を離すのを待つ
                _canSelect = false;
                _timer = 0.1f;
            }
            else
            {
                _pendingEnable = false;
                _canSelect = false;
                _timer = 0.1f;
            }
        }

    private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _activeEnemies.Count;
        }

        /// <summary>
        /// 現在選択中の敵が有効なターゲットか確認します。
        /// </summary>
        private bool IsValidSelection()
        {
            // 生きている敵だけを選択可能にする、などのロジックをここに追加できる
            if (!IsValidIndex(_selectedIndex)) return false;

            var targetEnemy = _activeEnemies[_selectedIndex];
            return targetEnemy.hp > 0;
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
            _canSelect = false;
            _pendingEnable = false;
        }
    }
}