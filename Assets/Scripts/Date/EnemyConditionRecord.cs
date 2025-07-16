using UnityEngine;
using System;

namespace SimpleRpg
{
    /// <summary>
    /// 行動パターンの条件を定義するクラスです。
    /// </summary>
    [Serializable]
    public class EnemyConditionRecord
    {
        /// <summary>
        /// 条件のカテゴリです。
        /// </summary>
        public ConditionCategory conditionCategory;

        /// <summary>
        /// 条件を比較する演算子です。
        /// </summary>
        public ComparisonOperator comparisonOperator;

        /// <summary>
        /// ターン数の条件の値です。
        /// </summary>
        [Header("ターン数の条件を0以上の整数で指定します。")]
        public int turnCriteria;

        /// <summary>
        /// HP残量の条件の値です。
        /// </summary>
        [Header("HP残量の割合を%で0から100の間で指定します。")]
        [Range(0, 100)]
        public float hpRateCriteria;

        /// <summary>
        /// BT残量の条件の値です。
        /// </summary>
        [Header("BT残量の割合を%で0から100の間で指定します。")]
        [Range(0, 100)]
        public float btRateCriteria;
    }
}