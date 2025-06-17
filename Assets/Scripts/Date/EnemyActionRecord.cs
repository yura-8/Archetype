using UnityEngine;
using System;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 行動パターンを定義するクラスです。
    /// </summary>
    [Serializable]
    public class EnemyActionRecord
    {
        /// <summary>
        /// 行動パターンのカテゴリです。
        /// </summary>
        public EnemyActionCategory enemyActionCategory;

        /// <summary>
        /// 行動パターンの条件リストです。
        /// </summary>
        public List<EnemyConditionRecord> enemyConditionRecords;

        /// <summary>
        /// 行動が魔法の場合の対象魔法データです。
        /// </summary>
        [Header("行動がスキルの場合の対象スキルデータを指定します。")]
        public SkillData　skillData;

        /// <summary>
        /// 行動の優先度です。
        /// </summary>
        [Header("行動の優先度を0から100までの整数で指定します。数値が大きい方が優先されます。")]
        [Range(0, 100)]
        public int priority;
    }
}