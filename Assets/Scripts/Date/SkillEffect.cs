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
        /// スキルのカテゴリ
        /// </summary>
        public SkillCategory skillCategory;

        /// <summary>
        /// スキルの効果範囲
        /// </summary>
        public EffectTarget effectTarget;

        /// <summary>
        /// 効果量
        /// </summary>
        public int value;

        /// <summary>
        /// 効果の対象
        /// </summary>
        public SkillParameter skillParameter;
    }
}
