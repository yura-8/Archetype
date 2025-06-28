using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

namespace SimpleRpg
{
    /// <summary>
    /// 選択ウィンドウを制御するクラスです。
    /// </summary>
    public class SelectionWindowController : MonoBehaviour, IBattleWindowController
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// スキルの選択ウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionSkillUIController _skillUiController;

        /// <summary>
        /// アイテムのの選択ウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionItemUIController _itemUiController;

        /// <summary>
        /// 選択ウィンドウにてスキルに関する処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionWindowSkillController _skillController;

        /// <summary>
        /// 選択ウィンドウにてアイテムに関する処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionWindowItemController _itemController;

        /// <summary>
        /// 現在選択中の項目のインデックスです。
        /// </summary>
        int _selectedIndex;

        /// <summary>
        /// 現在のページ数です。
        /// </summary>
        int _page;

        /// <summary>
        /// 項目選択が可能かどうかのフラグです。
        /// </summary>
        bool _canSelect;

        private bool IsSkillMode => _battleManager.SelectedCommand == BattleCommand.Attack && _battleManager.SelectedAttackCommand == AttackCommand.Skill;
        private bool IsItemMode => _battleManager.SelectedCommand == BattleCommand.Item;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        void Update()
        {
            SelectItem();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectItem()
        {
            if (_battleManager == null)
            {
                return;
            }

            if (_battleManager.BattlePhase != BattlePhase.SelectItem)
            {
                return;
            }

            if (!_canSelect)
            {
                return;
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                SelectUpperItem();
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                SelectLowerItem();
            }
            else if (InputGameKey.ConfirmButton())
            {
                OnPressedConfirmButton();
            }
            else if (InputGameKey.CancelButton())
            {
                OnPressedCancelButton();
            }
        }

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        bool IsValidIndex(int index)
        {
            if (IsSkillMode) return _skillController.IsValidIndex(index);
            if (IsItemMode) return _itemController.IsValidIndex(index);
            return false;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// 魔法の場合は消費MPを確認、アイテムの場合は所持数を確認します。
        /// </summary>
        bool IsValidSelection()
        {
            if (IsSkillMode) return _skillController.IsValidSelection(_selectedIndex);
            if (IsItemMode) return _itemController.IsValidSelection(_selectedIndex);
            return false;
        }

        /// <summary>
        /// ひとつ上の項目を選択します。
        /// </summary>
        void SelectUpperItem()
        {
            int newIndex = _selectedIndex - 1;

            // 移動先のインデックスが有効範囲内なら移動を実行
            if (IsValidIndex(newIndex))
            {
                _selectedIndex = newIndex;

                // ページが変わったか確認し、必要ならページ表示を更新
                UpdatePageAndCursor();
            }

            //if (IsUpperRow())
            //{
            //    if (_page > 0)
            //    {
            //        _page -= 1;
            //        SetPageElement();
            //        _selectedIndex = _page * 4;
            //    }
            //}
            //else
            //{
            //    if (IsValidIndex(_selectedIndex - 2))
            //    {
            //        _selectedIndex -= 2;
            //    }
            //}

            //ShowSelectionCursor();
        }

        /// <summary>
        /// ひとつ下の項目を選択します。
        /// </summary>
        void SelectLowerItem()
        {
            int newIndex = _selectedIndex + 1;

            // 移動先のインデックスが有効範囲内なら移動を実行
            if (IsValidIndex(newIndex))
            {
                _selectedIndex = newIndex;

                // ページが変わったか確認し、必要ならページ表示を更新
                UpdatePageAndCursor();
            }

            //if (IsUpperRow())
            //{
            //    if (IsValidIndex(_selectedIndex + 2))
            //    {
            //        _selectedIndex += 2;
            //    }
            //}
            //else
            //{
            //    if (_page < GetMaxPageNum() - 1)
            //    {
            //        _page += 1;
            //        SetPageElement();
            //        _selectedIndex = _page * 4;
            //    }
            //}

            //ShowSelectionCursor();
        }

        /// <summary>
        /// ページを更新し、カーソルを再表示するヘルパーメソッド
        /// </summary>
        void UpdatePageAndCursor()
        {
            // 現在のインデックスから新しいページ番号を計算
            int newPage = _selectedIndex / 4;

            // ページが実際に変わっていたら、ページの要素を再描画
            if (newPage != _page)
            {
                _page = newPage;
                SetPageElement();
            }

            // カーソルを表示
            ShowSelectionCursor();
        }

        /// <summary>
        /// 現在の選択位置が上側の行かどうかを確認します。
        /// </summary>
        bool IsUpperRow()
        {
            int currentRow = _selectedIndex % 4;
            int upperRowMax = 1;
            return currentRow <= upperRowMax;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        int GetMaxPageNum()
        {
            if (IsSkillMode) return _skillController.GetMaxPageNum();
            if (IsItemMode) return _itemController.GetMaxPageNum();
            return 0;
        }

        /// <summary>
        /// 選択中の位置に応じたカーソルを表示します。
        /// </summary>
        void ShowSelectionCursor()
        {
            int index = _selectedIndex % 4;
            if (IsSkillMode) _skillUiController.ShowSelectedCursor(index);
            else if (IsItemMode) _itemUiController.ShowSelectedCursor(index);
        }

        /// <summary>
        /// ウィンドウの状態をセットアップします。
        /// </summary>
        public void SetUpWindow()
        {
            if (IsSkillMode)
            {
                _skillUiController.SetUpControllerDictionary();
                _skillUiController.ClearAllSkillText();
                _skillUiController.ClearDescriptionText();
                _skillController.SetCharacterSkill();
            }
            else if (IsItemMode)
            {
                _itemUiController.SetUpControllerDictionary();
                _itemUiController.ClearAllItemText();
                _itemUiController.ClearDescriptionText();
                _itemController.SetDisplayItems(); // ※アイテム側のロジックを想定
            }
            InitializeSelect();
        }

        /// <summary>
        /// 項目選択を初期化します。
        /// </summary>
        public void InitializeSelect()
        {
            _page = 0;
            _selectedIndex = 0;
            
            if (_battleManager.SelectedCommand == BattleCommand.Attack && _battleManager.SelectedAttackCommand == AttackCommand.Skill)
            {
                _skillUiController.ShowSelectedCursor(_selectedIndex);
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                _itemUiController.ShowSelectedCursor(_selectedIndex);
            }
        }

        /// <summary>
        /// ページ内の項目をセットします。
        /// </summary>
        public void SetPageElement()
        {
            if (_battleManager.SelectedCommand == BattleCommand.Attack && _battleManager.SelectedAttackCommand == AttackCommand.Skill)
            {
                _skillController.SetPageSkill(_page, _skillUiController);
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                _itemController.SetPageItem(_page, _itemUiController);
            }

            // ページ送りのカーソルの表示状態を確認します。
            bool isVisiblePrevCursor = _page > 0;
            bool isVisibleNextCursor = _page < GetMaxPageNum() - 1;

            if (_battleManager.SelectedCommand == BattleCommand.Attack && _battleManager.SelectedAttackCommand == AttackCommand.Skill)
            {
                _skillController.SetPageSkill(_page, _skillUiController);
                _skillUiController.SetPrevCursorVisibility(isVisiblePrevCursor);
                _skillUiController.SetNextCursorVisibility(isVisibleNextCursor);
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                _itemController.SetPageItem(_page, _itemUiController);
                _itemUiController.SetPrevCursorVisibility(isVisiblePrevCursor);
                _itemUiController.SetNextCursorVisibility(isVisibleNextCursor);
            }
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        void OnPressedConfirmButton()
        {
            if (!IsValidSelection())
            {
                return;
            }

            if (_battleManager.SelectedCommand == BattleCommand.Attack && _battleManager.SelectedAttackCommand == AttackCommand.Skill)
            {
                var skillData = _skillController.GetSkillData(_selectedIndex);
                if (skillData != null)
                {
                    _battleManager.OnItemSelected(skillData.skillId);
                    HideWindow();
                    SetCanSelectState(false);
                }
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                var itemInfo = _itemController.GetItemInfo(_selectedIndex);
                if (itemInfo != null)
                {
                    _battleManager.OnItemSelected(itemInfo.itemId);
                    HideWindow();
                    SetCanSelectState(false);
                }
            }
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        void OnPressedCancelButton()
        {
            _battleManager.OnItemCanceled();
            HideWindow();
            SetCanSelectState(false);
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            if (_battleManager.SelectedCommand == BattleCommand.Attack && _battleManager.SelectedAttackCommand == AttackCommand.Skill)
            {
                _skillUiController.Show();
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                _itemUiController.Show();
            }
        }

        /// <summary>
        /// 選択ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            if (_battleManager.SelectedCommand == BattleCommand.Attack && _battleManager.SelectedAttackCommand == AttackCommand.Skill)
            {
                _skillUiController.Hide();
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                _itemUiController.Hide();
            }
        }

        /// <summary>
        /// 項目選択が可能かどうかの状態を設定します。
        /// </summary>
        /// <param name="canSelect">項目選択が可能かどうか</param>
        public void SetCanSelectState(bool canSelect)
        {
            _canSelect = canSelect;
        }
    }
}