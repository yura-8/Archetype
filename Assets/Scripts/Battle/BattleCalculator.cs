using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘に関連する計算を行うクラスです。
    /// </summary>
    public static class BattleCalculator
    {
        /// <summary>
        /// 攻撃時のダメージ計算を行います。
        /// </summary>
        /// <param name="attack">攻撃をする側の攻撃力</param>
        /// <param name="defense">攻撃を受ける側の防御力</param>
        public static int CalculateDamage(int attack, int defense)
        {
            float atk = attack / 2.0f;
            float def = defense / 4.0f;
            // float rand = Random.Range(0.8f, 1.2f);
            // int damage = Mathf.Max(Mathf.CeilToInt((atk - def) * rand), 1);
            int damage = Mathf.Max(Mathf.CeilToInt((atk - def)), 1);
            return damage;
        }

        /// <summary>
        /// 回復量の計算を行います。
        /// </summary>
        /// <param name="baseValue">計算の基本値</param>
        public static int CalculateHealValue(int baseValue)
        {
            // float rand = Random.Range(0.8f, 1.2f);
            // int healValue = Mathf.CeilToInt(baseValue * rand);
            int healValue = Mathf.CeilToInt(baseValue);
            return healValue;
        }

        /// <summary>
        /// 逃走できるかどうかの判定を行います。
        /// </summary>
        /// <param name="friendSpeed">逃走を試みる側の素早さ</param>
        /// <param name="enemySpeed">逃走を試みられる側の素早さ</param>
        //public static bool CalculateCanRun(int friendSpeed, int enemySpeed)
        //{
        //    float baseProbability = 50.0f;
        //    float speedDifference = friendSpeed - enemySpeed;
        //    float escapeRate = baseProbability + Mathf.Max(speedDifference, 0);
        //    float rand = Random.Range(0.0f, 1.0f) * 100f;
        //    return rand < escapeRate;
        //}

        /// <summary>
        /// 行動順決定のための優先度を計算します。
        /// </summary>
        /// <param name="speed">行動をする側の素早さ</param>
        public static int CalculateActionPriority(int speed)
        {
            // float rand = Random.Range(0.8f, 1.2f);
            // int priority = Mathf.CeilToInt(speed * rand);
            int priority = Mathf.CeilToInt(speed);
            return priority;
        }
    }
}