using System;

namespace SimpleRpg
{
    /// <summary>
    /// アイテムの効果に関する設定を保持するクラスです。
    /// </summary>
    [Serializable]
    public class ItemEffect
    {
        /// <summary>
        /// アイテム効果のカテゴリです。
        /// </summary>
        public ItemEffectCategory itemEffectCategory;

        /// <summary>
        /// アイテムの効果範囲です。
        /// </summary>
        public EffectTarget effectTarget;

        /// <summary>
        /// 効果量です。
        /// </summary>
        public int value;

        /// <summary>
        /// 効果の対象
        /// </summary>
        public SkillParameter skillParameter;
    }
}