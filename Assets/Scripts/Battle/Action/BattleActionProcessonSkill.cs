using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のスキルアクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorSkill : MonoBehaviour, IBattleActionProcessor
    {
        // --- 変数やSetReferencesメソッドは変更ありません ---
        private BattleActionProcessor _actionProcessor;
        private BattleManager _battleManager;
        private MessageWindowController _messageWindowController;
        private EnemyStatusManager _enemyStatusManager;

        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        }

        /// <summary>
        /// スキルのアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            var skillData = SkillDataManager.GetSkillDataById(action.itemId);
            if (skillData == null)
            {
                Debug.LogError($"スキルデータが見つかりません。ID: {action.itemId}");
                return;
            }

            // --- 消費BTの処理は変更ありません ---
            int hpDelta = 0;
            int btDelta = skillData.cost * -1;
            if (action.isActorFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, hpDelta, btDelta);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, btDelta);
            }

            // 1. メイン処理を一時停止させる
            _actionProcessor.SetPauseProcess(true);

            // 2. 新しい、一連の処理を行うコルーチンを開始する
            StartCoroutine(ProcessSkillActionCoroutine(action, skillData));
        }

        /// <summary>
        /// スキルの一連のアクションを処理するコルーチンです。
        /// </summary>
        private IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
        {
            // --- 1. スキル詠唱メッセージ (変更なし) ---
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateSkillCastMessage(actorName, skillData.skillName)
            );

            // --- 2. スキル効果のループ処理 (変更なし) ---
            foreach (var skillEffect in skillData.skillEffect)
            {
                bool isAllTarget = skillEffect.effectTarget == EffectTarget.FriendAll || skillEffect.effectTarget == EffectTarget.EnemyAll;

                // --- 3. スキルのカテゴリと効果範囲に応じて処理を分岐 ---
                switch (skillEffect.skillCategory)
                {
                    case SkillCategory.Recovery:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllRecoveryEffect(skillEffect));
                        else
                            yield return StartCoroutine(ProcessRecoveryEffect(DetermineEffectTarget(action, skillEffect), skillEffect));
                        break;

                    case SkillCategory.Damage:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllDamageEffect(action, skillEffect));
                        else
                            yield return StartCoroutine(ProcessDamageEffect(DetermineEffectTarget(action, skillEffect), skillEffect));
                        break;

                    case SkillCategory.Support:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllSupportEffect(action, skillEffect));
                        else
                            yield return StartCoroutine(ProcessSupportEffect(DetermineEffectTarget(action, skillEffect), skillEffect));
                        break;

                    // (以下、変更なし)
                    case SkillCategory.Movement:
                        Debug.LogWarning($"移動スキルは戦闘中に効果がありません: {skillEffect.skillCategory}");
                        yield return null;
                        break;
                    default:
                        Debug.LogWarning($"未定義のスキル効果カテゴリです: {skillEffect.skillCategory}");
                        break;
                }
            }
            // --- 4. 停止解除 (変更なし) ---
            _actionProcessor.SetPauseProcess(false);
        }

        /// <summary>
        /// 回復効果の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessRecoveryEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            // 回復量を計算し、ステータスを更新
            int hpDelta = BattleCalculator.CalculateHealValue(skillEffect.value);
            if (effectTargetAction.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(effectTargetAction.targetId, hpDelta, 0);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(effectTargetAction.targetId, hpDelta, 0);
            }

            // ステータスUIを更新し、回復メッセージの表示完了を待つ
            _battleManager.OnUpdateStatus();
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateHpHealMessage(targetName, hpDelta)
            );
        }

        /// <summary>
        /// 味方全体または敵全体の回復処理を行います。
        /// </summary>
        private IEnumerator ProcessAllRecoveryEffect(SkillEffect skillEffect)
        {
            if (skillEffect.effectTarget == EffectTarget.FriendAll)
            {
                var targetList = CharacterStatusManager.GetPartyMemberStatuses();
                foreach (var targetStatus in targetList)
                {
                    // 戦闘不能のキャラはスキップ
                    if (targetStatus.currentHp <= 0) continue;

                    int hpDelta = BattleCalculator.CalculateHealValue(skillEffect.value);
                    CharacterStatusManager.ChangeCharacterStatus(targetStatus.characterId, hpDelta, 0);
                    _battleManager.OnUpdateStatus();
                    string targetName = _actionProcessor.GetCharacterName(targetStatus.characterId, true);
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateHpHealMessage(targetName, hpDelta)
                    );
                }
            }
            else if (skillEffect.effectTarget == EffectTarget.EnemyAll)
            {
                var targetList = _enemyStatusManager.GetEnemyStatusList();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;

                    int hpDelta = BattleCalculator.CalculateHealValue(skillEffect.value);
                    _enemyStatusManager.ChangeEnemyStatus(targetStatus.enemyBattleId, hpDelta, 0);
                    _battleManager.UpdateEnemyStatusUI();
                    string targetName = _actionProcessor.GetCharacterName(targetStatus.enemyBattleId, false);
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateHpHealMessage(targetName, hpDelta)
                    );
                }
            }
        }

        /// <summary>
        /// ダメージ効果の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessDamageEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            // 術者と対象のパラメータを取得
            var actorParam = _actionProcessor.GetCharacterParameter(effectTargetAction.actorId, effectTargetAction.isActorFriend);
            var targetParam = _actionProcessor.GetCharacterParameter(effectTargetAction.targetId, effectTargetAction.isTargetFriend);

            // スキル基本威力とパラメータを元にダメージを計算
            // ここでは術者のATKをスキルの威力に加算する例
            int totalPower = skillEffect.value + actorParam.atk;
            int damage = BattleCalculator.CalculateDamage(totalPower, targetParam.def, targetParam.isGuarding);
            int hpDelta = damage * -1;

            // ステータス変更
            if (effectTargetAction.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(effectTargetAction.targetId, hpDelta, 0);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(effectTargetAction.targetId, hpDelta, 0);
            }

            // UI更新とメッセージ表示
            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyStatusUI();
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);

            // MessageWindowControllerの既存メソッドを利用
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateDamageMessage(targetName, damage)
            );
        }

        /// <summary>
        /// 全体ダメージ効果の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessAllDamageEffect(BattleAction action, SkillEffect skillEffect)
        {
            var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
            int totalPower = skillEffect.value + actorParam.atk;

            if (skillEffect.effectTarget == EffectTarget.FriendAll)
            {
                var targetList = CharacterStatusManager.GetPartyMemberStatuses();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var targetParam = _actionProcessor.GetCharacterParameter(targetStatus.characterId, true);
                    int damage = BattleCalculator.CalculateDamage(totalPower, targetParam.def, targetParam.isGuarding);
                    int hpDelta = damage * -1;

                    CharacterStatusManager.ChangeCharacterStatus(targetStatus.characterId, hpDelta, 0);
                    _battleManager.OnUpdateStatus();
                    string targetName = _actionProcessor.GetCharacterName(targetStatus.characterId, true);
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateDamageMessage(targetName, damage)
                    );
                }
            }
            else if (skillEffect.effectTarget == EffectTarget.EnemyAll)
            {
                var targetList = _enemyStatusManager.GetEnemyStatusList();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var targetParam = _actionProcessor.GetCharacterParameter(targetStatus.enemyBattleId, false);
                    int damage = BattleCalculator.CalculateDamage(totalPower, targetParam.def, targetParam.isGuarding);
                    int hpDelta = damage * -1;

                    _enemyStatusManager.ChangeEnemyStatus(targetStatus.enemyBattleId, hpDelta, 0);
                    _battleManager.UpdateEnemyStatusUI();
                    string targetName = _actionProcessor.GetCharacterName(targetStatus.enemyBattleId, false);
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateDamageMessage(targetName, damage)
                    );
                }
            }
        }

        /// <summary>
        /// 補助効果（ステータス変動）の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessSupportEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            // 持続ターンが-1の場合は、バフを適用せずメッセージ表示のみ行う
            if (skillEffect.duration > -1)
            {
                // SkillEffect（設計図）を元に、新しいAppliedBuff（適用効果）を生成
                var newBuff = new AppliedBuff
                {
                    parameter = skillEffect.skillParameter,
                    value = skillEffect.value,
                    duration = skillEffect.duration
                };

                // 対象キャラクターのバフリストに追加
                if (effectTargetAction.isTargetFriend)
                {
                    var status = CharacterStatusManager.GetCharacterStatusById(effectTargetAction.targetId);
                    // (注: 同じ種類のバフを重ねがけする場合のルールは別途実装が必要です)
                    status.buffs.Add(newBuff);
                }
                else
                {
                    var status = _enemyStatusManager.GetEnemyStatusByBattleId(effectTargetAction.targetId);
                    status.buffs.Add(newBuff);
                }
            }

            // メッセージ表示処理
            string message;
            if (skillEffect.value > 0)
            {
                message = $"{_actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend)}の {skillEffect.skillParameter} が あがった！";
            }
            else
            {
                message = $"{_actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend)}の {skillEffect.skillParameter} が さがった！";
            }

            _battleManager.OnUpdateStatus();
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateSupportEffectMessage(targetName, skillEffect.skillParameter, skillEffect.value)
            );
        }

        /// <summary>
        /// 全体補助効果の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessAllSupportEffect(BattleAction action, SkillEffect skillEffect)
        {
            if (skillEffect.effectTarget == EffectTarget.FriendAll)
            {
                var targetList = CharacterStatusManager.GetPartyMemberStatuses();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    // 単体用のProcessSupportEffectを、対象を差し替えながら実行する
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, skillEffect));
                }
            }
            else if (skillEffect.effectTarget == EffectTarget.EnemyAll)
            {
                var targetList = _enemyStatusManager.GetEnemyStatusList();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, skillEffect));
                }
            }
        }

        /// <summary>
        /// スキル効果の対象を決定します。
        /// </summary>
        private BattleAction DetermineEffectTarget(BattleAction originalAction, SkillEffect skillEffect)
        {
            // 元のアクション情報をコピーして、ターゲット情報だけを上書きできるようにする
            BattleAction effectTargetAction = originalAction.Clone();

            bool isTargetFriend = (skillEffect.effectTarget == EffectTarget.Own ||
                                   skillEffect.effectTarget == EffectTarget.FriendSolo ||
                                   skillEffect.effectTarget == EffectTarget.FriendAll);

            if (isTargetFriend)
            {
                // 自分自身や味方が対象の場合
                effectTargetAction.isTargetFriend = true;
                // "自分自身" を解決する
                if (skillEffect.effectTarget == EffectTarget.Own)
                {
                    effectTargetAction.targetId = originalAction.actorId;
                }
                // （注：もし全体化スキルを作る場合は、ここでターゲットIDのリストを扱う処理が必要）
            }
            else
            {
                // 敵が対象の場合
                effectTargetAction.isTargetFriend = false;
            }

            return effectTargetAction;
        }
    }
}