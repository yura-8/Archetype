using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターのコマンドを選択するクラスです。
    /// </summary>
    public class EnemyCommandSelector : MonoBehaviour
    {
        private class TargetInfo
        {
            public int TargetId { get; set; }
            public bool IsTargetOnSameSide { get; set; }
        }

        private BattleManager _battleManager;
        private EnemyStatusManager _enemyStatusManager;
        private BattleActionProcessor _battleActionProcessor;

        public void SetReferences(BattleManager battleManager, BattleActionProcessor battleActionProcessor)
        {
            _battleManager = battleManager;
            _battleActionProcessor = battleActionProcessor;
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        }

        public void SelectEnemyCommand()
        {
            foreach (var enemyStatus in _enemyStatusManager.GetEnemyStatusList())
            {
                if (enemyStatus.isDefeated || enemyStatus.isRunaway)
                {
                    continue;
                }

                EnemyActionRecord record = SelectActionFromRecords(enemyStatus.enemyData, enemyStatus.enemyBattleId);

                if (record == null)
                {
                    TryRegisterAttackAction(enemyStatus);
                    continue;
                }

                switch (record.enemyActionCategory)
                {
                    case EnemyActionCategory.Attack:
                        TryRegisterAttackAction(enemyStatus);
                        break;
                    case EnemyActionCategory.Skill:
                        if (CanUseSkill(record, enemyStatus.enemyBattleId))
                        {
                            TryRegisterSkillAction(record, enemyStatus);
                        }
                        else
                        {
                            TryRegisterAttackAction(enemyStatus);
                        }
                        break;
                    case EnemyActionCategory.Guard:
                        RegisterGuardAction(enemyStatus);
                        break;
                    default:
                        TryRegisterAttackAction(enemyStatus);
                        break;
                }
            }
            _battleManager.OnEnemyCommandSelected();
        }

        private void TryRegisterAttackAction(EnemyStatus actorStatus)
        {
            int targetId = SelectPlayerTarget();
            if (targetId == -1) return;

            var action = new BattleAction
            {
                actorId = actorStatus.enemyBattleId,
                isActorFriend = false,
                targetId = targetId,
                isTargetFriend = true,
                battleCommand = BattleCommand.Attack,
                actorSpeed = actorStatus.enemyData.dex,
                itemId = 0
            };
            _battleActionProcessor.RegisterAction(action);
        }

        private void TryRegisterSkillAction(EnemyActionRecord record, EnemyStatus actorStatus)
        {
            TargetInfo targetInfo = DetermineSkillTarget(record.skillData, actorStatus);
            if (targetInfo == null)
            {
                TryRegisterAttackAction(actorStatus);
                return;
            }

            var action = new BattleAction
            {
                actorId = actorStatus.enemyBattleId,
                isActorFriend = false,
                targetId = targetInfo.TargetId,
                isTargetFriend = !targetInfo.IsTargetOnSameSide,
                battleCommand = BattleCommand.Attack,
                itemId = record.skillData.skillId,
                actorSpeed = actorStatus.enemyData.dex
            };
            _battleActionProcessor.RegisterAction(action);
        }

        private void RegisterGuardAction(EnemyStatus actorStatus)
        {
            var action = new BattleAction
            {
                actorId = actorStatus.enemyBattleId,
                isActorFriend = false,
                targetId = actorStatus.enemyBattleId,
                isTargetFriend = false,
                battleCommand = BattleCommand.Guard,
                actorSpeed = actorStatus.enemyData.dex
            };
            _battleActionProcessor.RegisterAction(action);
        }

        private TargetInfo DetermineSkillTarget(SkillData skillData, EnemyStatus actorStatus)
        {
            if (skillData.skillEffect == null || skillData.skillEffect.Count == 0) return null;

            var primaryEffect = skillData.skillEffect[0];
            int targetId;

            switch (primaryEffect.effectTarget)
            {
                case EffectTarget.EnemySolo:
                    targetId = SelectPlayerTarget();
                    if (targetId == -1) return null;
                    return new TargetInfo { TargetId = targetId, IsTargetOnSameSide = false };

                case EffectTarget.FriendSolo:
                    targetId = SelectEnemyTarget(actorStatus.enemyBattleId, true);
                    if (targetId == -1) return null;
                    return new TargetInfo { TargetId = targetId, IsTargetOnSameSide = true };

                case EffectTarget.Own:
                    return new TargetInfo { TargetId = actorStatus.enemyBattleId, IsTargetOnSameSide = true };

                case EffectTarget.EnemyAll:
                case EffectTarget.FriendAll:
                    return new TargetInfo { TargetId = -1, IsTargetOnSameSide = false };

                default:
                    return new TargetInfo { TargetId = -1, IsTargetOnSameSide = false };
            }
        }

        // ★★★ ここからが修正されたターゲット選択ロジックです ★★★

        /// <summary>
        /// 攻撃対象となるプレイヤーを、ランダム性を持たせた賢いロジックで選択します。
        /// 優先度の高い候補を2体まで選び、その中からランダムで1体を選択します。
        /// </summary>
        /// <returns>選択されたプレイヤーのID。生存者がいない場合は-1。</returns>
        private int SelectPlayerTarget()
        {
            var livingPlayers = CharacterStatusManager.GetPartyMemberStatuses()
                .Where(p => p.currentHp > 0)
                .ToList();

            if (livingPlayers.Count == 0) return -1;
            if (livingPlayers.Count == 1) return livingPlayers[0].characterId;

            float GetPlayerHpRate(CharacterStatus p)
            {
                var param = CharacterDataManager.GetParameterTable(p.characterId).parameterRecords.Find(r => r.level == p.level);
                return (param == null || param.hp == 0) ? 1.0f : (float)p.currentHp / param.hp;
            }

            List<CharacterStatus> candidateTargets = new List<CharacterStatus>();

            // 優先度1: スタン状態のキャラクター
            var stunnedPlayers = livingPlayers
                .Where(p => p.currentStatus == SpecialStatusType.Stun)
                .OrderBy(GetPlayerHpRate)
                .ToList();

            if (stunnedPlayers.Count > 0)
            {
                candidateTargets = stunnedPlayers;
            }
            else
            {
                // 優先度2: HPが低いキャラクター
                candidateTargets = livingPlayers.OrderBy(GetPlayerHpRate).ToList();
            }

            // 候補者の中から上位2名（または1名）に絞る
            int candidateCount = Mathf.Min(candidateTargets.Count, 2);
            var finalCandidates = candidateTargets.GetRange(0, candidateCount);

            // 最終候補者の中からランダムで1体選ぶ
            return finalCandidates[Random.Range(0, finalCandidates.Count)].characterId;
        }

        /// <summary>
        /// 回復/補助の対象となる味方（敵）を、ランダム性を持たせた賢いロジックで選択します。
        /// HP割合が最も低い味方の候補を2体まで選び、その中からランダムで1体を選択します。
        /// </summary>
        private int SelectEnemyTarget(int actorId, bool canTargetSelf)
        {
            var potentialTargets = _enemyStatusManager.GetEnemyStatusList()
                .Where(e => !e.isDefeated && !e.isRunaway)
                .ToList();

            if (!canTargetSelf)
            {
                potentialTargets.RemoveAll(e => e.enemyBattleId == actorId);
            }

            if (potentialTargets.Count == 0) return -1;
            if (potentialTargets.Count == 1) return potentialTargets[0].enemyBattleId;

            // HP割合でソート
            var sortedAllies = potentialTargets
                .OrderBy(e => e.enemyData.hp == 0 ? 1.0f : (float)e.currentHp / e.enemyData.hp)
                .ToList();

            // 候補者の中から上位2名（または1名）に絞る
            int candidateCount = Mathf.Min(sortedAllies.Count, 2);
            var finalCandidates = sortedAllies.GetRange(0, candidateCount);

            // 最終候補者の中からランダムで1体選ぶ
            return finalCandidates[Random.Range(0, finalCandidates.Count)].enemyBattleId;
        }

        // ★★★ ターゲット選択ロジックの修正ここまで ★★★


        EnemyActionRecord SelectActionFromRecords(EnemyData enemyData, int enemyBattleId)
        {
            EnemyActionRecord record = null;
            var query = enemyData.enemyActionRecords.OrderByDescending(r => r.priority);
            foreach (var actionRecord in query)
            {
                if (CheckCondition(actionRecord, enemyBattleId))
                {
                    record = actionRecord;
                    break;
                }
            }
            return record;
        }

        bool CheckCondition(EnemyActionRecord record, int enemyBattleId)
        {
            if (record.enemyConditionRecords == null || record.enemyConditionRecords.Count == 0)
            {
                return true;
            }

            foreach (var conditionRecord in record.enemyConditionRecords)
            {
                bool match = false;
                switch (conditionRecord.conditionCategory)
                {
                    case ConditionCategory.Turn:
                        match = CheckTurnCondition(conditionRecord);
                        break;
                    case ConditionCategory.HpRate:
                        match = CheckHpRateCondition(conditionRecord, enemyBattleId);
                        break;
                    case ConditionCategory.BtRate:
                        match = CheckBtRateCondition(conditionRecord, enemyBattleId);
                        break;
                }
                if (!match) return false;
            }
            return true;
        }

        bool CheckTurnCondition(EnemyConditionRecord record)
        {
            return CompareValues(_battleManager.TurnCount, record.comparisonOperator, record.turnCriteria);
        }

        bool CheckHpRateCondition(EnemyConditionRecord record, int enemyBattleId)
        {
            var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(enemyBattleId);
            float hpRate = enemyStatus.currentHp * 100f / enemyStatus.enemyData.hp;
            return CompareValues(hpRate, record.comparisonOperator, record.hpRateCriteria);
        }

        bool CheckBtRateCondition(EnemyConditionRecord record, int enemyBattleId)
        {
            var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(enemyBattleId);
            if (enemyStatus.enemyData.bt == 0) return false;
            float btRate = (float)enemyStatus.currentBt * 100f / enemyStatus.enemyData.bt;
            return CompareValues(btRate, record.comparisonOperator, record.btRateCriteria);
        }

        bool CanUseSkill(EnemyActionRecord record, int enemyBattleId)
        {
            if (record.skillData == null) return false;
            var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(enemyBattleId);
            return enemyStatus.currentBt >= record.skillData.cost;
        }

        bool CompareValues(int targetValue, ComparisonOperator comparisonOperator, int criteria)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals: return targetValue == criteria;
                case ComparisonOperator.NotEquals: return targetValue != criteria;
                case ComparisonOperator.GreaterThan: return targetValue > criteria;
                case ComparisonOperator.GreaterOrEqual: return targetValue >= criteria;
                case ComparisonOperator.LessThan: return targetValue < criteria;
                case ComparisonOperator.LessOrEqual: return targetValue <= criteria;
                default: return false;
            }
        }

        bool CompareValues(float targetValue, ComparisonOperator comparisonOperator, float criteria)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals: return targetValue == criteria;
                case ComparisonOperator.NotEquals: return targetValue != criteria;
                case ComparisonOperator.GreaterThan: return targetValue > criteria;
                case ComparisonOperator.GreaterOrEqual: return targetValue >= criteria;
                case ComparisonOperator.LessThan: return targetValue < criteria;
                case ComparisonOperator.LessOrEqual: return targetValue <= criteria;
                default: return false;
            }
        }
    }
}