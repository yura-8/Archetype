using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 魔法の情報を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SimpleRpg/SkillData")]
    public class SkillDate : ScriptableObject
    {
        /// <summary>
        /// スキルのIDです。
        /// </summary>
        public int skillId;

        /// <summary>
        /// スキルの名前です。
        /// </summary>
        public string skillName;

        /// <summary>
        /// スキルの説明です。
        /// </summary>
        public string skillDesc;

        /// <summary>
        /// スキルの消費BTです。
        /// </summary>
        public int cost;

        /// <summary>
        /// スキルの効果リストです。
        /// </summary>
        //public List<SkillEffect> skillEffects;
    }
}