using System.Linq;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターのコマンドを選択するクラスです。
    /// </summary>
    public class EnemyCommandSelector : MonoBehaviour
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 戦闘中の敵キャラクターのデータを管理するクラスへの参照です。
        /// </summary>
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// 戦闘中のアクションを登録するクラスへの参照です。
        /// </summary>
        BattleActionRegister _battleActionRegister;

        /// <summary>
        /// 参照をセットします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラスへの参照</param>
        /// <param name="battleActionRegister">戦闘中のアクションを登録するクラスへの参照</param>
        public void SetReferences(BattleManager battleManager, BattleActionRegister battleActionRegister)
        {
            _battleManager = battleManager;
            _battleActionRegister = battleActionRegister;
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        }

        /// <summary>
        /// 敵キャラクターのコマンドを選択します。
        /// </summary>
        public void SelectEnemyCommand()
        {
            // このメソッドは変更ありません
            foreach (var enemyStatus in _enemyStatusManager.GetEnemyStatusList())
            {
                if (enemyStatus.isDefeated || enemyStatus.isRunaway)
                {
                    continue;
                }
                int targetId = CharacterStatusManager.partyCharacter[0];
                EnemyActionRecord record = SelectActionFromRecords(enemyStatus.enemyData, enemyStatus.enemyBattleId);
                if (record == null)
                {
                    _battleActionRegister.SetEnemyAttackAction(enemyStatus.enemyBattleId, targetId, enemyStatus.enemyData);
                    continue;
                }
                switch (record.enemyActionCategory)
                {
                    case EnemyActionCategory.Attack:
                        _battleActionRegister.SetEnemyAttackAction(enemyStatus.enemyBattleId, targetId, enemyStatus.enemyData);
                        break;
                    case EnemyActionCategory.Skill:
                        if (CanUseSkill(record, enemyStatus.enemyBattleId))
                        {
                            _battleActionRegister.SetEnemySkillAction(enemyStatus.enemyBattleId, targetId, record.skillData.skillId, enemyStatus.enemyData);
                        }
                        else
                        {
                            _battleActionRegister.SetEnemyAttackAction(enemyStatus.enemyBattleId, targetId, enemyStatus.enemyData);
                        }
                        break;
                    case EnemyActionCategory.Guard:
                        _battleActionRegister.SetEnemyGuardAction(enemyStatus.enemyBattleId, enemyStatus.enemyData);
                        break;
                    default:
                        _battleActionRegister.SetEnemyAttackAction(enemyStatus.enemyBattleId, targetId, enemyStatus.enemyData);
                        break;
                }
            }
            _battleManager.OnEnemyCommandSelected();
        }

        /// <summary>
        /// 行動パターンを選択します。
        /// </summary>
        EnemyActionRecord SelectActionFromRecords(EnemyData enemyData, int enemyBattleId)
        {
            // このメソッドは変更ありません
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

        /// <summary>
        /// 行動パターンの条件に合致しているか確認します。
        /// </summary>
        bool CheckCondition(EnemyActionRecord record, int enemyBattleId)
        {
            SimpleLogger.Instance.Log("CheckConditionが呼ばれました。");
            bool match = false;
            // 条件が一つも設定されていない場合は、無条件で実行すると判断
            if (record.enemyConditionRecords == null || record.enemyConditionRecords.Count == 0)
            {
                return true;
            }

            foreach (var conditionRecord in record.enemyConditionRecords)
            {
                switch (conditionRecord.conditionCategory)
                {
                    case ConditionCategory.Turn:
                        match = CheckTurnCondition(conditionRecord);
                        SimpleLogger.Instance.Log($"CheckTurnConditionの結果 : {match}");
                        break;
                    case ConditionCategory.HpRate:
                        match = CheckHpRateCondition(conditionRecord, enemyBattleId);
                        SimpleLogger.Instance.Log($"CheckHpRateConditionの結果 : {match}");
                        break;
                    case ConditionCategory.BtRate:
                        match = CheckBtRateCondition(conditionRecord, enemyBattleId);
                        SimpleLogger.Instance.Log($"CheckBtRateConditionの結果 : {match}");
                        break;
                }
                // 一つでも条件に合致しなかったら、この行動パターンは実行しない
                if (!match) return false;
            }
            // 全ての条件に合致した場合のみ true を返す
            return true;
        }

        /// <summary>
        /// ターン数の条件に合致しているか確認します。
        /// </summary>
        /// <param name="conditionRecords">条件のデータ</param>
        bool CheckTurnCondition(EnemyConditionRecord record)
        {
            return CompareValues(_battleManager.TurnCount, record.comparisonOperator, record.turnCriteria);
        }

        /// <summary>
        /// HP残量の条件に合致しているか確認します。
        /// Trueで合致しています。
        /// </summary>
        /// <param name="conditionRecords">条件のデータ</param>
        /// <param name="enemyBattleId">敵キャラクターの戦闘中ID</param>
        bool CheckHpRateCondition(EnemyConditionRecord record, int enemyBattleId)
        {
            var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(enemyBattleId);
            int currentHp = enemyStatus.currentHp;
            int maxHp = enemyStatus.enemyData.hp;
            float hpRate = currentHp * 100f / maxHp;
            SimpleLogger.Instance.Log($"HP残量の条件を確認します。 currentHp : {currentHp} || maxHp : {maxHp} || HP残量 : {hpRate}");
            return CompareValues(hpRate, record.comparisonOperator, record.hpRateCriteria);
        }

        /// <summary>
        /// BT残量の条件に合致しているか確認します。
        /// Trueで合致しています。
        /// </summary>
        bool CheckBtRateCondition(EnemyConditionRecord record, int enemyBattleId)
        {
            var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(enemyBattleId);
            int currentBt = enemyStatus.currentBt;
            int maxBt = enemyStatus.enemyData.bt;

            // 最大BTが0の場合、0除算を避ける
            if (maxBt == 0) return false;

            float btRate = (float)currentBt * 100f / maxBt;
            SimpleLogger.Instance.Log($"BT残量の条件を確認します。 currentBt : {currentBt} || maxBt : {maxBt} || BT残量 : {btRate}");
            return CompareValues(btRate, record.comparisonOperator, record.btRateCriteria);
        }

        /// <summary>
        /// 選択されたスキルが使用可能か確認します。
        /// Trueで使用可能です。
        /// </summary>
        /// <param name="record">行動パターンのデータ</param>
        /// <param name="enemyBattleId">敵キャラクターの戦闘中ID</param>
        bool CanUseSkill(EnemyActionRecord record, int enemyBattleId)
        {
            // スキルデータがnullの場合はfalseを返します。
            if (record.skillData == null)
            {
                return false;
            }

            var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(enemyBattleId);
            int currentBt = enemyStatus.currentBt;
            int btCost = record.skillData.cost;
            bool canUse = currentBt >= btCost;
            SimpleLogger.Instance.Log($"{record.skillData.skillName} のスキルが使用できるか確認します。 currentBt : {currentBt} btCost : {btCost} canUse : {canUse}");
            return canUse;
        }

        /// <summary>
        /// 演算子に応じた条件を満たしているか確認します。
        /// </summary>
        /// <param name="targetValue">対象の値</param>
        /// <param name="comparisonOperator">演算子</param>
        /// <param name="criteria">条件の値</param>
        bool CompareValues(int targetValue, ComparisonOperator comparisonOperator, int criteria)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals:
                    return targetValue == criteria;
                case ComparisonOperator.NotEquals:
                    return targetValue != criteria;
                case ComparisonOperator.GreaterThan:
                    return targetValue > criteria;
                case ComparisonOperator.GreaterOrEqual:
                    return targetValue >= criteria;
                case ComparisonOperator.LessThan:
                    return targetValue < criteria;
                case ComparisonOperator.LessOrEqual:
                    return targetValue <= criteria;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 演算子に応じた条件を満たしているか確認します。
        /// </summary>
        /// <param name="targetValue">対象の値</param>
        /// <param name="comparisonOperator">演算子</param>
        /// <param name="criteria">条件の値</param>
        bool CompareValues(float targetValue, ComparisonOperator comparisonOperator, float criteria)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals:
                    return targetValue == criteria;
                case ComparisonOperator.NotEquals:
                    return targetValue != criteria;
                case ComparisonOperator.GreaterThan:
                    return targetValue > criteria;
                case ComparisonOperator.GreaterOrEqual:
                    return targetValue >= criteria;
                case ComparisonOperator.LessThan:
                    return targetValue < criteria;
                case ComparisonOperator.LessOrEqual:
                    return targetValue <= criteria;
                default:
                    return false;
            }
        }
    }
}