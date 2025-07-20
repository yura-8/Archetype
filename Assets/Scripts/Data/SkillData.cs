using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// スキルの情報を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SimpleRpg/SkillData")]
    public class SkillData : ScriptableObject
    {
        /// <summary>
        /// スキルのID
        /// </summary>
        public int skillId;

        /// <summary>
        /// スキルの名前
        /// </summary>
        public string skillName;

        /// <summary>
        /// スキルの説明
        /// </summary>
        public string skillDesc;

        /// <summary>
        /// スキルの消費BT
        /// </summary>
        public int cost;

        /// <summary>
        /// スキルの使用制限
        /// </summary>
        public int maxCount;

        /// <summary>
        /// スキルの残り使用回数
        /// </summary>
        public int currentCount;

        /// <summary>
        /// スキルの効果リスト
        /// </summary>
        public List<SkillEffect> skillEffect;
    }
}