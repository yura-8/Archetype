using System;

namespace SimpleRpg
{
    /// <summary>
    /// 魔法の効果に関する設定を保持するクラスです。
    /// </summary>
    [Serializable]
    public class SkillEffect
    {
        /// <summary>
        /// 魔法のカテゴリです。
        /// </summary>
        public SkillCategory skillCategory;

        /// <summary>
        /// 魔法の効果範囲です。
        /// </summary>
        public SkillTarget magicTarget;

        /// <summary>
        /// 効果量です。
        /// </summary>
        public int value;

        /// <summary>
        /// ステイタス指定
        /// </summary>
        public ParameterRecord parameter;
    }
}
