using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のアクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessor : MonoBehaviour
    {
        /// <summary>
        /// 戦闘中のアクション間の実行間隔です。
        /// </summary>
        [SerializeField]
        float _actionInterval = 0.5f;

        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照です。
        /// </summary>
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// ターン内のアクションのリストです。
        /// </summary>
        List<BattleAction> _actions = new();

        /// <summary>
        /// アクションを処理するコルーチンへの参照です。
        /// </summary>
        Coroutine _processActionCoroutine;

        /// <summary>
        /// 戦闘中の攻撃アクションを処理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionProcessorAttack _battleActionProcessorAttack;

        /// <summary>
        /// 戦闘中の魔法アクションを処理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionProcessorSkill _battleActionProcessorSkill;

        [SerializeField]
        private BattleActionProcessorGuard _battleActionProcessorGuard;

        private MessageWindowController _messageWindowController;

        /// <summary>
        /// 戦闘中のアイテムアクションを処理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionProcessorItem _battleActionProcessorItem;

        /// <summary>
        /// 戦闘中の逃げるアクションを処理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionProcessorEscape _battleActionProcessorEscape;

        /// <summary>
        /// プロセスを一時停止するかどうかのフラグです。
        /// </summary>
        public bool IsPausedProcess { get; private set; }

        /// <summary>
        /// メッセージをポーズするかどうかのフラグです。
        /// </summary>
        public bool IsPausedMessage { get; private set; }

        /// <summary>
        /// このクラスを初期化します。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラスへの参照</param>
        public void InitializeProcessor(BattleManager battleManager)
        {
            _battleManager = battleManager;
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();

            _battleActionProcessorAttack.SetReferences(_battleManager, this);
            _battleActionProcessorSkill.SetReferences(_battleManager, this);
            _battleActionProcessorGuard.SetReferences(battleManager, this);
            _battleActionProcessorItem.SetReferences(_battleManager, this);
            _battleActionProcessorEscape.SetReferences(_battleManager, this);
        }

        /// <summary>
        /// ターン内のアクションのリストを初期化します。
        /// </summary>
        public void InitializeActions()
        {
            _actions.Clear();
        }

        /// <summary>
        /// ターン内のアクションのリストに要素を追加します。
        /// </summary>
        /// <param name="action">追加するアクション</param>
        public void RegisterAction(BattleAction action)
        {
            _actions.Add(action);
        }

        /// <summary>
        /// アクションリストの要素の優先度を設定します。
        /// </summary>
        public void SetPriorities()
        {
            // アクションコマンドの素早さの値に20%の乱数を乗じます。
            foreach (var action in _actions)
            {
                // 防御の場合は素早さ計算をスキップします
                if (action.battleCommand == BattleCommand.Guard) continue;
                action.actorSpeed = (int)(action.actorSpeed * (1 + Random.Range(-0.2f, 0.2f)));
            }

            // 防御、逃走、その他のアクションにリストを分割します
            var guardActions = _actions.Where(a => a.battleCommand == BattleCommand.Guard).ToList();
            var escapeActions = _actions.Where(a => a.battleCommand == BattleCommand.Escape).ToList();
            var otherActions = _actions.Where(a => a.battleCommand != BattleCommand.Guard && a.battleCommand != BattleCommand.Escape).ToList();

            // 優先度を割り振るためのカウンター
            int priority = 1;

            // 1. 通常アクションの優先度を設定 (素早さが遅い順に低い優先度を割り当てる)
            foreach (var action in otherActions.OrderBy(a => a.actorSpeed))
            {
                action.priority = priority;
                priority++;
            }

            // 2. 逃走アクションに高い優先度を設定 (通常アクションよりは必ず先)
            int escapePriority = 1000;
            foreach (var action in escapeActions)
            {
                action.priority = escapePriority;
            }

            // 3. 防御アクションに最高の優先度を設定 (他のどの行動よりも必ず先)
            int guardPriority = 9999;
            foreach (var action in guardActions)
            {
                action.priority = guardPriority;
            }
        }

        /// <summary>
        /// アクションリストの内容を優先度に応じて処理していきます。
        /// </summary>
        public void StartActions()
        {
            OutputActionOrder();
            IsPausedMessage = false;
            IsPausedProcess = false;
            _processActionCoroutine = StartCoroutine(ProcessAction());
        }

        /// <summary>
        /// アクションリストの内容を優先度に応じて処理していきます。
        /// </summary>
        IEnumerator ProcessAction()
        {
            var query = _actions.OrderByDescending(a => a.priority).ToList();
            foreach (var action in query)
            {
                // --- [共通処理] 戦闘が終了していたら、全ての行動を中断 ---
                if (_battleManager.IsBattleFinished)
                {
                    SimpleLogger.Instance.Log("戦闘が終了しているため、アクション処理を中断します。");
                    yield break;
                }

                string actorName = GetCharacterName(action.actorId, action.isActorFriend);

                // --- [共通処理] 行動主が行動可能かチェック (生存/スタン) ---
                bool canAct = true;
                if (action.isActorFriend)
                {
                    var friendStatus = CharacterStatusManager.GetCharacterStatusById(action.actorId);
                    if (friendStatus == null || friendStatus.isDefeated)
                    {
                        canAct = false;
                        // 倒れている場合はメッセージ不要
                    }
                    else if (friendStatus.currentStatus == SpecialStatusType.Stun)
                    {
                        canAct = false;
                        yield return StartCoroutine(_messageWindowController.GenerateStunnedMessage(actorName));
                    }
                }
                else // 敵の場合
                {
                    var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(action.actorId);
                    if (enemyStatus == null || enemyStatus.isDefeated || enemyStatus.isRunaway)
                    {
                        canAct = false;
                    }
                    else if (enemyStatus.currentStatus == SpecialStatusType.Stun)
                    {
                        canAct = false;
                        yield return StartCoroutine(_messageWindowController.GenerateStunnedMessage(actorName));
                    }
                }

                if (!canAct)
                {
                    SimpleLogger.Instance.Log($"{actorName} は行動不能のため、アクションをスキップします。");
                    continue; // 次のアクションへ
                }

                // --- [共通処理] ターゲットが既に倒されている場合、別の生きている敵にターゲットを変更 ---
                if ((action.battleCommand == BattleCommand.Attack || action.battleCommand == BattleCommand.Item) && !action.isTargetFriend && action.targetId != -1)
                {
                    var targetStatus = _enemyStatusManager.GetEnemyStatusByBattleId(action.targetId);
                    if (targetStatus == null || targetStatus.isDefeated)
                    {
                        var livingEnemies = _enemyStatusManager.GetEnemyStatusList().Where(e => !e.isDefeated && !e.isRunaway).ToList();
                        if (livingEnemies.Count > 0)
                        {
                            action.targetId = livingEnemies[0].enemyBattleId;
                            SimpleLogger.Instance.Log($"ターゲットが倒されていたため、{GetCharacterName(action.targetId, false)} にターゲットを変更しました。");
                        }
                        else
                        {
                            SimpleLogger.Instance.Log("攻撃対象の敵がいないため、アクションをスキップします。");
                            continue;
                        }
                    }
                }

                // --- アクションの実行 ---
                SimpleLogger.Instance.Log($"コマンドに応じた行動を行います。 コマンド : {action.battleCommand}");
                switch (action.battleCommand)
                {
                    case BattleCommand.Attack:
                        if (action.itemId > 0) _battleActionProcessorSkill.ProcessAction(action);
                        else _battleActionProcessorAttack.ProcessAction(action);
                        break;
                    case BattleCommand.Guard:
                        _battleActionProcessorGuard.ProcessAction(action);
                        break;
                    case BattleCommand.Item:
                        _battleActionProcessorItem.ProcessAction(action);
                        break;
                    case BattleCommand.Escape:
                        _battleActionProcessorEscape.ProcessAction(action);
                        break;
                    case BattleCommand.Status:
                        break;
                }

                // スキルや攻撃のメッセージ表示などの処理が終わるのを待つ
                while (IsPausedProcess)
                {
                    yield return null;
                }

                // --- [共通処理] 各アクション終了後に、戦闘の勝利/敗北を必ずチェック ---
                if (_enemyStatusManager.IsAllEnemyDefeated())
                {
                    SimpleLogger.Instance.Log("敵が全滅しました。戦闘を終了します。");
                    _battleManager.OnEnemyDefeated();
                    yield break; // アクション処理のコルーチンを完全に停止
                }
                if (CharacterStatusManager.IsAllCharacterDefeated())
                {
                    SimpleLogger.Instance.Log("味方が全滅しました。ゲームオーバーです。");
                    _battleManager.OnGameover();
                    yield break; // アクション処理のコルーチンを完全に停止
                }

                yield return new WaitForSeconds(_actionInterval);
            }

            // 全てのアクションが終わった後
            _battleManager.OnFinishedActions();
        }

        /// <summary>
        /// アクションの処理を停止します。
        /// </summary>
        public void StopActions()
        {
            if (_processActionCoroutine != null)
            {
                StopCoroutine(_processActionCoroutine);
            }
        }

        /// <summary>
        /// キャラクターの戦闘用パラメータを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        /// <param name="isFriend">味方かどうか</param>
        public BattleParameter GetCharacterParameter(int characterId, bool isFriend)
        {
            BattleParameter battleParameter = new();

            if (isFriend)
            {
                // 味方の場合は既存の処理でベースパラメータと装備品を取得
                battleParameter = CharacterStatusManager.GetCharacterBattleParameterById(characterId);
                var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
                battleParameter.isGuarding = characterStatus.isGuarding;
            }
            else
            {
                // 敵の場合はベースパラメータを取得
                var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(characterId);
                var enemyData = enemyStatus.enemyData;
                int effectiveMaxBt = enemyData.bt - enemyStatus.maxBtPenalty;
                if (effectiveMaxBt < 1) effectiveMaxBt = 1;

                battleParameter.atk = enemyData.atk;
                battleParameter.def = enemyData.def;
                battleParameter.dex = enemyData.dex;
                battleParameter.isGuarding = enemyStatus.isGuarding;

                // 特殊状態とBT残量による補正
                switch (enemyStatus.currentStatus)
                {
                    case SpecialStatusType.Overheat:
                        battleParameter.atk = (int)(battleParameter.atk * 1.2f);
                        battleParameter.def = (int)(battleParameter.def * 0.8f);
                        break;
                    case SpecialStatusType.Overcharge:
                        battleParameter.atk = (int)(battleParameter.atk * 1.3f);
                        break;
                }
                float btRate = (float)enemyStatus.currentBt / effectiveMaxBt;
                if (btRate >= 0.9f) battleParameter.atk = (int)(battleParameter.atk * 1.05f);
                else if (btRate <= 0.1f) battleParameter.atk = (int)(battleParameter.atk * 0.95f);

                // 敵に適用中のバフの効果を合算する
                foreach (var buff in enemyStatus.buffs)
                {
                    switch (buff.parameter)
                    {
                        case SkillParameter.atk: battleParameter.atk += buff.value; break;
                        case SkillParameter.def: battleParameter.def += buff.value; break;
                        case SkillParameter.dex: battleParameter.dex += buff.value; break;
                    }
                }

                // パラメータが0未満にならないように調整
                if (battleParameter.atk < 0) battleParameter.atk = 0;
                if (battleParameter.def < 0) battleParameter.def = 0;
                if (battleParameter.dex < 0) battleParameter.dex = 0;
            }
            return battleParameter;
        }

        /// <summary>
        /// キャラクターの名前を取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        /// <param name="isFriend">味方かどうか</param>
        public string GetCharacterName(int characterId, bool isFriend)
        {
            string name;
            if (isFriend)
            {
                var characterData = CharacterDataManager.GetCharacterData(characterId);
                name = characterData.characterName;
            }
            else
            {
                var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(characterId);
                name = enemyStatus.enemyData.enemyName;
            }
            return name;
        }

        /// <summary>
        /// 次のメッセージを表示します。
        /// もしメッセージの表示か完了していたら、次のアクションを処理します。
        /// </summary>
        public void ShowNextMessage()
        {
            SetPauseMessage(false);
        }

        /// <summary>
        /// プロセスを一時停止するかどうかを設定します。
        /// Trueで一時停止します。
        /// </summary>
        /// <param name="puase">一時停止するかどうか</param>
        public void SetPauseProcess(bool puase)
        {
            IsPausedProcess = puase;
        }

        /// <summary>
        /// メッセージを一時停止するかどうかを設定します。
        /// Trueで一時停止します。
        /// </summary>
        /// <param name="puase">一時停止するかどうか</param>
        public void SetPauseMessage(bool puase)
        {
            IsPausedMessage = puase;
        }

        /// <summary>
        /// デバッグ用機能
        /// キャラクターの行動順をコンソールに出力します。
        /// </summary>
        void OutputActionOrder()
        {
            SimpleLogger.Instance.Log("行動順を出力します。");
            var query = _actions.OrderBy(a => a.priority);
            foreach (var action in query)
            {
                SimpleLogger.Instance.Log($"キャラクターID : {action.actorId}, 味方かどうか : {action.isActorFriend}, Priority : {action.priority}, Command : {action.battleCommand}");
            }
        }
    }
}