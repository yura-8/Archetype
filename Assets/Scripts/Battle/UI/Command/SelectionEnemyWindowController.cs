using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq; // ★Linqを使用するため追加

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するウィンドウを制御するクラスです。
    /// </summary>
    public class SelectionEnemyWindowController : MonoBehaviour, IBattleWindowController
    {
        [SerializeField]
        SelectionEnemyUIController _uiController;
        BattleManager _battleManager;

        // ★★★ 変更点: EnemyDataからEnemyStatusのリストに変更
        private List<EnemyStatus> _displayEnemies; // UIに表示されている順の敵リスト
        private List<EnemyStatus> _selectableEnemies; // 操作対象となる「生きている」敵のリスト

        private int _currentUIIndex; // UI上でのカーソル位置
        private bool _canSelect;
        private bool _pendingEnable;
        private float _timer;

        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        void Update()
        {
            if (_battleManager == null || _battleManager.BattlePhase != BattlePhase.SelectEnemy) return;
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
                    return;
                }
            }
            if (!_canSelect) return;

            SelectItem();
        }

        void SelectItem()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                MoveCursor(1); // 下へ
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                MoveCursor(-1); // 上へ
            }
            else if (InputGameKey.ConfirmButton())
            {
                if (_displayEnemies != null && _displayEnemies.Count > _currentUIIndex)
                {
                    // ★★★ 変更点: OnTargetSelectedにEnemyStatusを渡す
                    _battleManager.OnTargetSelected(_displayEnemies[_currentUIIndex]);
                }
            }
            else if (InputGameKey.CancelButton())
            {
                _battleManager.OnTargetCanceled();
            }
        }

        /// ★★★ 新規追加: カーソル移動と defeated スキップ処理
        private void MoveCursor(int direction)
        {
            if (_selectableEnemies == null || _selectableEnemies.Count == 0) return;

            // 現在選択されている敵が、生きている敵リストの中で何番目かを探す
            var currentSelectableEnemy = _displayEnemies[_currentUIIndex];
            int currentSelectableIndex = _selectableEnemies.IndexOf(currentSelectableEnemy);

            // 次のインデックスを計算
            int nextSelectableIndex = (currentSelectableIndex + direction + _selectableEnemies.Count) % _selectableEnemies.Count;

            // 次に選択すべき敵を取得
            var nextEnemyToSelect = _selectableEnemies[nextSelectableIndex];

            // UI全体リストの中で、その敵が何番目かを探してカーソルを合わせる
            _currentUIIndex = _displayEnemies.IndexOf(nextEnemyToSelect);

            _uiController.ShowSelectedCursor(_currentUIIndex);
        }

        public void InitializeSelect()
        {
            if (_uiController == null)
            {
                Debug.LogError("UI Controllerが設定されていません！");
                return;
            }

            // ★★★ 変更点: 内部ロジックを全面的に修正
            // 戦闘中の全ての敵ステータスリストを取得
            _displayEnemies = _battleManager.GetEnemyStatusManager().GetEnemyStatusList();

            // その中から「生きている敵」だけを絞り込む
            _selectableEnemies = _displayEnemies.Where(e => !e.isDefeated).ToList();

            // もし生きている敵がいないなら、処理を中断
            if (_selectableEnemies.Count == 0)
            {
                Debug.LogWarning("選択可能な敵がいません。");
                // 必要であればキャンセル処理などを呼び出す
                _battleManager.OnTargetCanceled();
                return;
            }

            // 最初の選択肢を、生きている敵の先頭に設定
            _currentUIIndex = _displayEnemies.IndexOf(_selectableEnemies[0]);

            // UIの表示を更新
            _uiController.UpdateEnemyList(_displayEnemies);
            _uiController.ShowSelectedCursor(_currentUIIndex);
            ShowWindow();
            _canSelect = false;
        }

        public void SetCanSelectState(bool state)
        {
            _canSelect = state;
            if (state)
            {
                _pendingEnable = true;
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

        public void ShowWindow()
        {
            _uiController.Show();
        }

        public void HideWindow()
        {
            _uiController.Hide();
            _canSelect = false;
            _pendingEnable = false;
        }
    }
}