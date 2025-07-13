using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の敵キャラクターのデータを管理するクラスです。
    /// </summary>
    public class EnemyStatusManager : MonoBehaviour
    {
        /// <summary>
        /// 戦闘中の敵キャラクターのステータス一覧です。
        /// </summary>
        List<EnemyStatus> _enemyStatuses = new();

        /// <summary>
        /// 敵キャラクターのステータス一覧を初期化します。
        /// </summary>
        public void InitializeEnemyStatusList()
        {
            _enemyStatuses.Clear();
        }

        /// <summary>
        /// 敵キャラクターのステータス一覧を取得します。
        /// </summary>
        public List<EnemyStatus> GetEnemyStatusList()
        {
            return _enemyStatuses;
        }

        /// <summary>
        /// 敵キャラクターのステータスを戦闘中のIDで取得します。
        /// </summary>
        /// <param name="battleId">戦闘中のID</param>
        public EnemyStatus GetEnemyStatusByBattleId(int battleId)
        {
            return _enemyStatuses.Find(status => status.enemyBattleId == battleId);
        }

        /// <summary>
        /// 戦闘中の敵キャラクターのうち、最大の戦闘中IDを取得します。
        /// </summary>
        public int GetMaxBattleId()
        {
            return _enemyStatuses.Count > 0 ? _enemyStatuses.Max(status => status.enemyBattleId) : -1;
        }

        /// <summary>
        /// 敵キャラクターのステータスをセットします。
        /// </summary>
        /// <param name="enemyId">敵キャラクターの定義データのID</param>
        public void SetUpEnemyStatus(List<int> enemyId)
        {
            foreach (var id in enemyId)
            {
                int battleId = GetMaxBattleId() + 1;
                var enemyData = EnemyDataManager.GetEnemyDataById(id);
                EnemyStatus enemyStatus = new EnemyStatus
                {
                    enemyId = id,
                    enemyBattleId = battleId,
                    enemyData = enemyData,
                    currentHp = enemyData.hp,
                    currentBt = enemyData.bt
                };
                _enemyStatuses.Add(enemyStatus);
            }
        }

        /// <summary>
        /// 敵キャラクターのステータスを変更します。
        /// </summary>
        /// <param name="battleId">戦闘中のID</param>
        /// <param name="hpDelta">HPの変化量</param>
        /// <param name="btDelta">BTの変化量</param>
        public SpecialStatusType ChangeEnemyStatus(int battleId, int hpDelta, int btDelta)
        {
            var enemyStatus = GetEnemyStatusByBattleId(battleId);
            if (enemyStatus == null) return SpecialStatusType.None;

            SpecialStatusType newStatus = SpecialStatusType.None;

            // HP変動
            enemyStatus.currentHp += hpDelta;
            if (enemyStatus.currentHp > enemyStatus.enemyData.hp) enemyStatus.currentHp = enemyStatus.enemyData.hp;
            if (enemyStatus.currentHp < 0) enemyStatus.currentHp = 0;

            // BT変動
            if (btDelta != 0)
            {
                int effectiveMaxBt = enemyStatus.enemyData.bt - enemyStatus.maxBtPenalty;
                if (effectiveMaxBt < 1) effectiveMaxBt = 1;

                if (btDelta < 0)
                {
                    if (Mathf.Abs(btDelta) >= effectiveMaxBt * 0.2f)
                    {
                        if (enemyStatus.currentStatus != SpecialStatusType.Overheat) newStatus = SpecialStatusType.Overheat;
                        enemyStatus.currentStatus = SpecialStatusType.Overheat;
                        enemyStatus.statusDuration = 2;
                    }
                }

                enemyStatus.currentBt += btDelta;

                if (enemyStatus.currentBt <= 0)
                {
                    enemyStatus.currentBt = 0;
                    if (enemyStatus.currentStatus != SpecialStatusType.Stun) newStatus = SpecialStatusType.Stun;
                    enemyStatus.currentStatus = SpecialStatusType.Stun;
                    enemyStatus.statusDuration = 2;
                }
                else if (enemyStatus.currentBt > effectiveMaxBt)
                {
                    int overchargeLimit = (int)(enemyStatus.enemyData.bt * 1.3f);
                    if (enemyStatus.currentBt > overchargeLimit) enemyStatus.currentBt = overchargeLimit;

                    if (enemyStatus.currentStatus != SpecialStatusType.Overcharge) newStatus = SpecialStatusType.Overcharge;
                    enemyStatus.currentStatus = SpecialStatusType.Overcharge;
                    enemyStatus.statusDuration = 99;
                    enemyStatus.maxBtPenalty = (int)(enemyStatus.enemyData.bt * 0.05f);
                }
                else if (enemyStatus.currentStatus == SpecialStatusType.Overcharge && enemyStatus.currentBt <= effectiveMaxBt)
                {
                    enemyStatus.currentStatus = SpecialStatusType.None;
                    enemyStatus.maxBtPenalty = 0;
                }
            }
            return newStatus;
        }

        /// <summary>
        /// 敵キャラクターが倒れたかどうかを取得します。
        /// </summary>
        /// <param name="battleId">戦闘中のID</param>
        public bool IsEnemyDefeated(int battleId)
        {
            var enemyStatus = GetEnemyStatusByBattleId(battleId);
            return enemyStatus.currentHp <= 0;
        }

        /// <summary>
        /// 引数の敵キャラクターが倒れたフラグをセットします。
        /// </summary>
        /// <param name="battleId">戦闘中のID</param>
        public void OnDefeatEnemy(int battleId)
        {
            var enemyStatus = GetEnemyStatusByBattleId(battleId);
            enemyStatus.isDefeated = true;
        }

        /// <summary>
        /// 引数の敵キャラクターが逃げたフラグをセットします。
        /// </summary>
        /// <param name="battleId">戦闘中のID</param>
        public void OnEscapeEnemy(int battleId)
        {
            var enemyStatus = GetEnemyStatusByBattleId(battleId);
            enemyStatus.isRunaway = true;
        }

        /// <summary>
        /// 全ての敵キャラクターが倒れた、または逃げたかどうかを取得します。
        /// </summary>
        public bool IsAllEnemyDefeated()
        {
            bool isAllDefeated = true;
            foreach (var enemyStatus in _enemyStatuses)
            {
                if (!enemyStatus.isDefeated && !enemyStatus.isRunaway)
                {
                    isAllDefeated = false;
                    break;
                }
            }
            return isAllDefeated;
        }

        /// <summary>
        /// 敵キャラクターの防御状態を設定します。
        /// </summary>
        public void SetGuardingState(int battleId, bool isGuarding)
        {
            var status = GetEnemyStatusByBattleId(battleId);
            if (status != null)
            {
                status.isGuarding = isGuarding;
            }
        }

        /// <summary>
        /// ターン開始時に全敵キャラクターの防御状態を解除します。
        /// </summary>
        public void ResetAllGuardingStates()
        {
            foreach (var status in _enemyStatuses)
            {
                status.isGuarding = false;
            }
        }
    }
}