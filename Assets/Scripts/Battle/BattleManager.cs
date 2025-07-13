using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘に関する機能を管理するクラスです。
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
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
        /// 戦闘が終了したかどうかのフラグです。
        /// </summary>
        public bool IsBattleFinished { get; private set; }

        /// <summary>
        /// 行動中のキャラクター名を表示するテキストへの参照です。
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _currentActorNameText;

        /// <summary>
        /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
        /// </summary>
        //[SerializeField]
        //CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// 敵キャラクターのコマンドを選択するクラスへの参照です。
        /// </summary>
        [SerializeField]
        EnemyCommandSelector _enemyCommandSelector;

        /// <summary>
        /// 戦闘中のアクションを登録するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionRegister _battleActionRegister;

        /// <summary>
        /// 戦闘中のアクションを処理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionProcessor _battleActionProcessor;

        [SerializeField]
        private EnemyStatusUIManager _enemyStatusUIManager;

        /// <summary>
        /// 戦闘の結果処理を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleResultManager _battleResultManager;

        private TemperatureState _currentTemperature;

        // 外部から現在の温度を取得するためのプロパティ
        public TemperatureState CurrentTemperature => _currentTemperature;

        // 緊急バッテリーシステムの管理用変数を追加 
        private const int MAX_EMERGENCY_BATTERY_USES = 3;
        private int _emergencyBatteryUsesLeft;

        [Header("BGM設定")]
        [SerializeField]
        private AudioClip _battleBgm; // Inspectorから設定するBGMファイル

        private AudioSource _bgmAudioSource; // BGM再生用のAudioSource

        /// <summary>
        /// 現在コマンドを入力しているパーティメンバーのインデックスです。
        /// </summary>
        private int _currentActorIndex = 0;

        /// <summary>
        /// 選択されたスキルまたはアイテムのIDです。
        /// </summary>
        private int _selectedItemId = -1;

        /// <summary>
        /// 選択されたターゲットのIDです。
        /// </summary>
        private int _selectedTargetId = -1;

        /// <summary>
        /// 選択されたターゲットが味方かどうかです。
        /// </summary>
        private bool _isTargetFriend = false;

        // 戦闘終了処理が重複しないようにするためのフラグを追加
        private bool _isFinishingBattle = false;

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
            _enemyStatusManager.SetUpEnemyStatus(enemyId);
            SimpleLogger.Instance.Log($"敵キャラクターのステータスをセットしました。ID: {string.Join(", ", enemyId)}");
        }

        private void Awake()
        {
            // このゲームオブジェクトにアタッチされているAudioSourceを取得
            _bgmAudioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// 戦闘の開始処理を行います。
        /// </summary>
        public void StartBattle()
        {
            _isFinishingBattle = false;

            SimpleLogger.Instance.Log("戦闘を開始します。");
            GameStateManager.ChangeToBattle();
            SetBattlePhase(BattlePhase.ShowEnemy);

            EnemyDataManager.enemies = this.EnemyId;

            TurnCount = 1;
            IsBattleFinished = false;

            _battleWindowManager.SetUpWindowControllers(this);
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.HidePager();

            _battleActionProcessor.InitializeProcessor(this);
            _battleActionRegister.InitializeRegister(_battleActionProcessor);
            _enemyCommandSelector.SetReferences(this, _battleActionProcessor);
            _battleResultManager.SetReferences(this);
            //_characterMoverManager.StopCharacterMover();
            _battleStarter.StartBattle(this);

            // 緊急バッテリーの使用回数を初期化 
            _emergencyBatteryUsesLeft = MAX_EMERGENCY_BATTERY_USES;

            // 温度をNORMALに初期化
            _currentTemperature = TemperatureState.NORMAL;
            // 温度UIを初期化して表示
            var tempGauge = _battleWindowManager.GetTemperatureGaugeController();
            if (tempGauge != null)
            {
                tempGauge.UpdateTemperatureUI(_currentTemperature);
                tempGauge.ShowWindow();
            }

            if (_battleBgm != null && _bgmAudioSource != null)
            {
                _bgmAudioSource.clip = _battleBgm;
                _bgmAudioSource.loop = true; // ループ再生を有効に
                _bgmAudioSource.Play();
            }
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
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照を取得します。
        /// </summary>
        public EnemyStatusManager GetEnemyStatusManager()
        {
            return _enemyStatusManager;
        }

        /// <summary>
        /// コマンド入力を開始します。
        /// </summary>
        public void StartInputCommandPhase()
        {
            SimpleLogger.Instance.Log($"コマンド入力のフェーズを開始します。現在のターン数: {TurnCount}");
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.HideWindow();
            BattlePhase = BattlePhase.InputCommand_Main;
            _battleActionProcessor.InitializeActions();

            var commandWindow = GetWindowManager().GetCommandWindowController();
            commandWindow.InitializeCommand();
            commandWindow.ResetSelection();
            commandWindow.ShowWindow();

            // ★ ターン開始時に防御状態を解除
            _enemyStatusManager.ResetAllGuardingStates();
            CharacterStatusManager.ResetAllGuardingStates();

            // ★ 最初のキャラクターからコマンド入力を開始
            _currentActorIndex = 0;
            StartNextCharacterCommand();
        }

        /// <summary>
        /// 次のキャラクターのコマンド入力を開始、または全員の入力が完了した場合は敵の行動選択に移ります。
        /// </summary>
        private void StartNextCharacterCommand()
        {
            var statusWindows = GetWindowManager().GetStatusWindowController();
            var windowManager = GetWindowManager();

            // 全ての関連ウィンドウを非表示にする
            windowManager.GetAttackCommandWindowController().HideWindow();
            windowManager.GetSelectionWindowController().HideWindow();
            windowManager.GetDescriptionWindowController().HideWindow();
            windowManager.GetSelectionEnemyWindowController().HideWindow();
            windowManager.GetSelectionPartyWindowController().HideWindow();
            windowManager.GetConfirmationWindowController().HideWindow();

            SetBattlePhase(BattlePhase.InputCommand_Main);
            SimpleLogger.Instance.Log($"StartNextCharacterCommand: フェーズを{BattlePhase}に設定, アクターインデックス: {_currentActorIndex}");

            // 1. まず、すべてのウィンドウのハイライトを解除します
            foreach (var window in statusWindows)
            {
                if (window != null) // 安全チェック
                {
                    window.SetHighlight(false);
                }
            }

            // 2. 全員のコマンド入力が完了した場合の処理
            if (_currentActorIndex >= CharacterStatusManager.partyCharacter.Count)
            {
                SimpleLogger.Instance.Log("味方全員のコマンド入力が完了しました。");
                if (_currentActorNameText != null) _currentActorNameText.gameObject.SetActive(false);
                //windowManager.GetCommandWindowController().HideWindow();
                PostCommandSelect();
                return;
            }

            // 戦闘不能キャラクターのスキップ処理
            int currentActorId = CharacterStatusManager.partyCharacter[_currentActorIndex];
            var currentActorStatus = CharacterStatusManager.GetCharacterStatusById(currentActorId);
            if (currentActorStatus.currentHp <= 0)
            {
                SimpleLogger.Instance.Log($"{CharacterDataManager.GetCharacterName(currentActorId)} は行動不能のため、ターンをスキップします。");
                _currentActorIndex++;
                StartNextCharacterCommand();
                return;
            }

            // 3. 現在のキャラクターの情報を取得してUIを更新
            if (_currentActorIndex < statusWindows.Count && statusWindows[_currentActorIndex] != null)
            {
                statusWindows[_currentActorIndex].SetHighlight(true);

                if (_currentActorNameText != null)
                {
                    string actorName = CharacterDataManager.GetCharacterName(currentActorId);
                    _currentActorNameText.text = actorName;
                    _currentActorNameText.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 現在のキャラクターのアクションを登録し、次のキャラクターのコマンド入力へ進めます。
        /// </summary>
        private void RegisterCurrentAction()
        {
            // 現在行動を選択しているキャラクターのIDを取得
            int actorId = CharacterStatusManager.partyCharacter[_currentActorIndex];
            SimpleLogger.Instance.Log($"アクションを登録します。Actor: {actorId}, Command: {SelectedCommand}, Target: {_selectedTargetId}");
            switch (SelectedCommand)
            {
                case BattleCommand.Attack:
                    if (SelectedAttackCommand == AttackCommand.Normal)
                    {
                        _battleActionRegister.SetFriendAttackAction(actorId, _selectedTargetId);
                    }
                    else if (SelectedAttackCommand == AttackCommand.Skill)
                    {
                        _battleActionRegister.SetFriendSkillAction(actorId, _selectedTargetId, _selectedItemId);
                    }
                    break;
                case BattleCommand.Item:
                    _battleActionRegister.SetFriendItemAction(actorId, _selectedTargetId, _selectedItemId);
                    break;
                case BattleCommand.Guard:
                    _battleActionRegister.SetFriendGuardAction(actorId);
                    break;
                case BattleCommand.Escape:
                    // 逃走はアクションを登録しないので何もしない
                    break;
            }

            // 次のキャラクターへ
            _currentActorIndex++;
            StartNextCharacterCommand();
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

            //windowManager.GetCommandWindowController().HideWindow();

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
            windowManager.GetAttackCommandWindowController().HideWindow();

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
            var windowManager = GetWindowManager();

            // メインコマンドウィンドウを確実に非表示にする
            //windowManager.GetCommandWindowController().HideWindow();

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
            // 選択されたIDを一時保存
            _selectedItemId = itemId;

            // スキルかアイテムかで処理を分ける
            if (SelectedCommand == BattleCommand.Attack && SelectedAttackCommand == AttackCommand.Skill)
            {
                // スキルの場合
                var skillData = SkillDataManager.GetSkillDataById(itemId);
                if (skillData == null) return;

                // スキル効果リストが存在し、かつ中身が1つ以上あることを確認
                if (skillData.skillEffect != null && skillData.skillEffect.Count > 0)
                {
                    // リストの最初の効果を代表としてターゲットタイプを取得
                    EffectTarget targetType = skillData.skillEffect[0].effectTarget;
                    SimpleLogger.Instance.Log($"スキル選択: {skillData.skillName} (ターゲット: {targetType})");
                    DetermineNextPhaseByTarget(targetType);
                }
                else
                {
                    Debug.LogError($"スキル「{skillData.skillName}」に効果が設定されていません。");
                }
            }
            else if (SelectedCommand == BattleCommand.Item)
            {
                // アイテムの場合
                var itemData = ItemDataManager.GetItemDataById(itemId);
                if (itemData == null) return;

                // ★ 修正点: itemEffectsリストの最初の要素からターゲットタイプを判定
                if (itemData.itemEffects != null && itemData.itemEffects.Count > 0)
                {
                    EffectTarget targetType = itemData.itemEffects[0].effectTarget;
                    SimpleLogger.Instance.Log($"アイテム選択: {itemData.itemName} (ターゲット: {targetType})");
                    DetermineNextPhaseByTarget(targetType);
                }
                else
                {
                    Debug.LogError($"アイテム「{itemData.itemName}」に効果が設定されていません。");
                }
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

                case EffectTarget.Own:
                    // 自分自身がターゲットなら、ターゲット情報を即座に設定してアクション登録へ
                    Debug.Log("自分自身をターゲットにします。");
                    GetWindowManager().GetDescriptionWindowController().HideWindow();

                    // 現在行動中のキャラクターを自分自身として設定
                    _selectedTargetId = GetCurrentActorCharacterId();
                    _isTargetFriend = true;
                    RegisterCurrentAction(); // アクションを登録して次のキャラクターの入力へ
                    break;

                case EffectTarget.EnemyAll:
                case EffectTarget.FriendAll:
                    // 全体効果の場合もターゲット選択は不要
                    Debug.Log("全体をターゲットにします。");
                    GetWindowManager().GetDescriptionWindowController().HideWindow();

                    // targetIdは-1（全体を示す）などの扱いでアクション登録
                    _selectedTargetId = -1;
                    RegisterCurrentAction();
                    break;

                default:
                    Debug.Log("ターゲット選択不要。行動を実行します。");
                    SetBattlePhase(BattlePhase.Action);
                    GetWindowManager().GetDescriptionWindowController().HideWindow();
                    break;
            }
        }

        /// <summary>
        /// ターゲットの選択後
        /// </summary>
        public void OnTargetSelected(EnemyStatus target)
        {
            Debug.Log($"{target.enemyData.enemyName} (BattleID: {target.enemyBattleId}) がターゲットに選択されました。");
            GetWindowManager().GetDescriptionWindowController().HideWindow();
            GetWindowManager().GetSelectionEnemyWindowController().HideWindow();

            // ★★★ 変更点: 一意な enemyBattleId を使用する
            _selectedTargetId = target.enemyBattleId;
            _isTargetFriend = false;
            RegisterCurrentAction();
        }

        /// <summary>
        /// パーティーメンバーが選択された時のコールバックです。
        /// </summary>
        public void OnPartySelected(CharacterStatus target)
        {
            Debug.Log($"{CharacterDataManager.GetCharacterName(target.characterId)} がターゲットに選択されました。");
            GetWindowManager().GetDescriptionWindowController().HideWindow();
            GetWindowManager().GetSelectionPartyWindowController().HideWindow();

            // ターゲット情報を保存してアクションを登録
            _selectedTargetId = target.characterId;
            _isTargetFriend = true;
            RegisterCurrentAction();
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
                    windowManager.GetDescriptionWindowController().HideWindow();
                    windowManager.GetCommandWindowController().ShowWindow();
                    // メインコマンド選択に戻る
                    SetBattlePhase(BattlePhase.InputCommand_Main);
                    break;

                // スキルリスト選択中にキャンセル
                case BattlePhase.SelectSkill:
                    Debug.Log("スキルコマンド選択のキャンセル処理が実行されました！");
                    windowManager.GetSelectionWindowController().HideWindow();
                    windowManager.GetDescriptionWindowController().HideWindow();
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
                        // 「通常攻撃/スキル」選択に戻る
                        windowManager.GetAttackCommandWindowController().ShowWindow();
                        SetBattlePhase(BattlePhase.InputCommand_Attack);
                    }
                    else if (SelectedAttackCommand == AttackCommand.Skill)
                    {
                        Debug.Log("スキルのキャンセル");
                        // スキルのリスト選択に戻る
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

            // どのコマンドが選択されていたかに応じて処理を分岐
            switch (SelectedCommand)
            {
                case BattleCommand.Guard:
                    Debug.Log("「ぼうぎょ」の準備をします。");
                    // 「ぼうぎょ」は通常の行動なので、アクションを登録して次のキャラへ
                    RegisterCurrentAction();
                    break;

                case BattleCommand.Escape:
                    Debug.Log("「にげる」を試みます！");
                    // 「にげる」は特殊処理。アクション登録はせず、専用の逃走処理を開始
                    StartCoroutine(ProcessEscapeCoroutine());
                    break;
            }

            // プレイヤーの行動をセットしたので、行動実行フェーズに進む
            //SetBattlePhase(BattlePhase.Action);
        }

        /// <summary>
        /// 逃走処理の一連の流れを実行するコルーチンです。
        /// </summary>
        private IEnumerator ProcessEscapeCoroutine()
        {
            // 1. コマンド入力中のUIを非表示にする
            //GetWindowManager().GetCommandWindowController().HideWindow();
            if (_currentActorNameText != null) _currentActorNameText.gameObject.SetActive(false);

            // 全てのステータスウィンドウのハイライトを解除
            foreach (var window in GetWindowManager().GetStatusWindowController())
            {
                if (window != null) window.SetHighlight(false);
            }

            // 2. メッセージウィンドウを表示する
            var messageWindow = GetWindowManager().GetMessageWindowController();
            messageWindow.ShowWindow();

            // 3.「パーティは逃げ出した！」メッセージの表示が終わるのを待つ
            //    （GeneratePartyEscapeMessageは、後でMessageWindowControllerに作成します）
            yield return messageWindow.StartCoroutine(
                messageWindow.GeneratePartyEscapeMessage()
            );

            // 4. BattleManagerに戦闘終了を通知する
            OnEscapeaway();
        }

        /// <summary>
        /// 確認ウィンドウで「いいえ」またはキャンセルが選択されたときの処理です。
        /// </summary>
        public void OnConfirmationCanceled()
        {
            // 1. 「はい／いいえ」のウィンドウを非表示にする
            GetWindowManager().GetConfirmationWindowController().HideWindow();

            // 3. ゲームの状態を「メインコマンド入力待ち」に戻す
            SetBattlePhase(BattlePhase.InputCommand_Main);
            // 3. メインコマンドウィンドウを表示
            var commandWindow = GetWindowManager().GetCommandWindowController();
            commandWindow.InitializeCommand();
            commandWindow.ShowWindow();
            SimpleLogger.Instance.Log($"確認キャンセル: メインコマンドウィンドウを表示, フェーズ: {BattlePhase}");
        }

        /// <summary>
        /// コマンド選択が完了した後の処理です。
        /// </summary>
        void PostCommandSelect()
        {
            SimpleLogger.Instance.Log("敵のコマンド入力を行います。");
            _enemyCommandSelector.SelectEnemyCommand();
        }

        /// <summary>
        /// 敵キャラクターのコマンドが選択された時のコールバックです。
        /// </summary>
        public void OnEnemyCommandSelected()
        {
            SimpleLogger.Instance.Log("敵味方の行動が決まったので実際に行動させます。");

            SetBattlePhase(BattlePhase.Action);
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.ShowWindow();
            _battleActionProcessor.SetPriorities();
            _battleActionProcessor.StartActions();
        }

        /// <summary>
        /// ステータスの値が更新された時のコールバックです。
        /// </summary>
        public void OnUpdateStatus()
        {
            var statusWindows = _battleWindowManager.GetStatusWindowController();
            int partyCount = CharacterStatusManager.partyCharacter.Count;

            // ウィンドウの数がパーティメンバーの数より少ない場合のみエラーを出す
            if (statusWindows.Count < partyCount)
            {
                Debug.LogError("パーティメンバーの数に対してステータスウィンドウが不足しています。");
                return;
            }

            // パーティメンバーの数だけループして、対応するウィンドウの表示を更新する
            for (int i = 0; i < partyCount; i++)
            {
                var windowController = statusWindows[i];
                var characterId = CharacterStatusManager.partyCharacter[i];
                var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);

                // ウィンドウとステータスを渡して更新を命令
                if (windowController != null && characterStatus != null)
                {
                    windowController.UpdateStatus(characterStatus);
                }
            }
        }

        /// <summary>
        /// 敵を全て倒した時のコールバックです。
        /// </summary>
        public void OnEnemyDefeated()
        {
            SimpleLogger.Instance.Log("敵を全て倒しました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
            _battleResultManager.OnWin();
        }

        /// <summary>
        /// ゲームオーバーになった時のコールバックです。
        /// </summary>
        public void OnGameover()
        {
            SimpleLogger.Instance.Log("ゲームオーバーになりました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
            _battleResultManager.OnLose();
        }

        /// <summary>
        /// 味方が逃走に成功した時のコールバックです。
        /// </summary>
        public void OnEscapeaway()
        {
            SimpleLogger.Instance.Log("逃走に成功しました。");
            IsBattleFinished = true;
            if (_bgmAudioSource != null) _bgmAudioSource.Stop();
            OnFinishBattle();
        }

        /// <summary>
        /// 敵が逃走に成功した時のコールバックです。
        /// </summary>
        public void OnEnemyEscapeaway()
        {
            SimpleLogger.Instance.Log("敵が逃走に成功しました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
            _battleResultManager.OnWin();
        }

        /// <summary>
        /// 戦闘を終了する時のコールバックです。
        /// </summary>
        public void OnFinishBattle()
        {
            // すでに終了処理が始まっている場合は、ここで処理を中断
            if (_isFinishingBattle) return;
            _isFinishingBattle = true;

            SimpleLogger.Instance.Log("戦闘に勝利して終了します。");

            ClearAllCharacterStatuses();

            _enemyStatusManager.InitializeEnemyStatusList();
            _battleActionProcessor.InitializeActions();
            _battleActionProcessor.StopActions();

            //_characterMoverManager.ResumeCharacterMover();
            BattlePhase = BattlePhase.NotInBattle;
            if (_bgmAudioSource != null) _bgmAudioSource.Stop();

            SceneManager.LoadScene("SampleScene");
        }

        /// <summary>
        /// 戦闘を終了する時のコールバックです。
        /// </summary>
        public void OnFinishBattleWithGameover()
        {
            if (_isFinishingBattle) return;
            _isFinishingBattle = true;

            SimpleLogger.Instance.Log("ゲームオーバーとして戦闘を終了します。");

            ClearAllCharacterStatuses();

            _enemyStatusManager.InitializeEnemyStatusList();
            _battleActionProcessor.InitializeActions();
            _battleActionProcessor.StopActions();

            //_characterMoverManager.ResumeCharacterMover();
            BattlePhase = BattlePhase.NotInBattle;

            if (_bgmAudioSource != null) _bgmAudioSource.Stop();

            SceneManager.LoadScene("SampleScene");
        }

        /// <summary>
        /// メッセージウィンドウでメッセージの表示が完了した時のコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            switch (BattlePhase)
            {
                case BattlePhase.ShowEnemy:
                    SimpleLogger.Instance.Log("敵の表示が完了しました。");
                    StartInputCommandPhase();
                    break;
                case BattlePhase.Action:
                    _battleActionProcessor.ShowNextMessage();
                    break;
                    // ★ Resultフェーズでの呼び出しは不要になったので、caseごと削除します。
                    // case BattlePhase.Result:
                    //     _battleResultManager.ShowNextMessage();
                    //     break;
            }
        }

        /// <summary>
        /// ターン内の行動が完了した時のコールバックです。
        /// </summary>
        public void OnFinishedActions()
        {
            if (IsBattleFinished)
            {
                SimpleLogger.Instance.Log("OnFinishedActions() || 戦闘が終了しているため、処理を中断します。");
                return;
            }
            StartCoroutine(OnFinishedActionsCoroutine());
        }

        /// <summary>
        /// ターン終了時の処理をまとめたコルーチン
        /// </summary>
        private IEnumerator OnFinishedActionsCoroutine()
        {
            ProcessBuffDurations();

            // 特殊状態の処理が終わるのを待つ
            yield return StartCoroutine(ProcessSpecialStatusDurations());

            yield return StartCoroutine(UpdateTemperature());

            SimpleLogger.Instance.Log("ターン内の行動が完了しました。");
            TurnCount++;
            StartInputCommandPhase();
        }

        /// ターン終了時に全てのバフ・デバフの持続ターンを処理します。
        private void ProcessBuffDurations()
        {
            // 味方キャラクターのバフを処理
            var partyStatuses = CharacterStatusManager.GetPartyMemberStatuses();
            foreach (var status in partyStatuses)
            {
                // リストから削除してもループが壊れないように逆順で処理
                for (int i = status.buffs.Count - 1; i >= 0; i--)
                {
                    var buff = status.buffs[i];
                    buff.duration--;
                    if (buff.duration <= 0)
                    {
                        // TODO: 効果が切れたメッセージを表示する処理
                        status.buffs.RemoveAt(i);
                    }
                }
            }

            // 敵キャラクターのバフを処理
            var enemyStatuses = _enemyStatusManager.GetEnemyStatusList();
            foreach (var status in enemyStatuses)
            {
                for (int i = status.buffs.Count - 1; i >= 0; i--)
                {
                    var buff = status.buffs[i];
                    buff.duration--;
                    if (buff.duration <= 0)
                    {
                        status.buffs.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 敵の表示（スプライトとステータスUI）をまとめて更新します。
        /// このメソッドはダメージ計算後など、敵の状態が変化した際に呼び出してください。
        /// </summary>
        public void UpdateEnemyVisuals()
        {
            if (_enemyStatusUIManager != null && _battleSpriteController != null)
            {
                var enemies = _enemyStatusManager.GetEnemyStatusList();
                _enemyStatusUIManager.UpdateAllEnemyStatuses(enemies);
                _battleSpriteController.UpdateAllEnemySprites(enemies);
            }
        }

        /// <summary>
        /// 敵のステータスUI全体を更新します。
        /// </summary>
        public void UpdateEnemyStatusUI()
        {
            if (_enemyStatusUIManager != null)
            {
                _enemyStatusUIManager.UpdateAllEnemyStatuses(_enemyStatusManager.GetEnemyStatusList());
            }
        }

        /// <summary>
        /// 敵ステータスUIの管理クラスへの参照を取得します。
        /// </summary>
        public EnemyStatusUIManager GetEnemyStatusUIManager()
        {
            return _enemyStatusUIManager;
        }

        /// <summary>
        /// 現在コマンドを入力しているキャラクターのIDを取得します。
        /// </summary>
        /// <returns>現在のキャラクターID。該当しない場合は-1を返します。</returns>
        public int GetCurrentActorCharacterId()
        {
            if (_currentActorIndex >= 0 && _currentActorIndex < CharacterStatusManager.partyCharacter.Count)
            {
                return CharacterStatusManager.partyCharacter[_currentActorIndex];
            }
            Debug.LogError("有効なアクターインデックスではありません。");
            return -1;
        }

        /// <summary>
        /// ターン終了時に全ての特殊状態の持続ターンや効果を処理します。
        /// </summary>
        private IEnumerator ProcessSpecialStatusDurations()
        {
            var messageWindow = _battleWindowManager.GetMessageWindowController();

            // --- 味方キャラクターの特殊状態を処理 ---
            // ループ中にリストが変更される可能性に備え、ToList()でコピーを作成して処理
            foreach (var status in CharacterStatusManager.GetPartyMemberStatuses().ToList())
            {
                if (status.isDefeated || status.currentStatus == SpecialStatusType.None) continue;

                bool statusRecovered = false;
                string characterName = CharacterDataManager.GetCharacterName(status.characterId);

                // 過充電のHP減少効果
                if (status.currentStatus == SpecialStatusType.Overcharge)
                {
                    var parameterRecord = CharacterDataManager.GetParameterTable(status.characterId).parameterRecords.Find(p => p.level == status.level);
                    int damage = Mathf.Max(1, (int)(parameterRecord.hp * 0.05f));
                    CharacterStatusManager.ChangeCharacterStatus(status.characterId, -damage, 0);
                    yield return StartCoroutine(messageWindow.GenerateOverchargeDamageMessage(characterName, damage));

                    // ダメージで倒れた場合は以降の処理をスキップ
                    if (CharacterStatusManager.IsCharacterDefeated(status.characterId))
                    {
                        OnUpdateStatus(); // UI更新
                        if (CharacterStatusManager.IsAllCharacterDefeated()) OnGameover();
                        continue;
                    }
                }

                // スタン状態の処理
                if (status.currentStatus == SpecialStatusType.Stun)
                {
                    status.statusDuration--;
                    if (status.statusDuration <= 0)
                    {
                        if (status.currentBt <= 0)
                        {
                            if (_emergencyBatteryUsesLeft > 0)
                            {
                                _emergencyBatteryUsesLeft--;
                                var parameterRecord = CharacterDataManager.GetParameterTable(status.characterId).parameterRecords.Find(p => p.level == status.level);

                                // ★修正点: キャラクター本来の最大BT値を回復量に設定
                                CharacterStatusManager.ChangeCharacterStatus(status.characterId, 0, parameterRecord.bt);

                                status.maxBtPenalty = 0;
                                statusRecovered = true;
                                yield return StartCoroutine(messageWindow.GenerateEmergencyBatteryMessage(characterName));
                            }
                        }
                        else { statusRecovered = true; }
                    }
                }
                else { status.statusDuration--; }

                // ターン経過で状態が回復した場合
                if (status.statusDuration <= 0 && !statusRecovered)
                {
                    yield return StartCoroutine(messageWindow.GenerateStatusRecoveryMessage(characterName));
                    status.currentStatus = SpecialStatusType.None;
                }
                else if (statusRecovered)
                {
                    status.currentStatus = SpecialStatusType.None;
                }
            }
            OnUpdateStatus(); // 味方全体のUIを更新

            // --- 敵キャラクターの特殊状態を処理 ---
            foreach (var status in _enemyStatusManager.GetEnemyStatusList().ToList())
            {
                if (status.isDefeated || status.isRunaway || status.currentStatus == SpecialStatusType.None) continue;

                bool statusRecovered = false;
                string characterName = status.enemyData.enemyName;

                if (status.currentStatus == SpecialStatusType.Overcharge)
                {
                    int damage = Mathf.Max(1, (int)(status.enemyData.hp * 0.05f));
                    _enemyStatusManager.ChangeEnemyStatus(status.enemyBattleId, -damage, 0);
                    yield return StartCoroutine(messageWindow.GenerateOverchargeDamageMessage(characterName, damage));
                    if (_enemyStatusManager.IsEnemyDefeated(status.enemyBattleId))
                    {
                        _enemyStatusManager.OnDefeatEnemy(status.enemyBattleId);
                        UpdateEnemyVisuals(); // UI更新
                        if (_enemyStatusManager.IsAllEnemyDefeated()) OnEnemyDefeated();
                        continue;
                    }
                }

                if (status.currentStatus == SpecialStatusType.Stun)
                {
                    status.statusDuration--;
                    if (status.statusDuration <= 0)
                    {
                        if (status.currentBt <= 0)
                        {
                            // ★修正点: 敵キャラクター本来の最大BT値を回復量に設定
                            _enemyStatusManager.ChangeEnemyStatus(status.enemyBattleId, 0, status.enemyData.bt);

                            status.maxBtPenalty = 0;
                            statusRecovered = true;
                            yield return StartCoroutine(messageWindow.GenerateEmergencyBatteryMessage(characterName));
                        }
                        else { statusRecovered = true; }
                    }
                }
                else { status.statusDuration--; }

                if (status.statusDuration <= 0 && !statusRecovered)
                {
                    yield return StartCoroutine(messageWindow.GenerateStatusRecoveryMessage(characterName));
                    status.currentStatus = SpecialStatusType.None;
                }
                else if (statusRecovered)
                {
                    status.currentStatus = SpecialStatusType.None;
                }
            }
            UpdateEnemyVisuals(); // 敵全体のUIを更新
        }

        /// <summary>
            /// 全てのキャラクターの特殊状態をリセットします。
            /// </summary>
        private void ClearAllCharacterStatuses()
        {
            // リストがnullでないことを確認するガード処理
            if (CharacterStatusManager.characterStatuses == null) return;

            // 味方
            foreach (var status in CharacterStatusManager.characterStatuses)
            {
                if (status == null) continue;

                status.currentStatus = SpecialStatusType.None;
                status.statusDuration = 0;
                status.maxBtPenalty = 0;
                // 過充電で最大BTを超えていた場合、元に戻す
                var parameterRecord = CharacterDataManager.GetParameterTable(status.characterId).parameterRecords.Find(p => p.level == status.level);
                if (parameterRecord != null && status.currentBt > parameterRecord.bt)
                {
                    status.currentBt = parameterRecord.bt;
                }
            }
            // 敵
            foreach (var status in _enemyStatusManager.GetEnemyStatusList())
            {
                status.currentStatus = SpecialStatusType.None;
                status.statusDuration = 0;
                status.maxBtPenalty = 0;
                if (status.currentBt > status.enemyData.bt)
                {
                    status.currentBt = status.enemyData.bt;
                }
            }
        }

        /// <summary>
        /// 戦闘結果マネージャーへの参照を取得します。
        /// </summary>
        public BattleResultManager GetResultManager()
        {
            return _battleResultManager;
        }

        /// <summary>
        /// ターン終了時に温度を確率で変化させます。
        /// </summary>
        private IEnumerator UpdateTemperature()
        {
            // 例：30%の確率で温度が変化
            if (UnityEngine.Random.Range(0, 100) < 20)
            {
                var messageWindow = _battleWindowManager.GetMessageWindowController();
                TemperatureState newState;
                string message = "";

                // 現在の温度に応じて次の状態を決定
                switch (_currentTemperature)
                {
                    case TemperatureState.NORMAL:
                        // NORMALからはHOTかCOLDに変化
                        newState = (UnityEngine.Random.value > 0.5f) ? TemperatureState.HOT : TemperatureState.COLD;
                        break;
                    case TemperatureState.HOT:
                        // HOTからはNORMALに変化
                        newState = TemperatureState.NORMAL;
                        break;
                    case TemperatureState.COLD:
                        // COLDからはNORMALに変化
                        newState = TemperatureState.NORMAL;
                        break;
                    default:
                        newState = TemperatureState.NORMAL;
                        break;
                }

                int randomNumber = UnityEngine.Random.Range(1, 4);

                if(randomNumber==1)
                {
                    if (newState == TemperatureState.HOT) message = "敵ロボットからの排熱で気温が上がった！";
                    else if (newState == TemperatureState.COLD) message = "敵ロボットの攻撃で空調が壊れて気温が下がった…";
                    else message = "異常を感知した警備ロボットが温度をもとに戻した。";
                }
                else if(randomNumber==2)
                {
                    if (newState == TemperatureState.HOT) message = "敵ロボットの攻撃で近くの機械が炎上した！";
                    else if (newState == TemperatureState.COLD) message = "敵ロボットの攻撃で近くにあった自販機が壊れて冷気が漏れ出した…";
                    else message = "異常を感知した警備ロボットが温度をもとに戻した。";
                }
                else
                {
                    if (newState == TemperatureState.HOT) message = "敵ロボットの攻撃で空調が壊れて気温が上がった！";
                    else if (newState == TemperatureState.COLD) message = "敵ロボットが冷気を吹き出し気温が下がった…";
                    else message = "異常を感知した警備ロボットが温度をもとに戻した。";
                }
                

                // 状態を更新してメッセージ表示
                ChangeTemperature(newState);
                yield return StartCoroutine(messageWindow.GenerateMultiLineMessage(message));
            }
        }

        /// <summary>
        /// 温度状態を変更し、関連する処理を呼び出します。
        /// </summary>
        /// <param name="newState">新しい温度状態</param>
        public void ChangeTemperature(TemperatureState newState)
        {
            _currentTemperature = newState;
            SimpleLogger.Instance.Log($"温度が {newState} に変化しました。");

            // UIを更新
            var tempGauge = _battleWindowManager.GetTemperatureGaugeController();
            if (tempGauge != null)
            {
                tempGauge.UpdateTemperatureUI(newState);
            }

            // COLD状態の特殊効果を適用
            if (newState == TemperatureState.COLD)
            {
                // 全キャラクターの最大BTを減少させる
                CharacterStatusManager.ApplyColdPenaltyToAll(0.1f); // 10%減少
                _enemyStatusManager.ApplyColdPenaltyToAll(0.1f);
                OnUpdateStatus(); // 味方UI更新
                UpdateEnemyStatusUI(); // 敵UI更新
            }
            // 注意：要件通り、HOTやNORMALに戻ってもペナルティは解除しません。
        }
    }
}