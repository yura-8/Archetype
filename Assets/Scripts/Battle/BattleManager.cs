using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

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
            SimpleLogger.Instance.Log($"コマンドが選択されました: {selectedCommand}");
            SelectedCommand = selectedCommand;
            //HandleCommand();

            switch (selectedCommand)
            {
                case BattleCommand.Attack:
                    var attackCommandWindow = GetWindowManager().GetAttackCommandWindowController();
                    attackCommandWindow.InitializeCommand();
                    attackCommandWindow.ShowWindow();

                    SetBattlePhase(BattlePhase.InputCommand_Attack);
                    break;

                // ...他のコマンドが選ばれた場合の処理...
                case BattleCommand.Item:
                    var itemCommandWindow = GetWindowManager().GetSelectionWindowController();
                    itemCommandWindow.InitializeSelect();
                    itemCommandWindow.ShowWindow();

                    SetBattlePhase(BattlePhase.SelectItem);
                    ShowSelectionWindow();
                    break;
                case BattleCommand.Escape:
                    SetBattlePhase(BattlePhase.Action);
                    break;
            }
        }

        /// <summary>
        /// コマンドが選択された時のコールバックです。
        /// </summary>
        public void OnCommandAttackSelected(AttackCommand selectedAttackCommand)
        {
            SimpleLogger.Instance.Log($"攻撃コマンド選択: {selectedAttackCommand}");
            SelectedAttackCommand = selectedAttackCommand;

            switch (SelectedAttackCommand)
            {
                case AttackCommand.Normal:
                    Debug.Log("攻撃対象を選択してください。");
                    SetBattlePhase(BattlePhase.SelectEnemy);
                    break;

                case AttackCommand.Skill:
                    SetBattlePhase(BattlePhase.SelectSkill);
                    ShowSelectionWindow();
                    break;
            }
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        void ShowSelectionWindow()
        {
            SimpleLogger.Instance.Log($"ShowSelectionWindow()が呼ばれました。選択されたコマンド: {SelectedCommand}");
            StartCoroutine(ShowSelectionWindowProcess());
        }

        /// <summary>
        /// 選択ウィンドウを表示する処理です。
        /// </summary>
        IEnumerator ShowSelectionWindowProcess()
        {
            yield return null;
            BattlePhase = BattlePhase.SelectItem;
            var selectionWindowController = _battleWindowManager.GetSelectionWindowController();
            selectionWindowController.SetUpWindow();
            selectionWindowController.SetPageElement();
            selectionWindowController.ShowWindow();
            selectionWindowController.SetCanSelectState(true);
        }

        /// <summary>
        /// 選択ウィンドウで項目が選択された時のコールバックです。
        /// </summary>
        public void OnItemSelected(int itemId)
        {
            // スキルかアイテムかで処理を分ける
            if (SelectedCommand == BattleCommand.Attack && SelectedAttackCommand == AttackCommand.Skill)
            {
                // スキルの場合
                var skillData = SkillDataManager.GetSkillDataById(itemId);
                if (skillData == null) return;
                SimpleLogger.Instance.Log($"スキル選択: {skillData.skillName} (ターゲット: {skillData.skillEffect.effectTarget})");
                DetermineNextPhaseByTarget(skillData.skillEffect.effectTarget);
            }
            else if (SelectedCommand == BattleCommand.Item)
            {
                // アイテムの場合 (ItemDataManagerのようなものがあると仮定)
                var itemData = ItemDataManager.GetItemDataById(itemId);
                if (itemData == null) return;
                SimpleLogger.Instance.Log($"アイテム選択: {itemData.itemName} (ターゲット: {itemData.itemEffect.effectTarget})");
                DetermineNextPhaseByTarget(itemData.itemEffect.effectTarget);
            }
        }

        /// <summary>
        /// ターゲットの種類に応じて次のフェーズを決定します。
        /// </summary>
        /// <param name="targetType">スキルやアイテムのターゲットタイプ</param>
        private void DetermineNextPhaseByTarget(EffectTarget targetType)
        {
            switch (targetType)
            {
                case EffectTarget.EnemySolo:
                    // 敵単体がターゲットなら、敵選択フェーズへ
                    Debug.Log("対象の敵を選択してください。");
                    SetBattlePhase(BattlePhase.SelectEnemy);
                    // 敵選択カーソル表示の処理へ
                    break;

                case EffectTarget.FriendSolo:
                    // 味方単体がターゲットなら、味方選択フェーズへ
                    Debug.Log("対象の味方を選択してください。");
                    SetBattlePhase(BattlePhase.SelectFriend);
                    // 味方選択カーソル表示の処理へ
                    break;

                // ここに EnemyAll（敵全体）や FriendAll（味方全体）などのケースも追加していく

                default:
                    Debug.Log("ターゲット選択不要。行動を実行します。");
                    SetBattlePhase(BattlePhase.Action);
                    // 行動実行処理へ
                    break;
            }
        }

        /// <summary>
        /// 選択ウィンドウでキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnItemCanceled()
        {
            Debug.Log($"キャンセルが要求されました。現在のフェーズ: {BattlePhase}");

            var windowManager = GetWindowManager();

            // 現在のフェーズに応じて処理を分岐
            switch (BattlePhase)
            {
                // 「通常攻撃/スキル」選択中にキャンセル → メインコマンド選択に戻る
                case BattlePhase.InputCommand_Attack:
                    SetBattlePhase(BattlePhase.InputCommand_Main);
                    windowManager.GetAttackCommandWindowController().HideWindow();
                    break;

                // スキル/アイテムリスト選択中にキャンセル
                case BattlePhase.SelectItem:
                    windowManager.GetSelectionWindowController().HideWindow();
                    // アイテム選択から来たなら → メインコマンド選択に戻る
                    SetBattlePhase(BattlePhase.InputCommand_Main);
                    break;

                // スキル/アイテムリスト選択中にキャンセル
                case BattlePhase.SelectSkill:
                    windowManager.GetSelectionWindowController().HideWindow();
                    // スキル選択から来たなら → 「通常攻撃/スキル」選択に戻る
                    SetBattlePhase(BattlePhase.InputCommand_Attack);
                    break;
                // 敵選択中にキャンセル
                case BattlePhase.SelectEnemy:
                    // 敵選択カーソルを非表示にする処理
                    // windowManager.GetEnemyNameWindowController().HideCursor();

                    // どこから来たかに応じて戻り先を変更
                    if (SelectedAttackCommand == AttackCommand.Normal)
                    {
                        // 通常攻撃から来たなら → 「通常攻撃/スキル」選択に戻る
                        SetBattlePhase(BattlePhase.InputCommand_Attack);
                    }
                    else if (SelectedAttackCommand == AttackCommand.Skill)
                    {
                        // スキルのリスト選択に戻る
                        SetBattlePhase(BattlePhase.SelectSkill);
                    }else
                    {
                        // アイテムのリスト選択に戻る
                        SetBattlePhase(BattlePhase.SelectItem);
                    }
                    break;

                // 味方選択中にキャンセル
                case BattlePhase.SelectFriend:
                    // 味方選択カーソルを非表示にする処理
                    // windowManager.GetTargetSelectionController().HideCursor();

                    if (SelectedAttackCommand == AttackCommand.Skill)
                    {
                        // スキルのリスト選択に戻る
                        SetBattlePhase(BattlePhase.SelectSkill);
                    }
                    else
                    {
                        // アイテムのリスト選択に戻る
                        SetBattlePhase(BattlePhase.SelectItem);
                    }
                    break;

                // これ以上戻れないメインコマンド選択画面では、キャンセルは効かない（何もしない）
                case BattlePhase.InputCommand_Main:
                default:
                    Debug.Log("このフェーズではキャンセルできません。");
                    break;
            }
        }
    }
}