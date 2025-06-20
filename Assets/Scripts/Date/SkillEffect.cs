using System;

namespace SimpleRpg
{
    /// <summary>
    /// スキルの効果に関する設定を保持するクラスです。
    /// </summary>
    [Serializable]
    public class SkillEffect
    {
        /// <summary>
        /// スキルのカテゴリです。
        /// </summary>
        public SkillCategory skillCategory;

        /// <summary>
        /// スキルの効果範囲です。
        /// </summary>
        public EffectTarget magicTarget;

        /// <summary>
        /// 効果量です。
        /// </summary>
        public int value;

        /// <summary>
        /// ステイタス指定
        /// </summary>
        //public ParameterRecord parameter;
    }
}
