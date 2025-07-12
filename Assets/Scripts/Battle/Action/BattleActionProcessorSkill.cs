using System.Collections;
using System.Linq; // ToList() を使うために追加
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のスキルアクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorSkill : MonoBehaviour, IBattleActionProcessor
    {
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

            // BT消費
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

            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ProcessSkillActionCoroutine(action, skillData));
        }

        /// <summary>
        /// スキルの一連のアクションを処理するコルーチンです。
        /// </summary>
        private IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
        {
            // スキル詠唱メッセージ
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateSkillCastMessage(actorName, skillData.skillName)
            );

            // スキル効果のループ処理
            foreach (var skillEffect in skillData.skillEffect)
            {
                bool isAllTarget = skillEffect.effectTarget == EffectTarget.FriendAll || skillEffect.effectTarget == EffectTarget.EnemyAll;

                // スキルのカテゴリと効果範囲に応じて処理を分岐
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

                    case SkillCategory.Movement:
                        Debug.LogWarning($"移動スキルは戦闘中に効果がありません: {skillEffect.skillCategory}");
                        yield return null;
                        break;
                    default:
                        Debug.LogWarning($"未定義のスキル効果カテゴリです: {skillEffect.skillCategory}");
                        break;
                }
                // 戦闘が終了していたら、以降の効果処理を中断
                if (_battleManager.IsBattleFinished)
                {
                    yield break;
                }
            }
            _actionProcessor.SetPauseProcess(false);
        }

        /// <summary>
        /// 回復効果の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessRecoveryEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            int hpDelta = BattleCalculator.CalculateHealValue(skillEffect.value);
            if (effectTargetAction.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(effectTargetAction.targetId, hpDelta, 0);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(effectTargetAction.targetId, hpDelta, 0);
            }

            // UI更新
            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals(); // ★修正
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateHpHealMessage(targetName, hpDelta)
            );
        }

        /// <summary>
        /// 全体回復処理を行います。
        /// </summary>
        private IEnumerator ProcessAllRecoveryEffect(SkillEffect skillEffect)
        {
            if (skillEffect.effectTarget == EffectTarget.FriendAll)
            {
                var targetList = CharacterStatusManager.GetPartyMemberStatuses();
                foreach (var targetStatus in targetList.ToList())
                {
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
                foreach (var targetStatus in targetList.ToList())
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
                    int hpDelta = BattleCalculator.CalculateHealValue(skillEffect.value);
                    _enemyStatusManager.ChangeEnemyStatus(targetStatus.enemyBattleId, hpDelta, 0);
                    _battleManager.UpdateEnemyVisuals(); // ★修正
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
            var actorParam = _actionProcessor.GetCharacterParameter(effectTargetAction.actorId, effectTargetAction.isActorFriend);
            var targetParam = _actionProcessor.GetCharacterParameter(effectTargetAction.targetId, effectTargetAction.isTargetFriend);

            int totalPower = skillEffect.value + actorParam.atk;
            int damage = BattleCalculator.CalculateDamage(totalPower, targetParam.def, targetParam.isGuarding);
            int hpDelta = damage * -1;
            bool isTargetDefeated = false;

            // ステータス変更と撃破判定
            if (effectTargetAction.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(effectTargetAction.targetId, hpDelta, 0);
                isTargetDefeated = CharacterStatusManager.IsCharacterDefeated(effectTargetAction.targetId);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(effectTargetAction.targetId, hpDelta, 0);
                isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(effectTargetAction.targetId);
                if (isTargetDefeated)
                {
                    _enemyStatusManager.OnDefeatEnemy(effectTargetAction.targetId);
                }
            }

            // UI更新とメッセージ表示
            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);

            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateDamageMessage(targetName, damage)
            );

            // 撃破時のメッセージ処理
            if (isTargetDefeated)
            {
                _battleManager.UpdateEnemyVisuals();
                if (effectTargetAction.isTargetFriend)
                {
                    yield return _messageWindowController.StartCoroutine(
                       _messageWindowController.GenerateDefeateFriendMessage(targetName)
                    );
                    if (CharacterStatusManager.IsAllCharacterDefeated())
                    {
                        _battleManager.OnGameover();
                    }
                }
                else
                {
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateDefeateEnemyMessage(targetName)
                    );
                    if (_enemyStatusManager.IsAllEnemyDefeated())
                    {
                        _battleManager.OnEnemyDefeated();
                    }
                }
            }
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
                var targetList = CharacterStatusManager.GetPartyMemberStatuses().ToList();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var targetParam = _actionProcessor.GetCharacterParameter(targetStatus.characterId, true);
                    int damage = BattleCalculator.CalculateDamage(totalPower, targetParam.def, targetParam.isGuarding);
                    int hpDelta = damage * -1;

                    CharacterStatusManager.ChangeCharacterStatus(targetStatus.characterId, hpDelta, 0);
                    bool isTargetDefeated = CharacterStatusManager.IsCharacterDefeated(targetStatus.characterId);

                    _battleManager.OnUpdateStatus();
                    string targetName = _actionProcessor.GetCharacterName(targetStatus.characterId, true);
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateDamageMessage(targetName, damage)
                    );

                    if (isTargetDefeated)
                    {
                        _battleManager.OnUpdateStatus();
                        yield return _messageWindowController.StartCoroutine(
                           _messageWindowController.GenerateDefeateFriendMessage(targetName)
                        );
                        if (CharacterStatusManager.IsAllCharacterDefeated())
                        {
                            _battleManager.OnGameover();
                            yield break;
                        }
                    }
                }
            }
            else if (skillEffect.effectTarget == EffectTarget.EnemyAll)
            {
                var targetList = _enemyStatusManager.GetEnemyStatusList().ToList();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
                    var targetParam = _actionProcessor.GetCharacterParameter(targetStatus.enemyBattleId, false);
                    int damage = BattleCalculator.CalculateDamage(totalPower, targetParam.def, targetParam.isGuarding);
                    int hpDelta = damage * -1;

                    _enemyStatusManager.ChangeEnemyStatus(targetStatus.enemyBattleId, hpDelta, 0);
                    bool isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(targetStatus.enemyBattleId);
                    if (isTargetDefeated)
                    {
                        _enemyStatusManager.OnDefeatEnemy(targetStatus.enemyBattleId);
                    }

                    _battleManager.UpdateEnemyVisuals();
                    string targetName = _actionProcessor.GetCharacterName(targetStatus.enemyBattleId, false);
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateDamageMessage(targetName, damage)
                    );

                    if (isTargetDefeated)
                    {
                        _battleManager.UpdateEnemyVisuals();
                        yield return _messageWindowController.StartCoroutine(
                            _messageWindowController.GenerateDefeateEnemyMessage(targetName)
                        );
                        if (_enemyStatusManager.IsAllEnemyDefeated())
                        {
                            _battleManager.OnEnemyDefeated();
                            yield break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 補助効果（ステータス変動）の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessSupportEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            if (skillEffect.duration > -1)
            {
                var newBuff = new AppliedBuff
                {
                    parameter = skillEffect.skillParameter,
                    value = skillEffect.value,
                    duration = skillEffect.duration
                };
                if (effectTargetAction.isTargetFriend)
                {
                    CharacterStatusManager.GetCharacterStatusById(effectTargetAction.targetId).buffs.Add(newBuff);
                }
                else
                {
                    _enemyStatusManager.GetEnemyStatusByBattleId(effectTargetAction.targetId).buffs.Add(newBuff);
                }
            }

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals(); // ★修正
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
                foreach (var targetStatus in targetList.ToList())
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, skillEffect));
                }
            }
            else if (skillEffect.effectTarget == EffectTarget.EnemyAll)
            {
                var targetList = _enemyStatusManager.GetEnemyStatusList();
                foreach (var targetStatus in targetList.ToList())
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
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
            BattleAction effectTargetAction = originalAction.Clone();
            bool isTargetFriend = (skillEffect.effectTarget == EffectTarget.Own ||
                                   skillEffect.effectTarget == EffectTarget.FriendSolo ||
                                   skillEffect.effectTarget == EffectTarget.FriendAll);

            effectTargetAction.isTargetFriend = isTargetFriend;
            if (skillEffect.effectTarget == EffectTarget.Own)
            {
                effectTargetAction.targetId = originalAction.actorId;
            }
            return effectTargetAction;
        }
    }
}