using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するウィンドウを制御するクラスです。
    /// </summary>
    public class SelectionPartyWindowController : MonoBehaviour, IBattleWindowController
    {
        /// <summary>
        /// 敵キャラクターの名前を表示するUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionPartyUIController _uiController;

        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 現在選択可能なパーティーメンバーリストです。
        /// </summary>
        private List<CharacterStatus> _partyMembers;

        /// <summary>
        /// 現在選択中のインデックスです。
        /// </summary>
        private int _selectedIndex;
        private bool _canSelect;
        private bool _pendingEnable;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
            if (_battleManager == null)
            {
                Debug.LogError("BattleManagerがnullです！");
            }
        }

        void Update()
        {
            //敵選択フェーズでなければ何もしない
            if (_battleManager == null || _battleManager.BattlePhase != BattlePhase.SelectParty)
            {
                return;
            }
            if (_pendingEnable)
            {
                if (!InputGameKey.ConfirmButton())
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

            Debug.Log("[PartyCmd] 入力受付中");
            SelectItem();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectItem()
        {
            if (_battleManager == null || _partyMembers == null)
            {
                return;
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _selectedIndex--;
                // カーソルが一番上を通り越したら、一番下にループ
                if (_selectedIndex < 0)
                {
                    _selectedIndex = _partyMembers.Count - 1;
                }
                _uiController.ShowSelectedCursor(_selectedIndex);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _selectedIndex++;
                // カーソルが一番下を通り越したら、一番上にループ
                if (_selectedIndex >= _partyMembers.Count)
                {
                    _selectedIndex = 0;
                }
                _uiController.ShowSelectedCursor(_selectedIndex);
            }
            else if (InputGameKey.ConfirmButton())
            {
                // BattleManagerに「この敵が選ばれました」と通知
                //if (_partyMembers != null && _partyMembers.Count > 0)
                if (IsValidSelection())
                {
                    _battleManager.OnPartySelected(_partyMembers[_selectedIndex]);
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

            // PartyDataManagerから現在の敵リストを取得
            _partyMembers = new List<CharacterStatus>();
            foreach (var characterId in GameDataManager.Instance.PartyCharacterIds)
            {
                var status = CharacterStatusManager.GetCharacterStatusById(characterId);
                if (status != null)
                {
                    _partyMembers.Add(status);
                }
            }

            _selectedIndex = 0;

            // UIの表示を更新
            _uiController.UpdatePartyList(_partyMembers);
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
                _pendingEnable = true;
                _canSelect = false;
            }
            else
            {
                _pendingEnable = false;
                _canSelect = false;
            }
        }

    private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _partyMembers.Count;
        }

        /// <summary>
        /// 現在選択中の敵が有効なターゲットか確認します。
        /// </summary>
        private bool IsValidSelection()
        {
            // 生きている敵だけを選択可能にする、などのロジックをここに追加できる
            if (!IsValidIndex(_selectedIndex)) return false;

            var targetCharacter = _partyMembers[_selectedIndex];
            return targetCharacter.currentHp > 0;
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