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
            SimpleLogger.Instance.Log($"敵キャラクターのステータスをセットしました。ID: {string.Join(", ", enemyId)}");
        }

        /// <summary>
        /// 戦闘の開始処理を行います。
        /// </summary>
        public void StartBattle()
        {
            SimpleLogger.Instance.Log("戦闘を開始します。");
            GameStateManager.ChangeToBattle();
            SetBattlePhase(BattlePhase.ShowEnemy);

            EnemyDataManager.enemies = this.EnemyId;

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
            var windowManager = GetWindowManager();

            switch (selectedCommand)
            {
                case BattleCommand.Attack:
                    SetBattlePhase(BattlePhase.InputCommand_Attack);

                    var attackCommandWindow = GetWindowManager().GetAttackCommandWindowController();
                    attackCommandWindow.InitializeCommand();
                    attackCommandWindow.ShowWindow();
                    break;
                case BattleCommand.Item:
                    SetBattlePhase(BattlePhase.SelectItem);

                    var itemCommandWindow = GetWindowManager().GetSelectionWindowController();
                    itemCommandWindow.InitializeSelect();
                    itemCommandWindow.ShowWindow();
                    GetWindowManager().GetDescriptionWindowController().ShowWindow();
                    ShowSelectionWindow();
                    break;
                case BattleCommand.Guard:
                case BattleCommand.Escape:
                    SetBattlePhase(BattlePhase.InputCommand_confirm);
                    StartCoroutine(ShowConfirmationProcess());
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
            var windowManager = GetWindowManager();
            //windowManager.GetAttackCommandWindowController().HideWindow();

            switch (SelectedAttackCommand)
            {
                case AttackCommand.Normal:
                    Debug.Log("攻撃対象を選択してください。");
                    SetBattlePhase(BattlePhase.SelectEnemy);
                    windowManager.GetSelectionEnemyWindowController().InitializeSelect();
                    StartCoroutine(ShowEnemySelectProcess());
                    windowManager.GetDescriptionWindowController().HideWindow();
                    break;

                case AttackCommand.Skill:
                    SetBattlePhase(BattlePhase.SelectSkill);
                    windowManager.GetDescriptionWindowController().ShowWindow();
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
            if (SelectedCommand == BattleCommand.Attack && SelectedAttackCommand == AttackCommand.Skill)
            {
                SetBattlePhase(BattlePhase.SelectSkill);
            }
            else if (SelectedCommand == BattleCommand.Item)
            {
                SetBattlePhase(BattlePhase.SelectItem);
            }

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
            GetWindowManager().GetSelectionWindowController().HideWindow();

            switch (targetType)
            {
                case EffectTarget.EnemySolo:
                    // 敵単体がターゲットなら、敵選択フェーズへ
                    Debug.Log("対象の敵を選択してください。");
                    SetBattlePhase(BattlePhase.SelectEnemy);
                    StartCoroutine(ShowEnemySelectProcess());
                    break;

                case EffectTarget.FriendSolo:
                    // 味方単体がターゲットなら、味方選択フェーズへ
                    Debug.Log("対象のパーティーメンバーを選択してください。");
                    SetBattlePhase(BattlePhase.SelectParty);
                    StartCoroutine(ShowPartySelectProcess());
                    break;

                // ここに EnemyAll（敵全体）や FriendAll（味方全体）などのケースも追加していく

                default:
                    Debug.Log("ターゲット選択不要。行動を実行します。");
                    SetBattlePhase(BattlePhase.Action);
                    GetWindowManager().GetDescriptionWindowController().HideWindow();
                    // 行動実行処理へ
                    break;
            }
        }

        /// <summary>
        /// ターゲットの選択後
        /// </summary>
        /// <param name="targetType">スキルやアイテムのターゲットタイプ</param>
        public void OnTargetSelected(EnemyData target)
        {
            Debug.Log($"{target.enemyName} がターゲットに選択されました。");
            SetBattlePhase(BattlePhase.Action);
            GetWindowManager().GetDescriptionWindowController().HideWindow();
            // ここから、実際にダメージを与えるなどの行動実行処理を呼び出すことになります。
            // StartCoroutine(ExecutePlayerAction());
        }

        /// <summary>
        /// パーティーメンバーが選択された時のコールバックです。
        /// </summary>
        public void OnPartySelected(CharacterStatus target)
        {
            Debug.Log($"{CharacterDataManager.GetCharacterName(target.characterId)} がターゲットに選択されました。");
            SetBattlePhase(BattlePhase.Action);
            GetWindowManager().GetDescriptionWindowController().HideWindow();
        }

        /// <summary>
        /// 選択ウィンドウでキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnAttackCanceled()
        {
            var windowManager = GetWindowManager();
            Debug.Log("攻撃コマンド選択のキャンセル処理が実行されました！");
            windowManager.GetAttackCommandWindowController().HideWindow();
            windowManager.GetCommandWindowController().ShowWindow();
            SetBattlePhase(BattlePhase.InputCommand_Main);
        }

        public void OnSelectionCanceled()
        {
            var windowManager = GetWindowManager();
            Debug.Log("選択コマンド選択のキャンセル処理が実行されました！");

            switch (BattlePhase)
            {
                // アイテムリスト選択中にキャンセル
                case BattlePhase.SelectItem:
                    Debug.Log("アイテムコマンド選択のキャンセル処理が実行されました！");
                    windowManager.GetSelectionWindowController().HideWindow();
                    GetWindowManager().GetDescriptionWindowController().HideWindow();
                    windowManager.GetCommandWindowController().ShowWindow();
                    // メインコマンド選択に戻る
                    SetBattlePhase(BattlePhase.InputCommand_Main);
                    break;

                // スキルリスト選択中にキャンセル
                case BattlePhase.SelectSkill:
                    Debug.Log("スキルコマンド選択のキャンセル処理が実行されました！");
                    windowManager.GetSelectionWindowController().HideWindow();
                    GetWindowManager().GetDescriptionWindowController().HideWindow();
                    // 「通常攻撃/スキル」選択に戻る
                    windowManager.GetAttackCommandWindowController().ShowWindow();
                    SetBattlePhase(BattlePhase.InputCommand_Attack);
                    break;

                default:
                    Debug.Log("このフェーズではキャンセルできません。");
                    break;
            }
        }

        public void OnTargetCanceled()
        {
            var windowManager = GetWindowManager();
            Debug.Log("対象コマンド選択のキャンセル処理が実行されました！");
            GetWindowManager().GetDescriptionWindowController().HideWindow();

            switch (BattlePhase)
            {
                // 敵選択中にキャンセル
                case BattlePhase.SelectEnemy:
                     windowManager.GetSelectionEnemyWindowController().HideWindow();
                    windowManager.GetAttackCommandWindowController().ShowWindow();

                    Debug.Log("敵選択のキャンセル処理が実行されました！");
                    // どこから来たかに応じて戻り先を変更
                    if (SelectedAttackCommand == AttackCommand.Normal)
                    {
                        Debug.Log("通常攻撃のキャンセル");
                        // 「通常攻撃/スキル」選択に戻る;
                        SetBattlePhase(BattlePhase.InputCommand_Attack);
                    }
                    else if (SelectedAttackCommand == AttackCommand.Skill)
                    {
                        Debug.Log("スキルのキャンセル");
                        // スキルのリスト選択に戻る
                        //windowManager.GetAttackCommandWindowController().ShowWindow();
                        GetWindowManager().GetDescriptionWindowController().ShowWindow();
                        SetBattlePhase(BattlePhase.SelectSkill);
                        ShowSelectionWindow();
                    }
                    else
                    {
                        Debug.Log("不明なキャンセル");
                    }
                    break;

                // 味方選択中にキャンセル
                case BattlePhase.SelectParty:
                    // 味方選択カーソルを非表示にする処理
                    windowManager.GetSelectionPartyWindowController().HideWindow();
                    windowManager.GetAttackCommandWindowController().ShowWindow();
                    Debug.Log("パーティー選択のキャンセル処理が実行されました！");

                    if (SelectedAttackCommand == AttackCommand.Skill)
                    {
                        // スキルのリスト選択に戻る
                        GetWindowManager().GetDescriptionWindowController().ShowWindow();
                        SetBattlePhase(BattlePhase.SelectSkill);
                        ShowSelectionWindow();
                    }
                    else if(SelectedCommand == BattleCommand.Item)
                    {
                        // アイテムのリスト選択に戻る
                        GetWindowManager().GetDescriptionWindowController().ShowWindow();
                        SetBattlePhase(BattlePhase.SelectItem);
                        ShowSelectionWindow();
                    }
                    break;

                default:
                    Debug.Log("対象選択のフェーズではキャンセルできません。");
                    break;
            }
        }

        // 敵選択ウィンドウを 1 フレーム遅らせて開く
        IEnumerator ShowEnemySelectProcess()
        {
            yield return null;                         // ―― 1 フレーム待つ
            var enemySel = GetWindowManager().GetSelectionEnemyWindowController();
            enemySel.InitializeSelect();               // ← 再初期化安全
            enemySel.ShowWindow();
            enemySel.SetCanSelectState(true);
            //GetWindowManager().GetDescriptionWindowController().ShowWindow();
        }

        // 味方選択ウィンドウ版
        IEnumerator ShowPartySelectProcess()
        {
            yield return null;
            var partySel = GetWindowManager().GetSelectionPartyWindowController();
            partySel.InitializeSelect();
            partySel.ShowWindow();
            partySel.SetCanSelectState(true);
            //GetWindowManager().GetDescriptionWindowController().ShowWindow();
        }

        /// <summary>
        /// 確認ウィンドウを表示するコルーチンです。
        /// </summary>
        private IEnumerator ShowConfirmationProcess()
        {
            GetWindowManager().GetConfirmationWindowController().ShowWindow();
            string question = "";

            if (SelectedCommand == BattleCommand.Guard)
            {
                question = "GUARDしますか？";
            }
            else if (SelectedCommand == BattleCommand.Escape)
            {
                question = "ESCAPEしますか？";
            }

            // ★ 修正点: ShowWindow(question) から Activate(question) に変更
            GetWindowManager().GetConfirmationWindowController().Activate(question);

            yield return null;

            GetWindowManager().GetConfirmationWindowController().SetCanSelect(true);
        }

        /// <summary>
        /// 確認ウィンドウで「はい」が選択されたときの処理です。
        /// </summary>
        public void OnConfirmationAccepted()
        {
            GetWindowManager().GetConfirmationWindowController().HideWindow();

            // ★★★ 以下に処理を書き足す ★★★

            // どのコマンドが選択されていたかに応じて処理を分岐
            switch (SelectedCommand)
            {
                case BattleCommand.Guard:
                    Debug.Log("「ぼうぎょ」の準備をします。");
                    // ここで、キャラクターが防御状態になる、などの実際のゲームロジックを将来的に追加します。
                    break;

                case BattleCommand.Escape:
                    Debug.Log("「にげる」を試みます！");
                    // ここで、逃走が成功するかどうかの確率計算などのゲームロジックを将来的に追加します。
                    // 成功したら戦闘終了、失敗したらメッセージ表示、などの処理に繋げます。
                    break;
            }

            // プレイヤーの行動をセットしたので、行動実行フェーズに進む
            SetBattlePhase(BattlePhase.Action);
        }

        // BattleManager.cs

        /// <summary>
        /// 確認ウィンドウで「いいえ」またはキャンセルが選択されたときの処理です。
        /// </summary>
        public void OnConfirmationCanceled()
        {
            // 1. 「はい／いいえ」のウィンドウを非表示にする
            GetWindowManager().GetConfirmationWindowController().HideWindow();

            // 3. ゲームの状態を「メインコマンド入力待ち」に戻す
            SetBattlePhase(BattlePhase.InputCommand_Main);
        }
    }
}