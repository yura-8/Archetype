using System;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターがスキルを覚えるレベルを定義するクラスです。
    /// </summary>
    [Serializable]
    public class CharacterSkillRecord
    {
        /// <summary>
        /// スキルを覚えるレベルの値です。
        /// </summary>
        public int level;

        /// <summary>
        /// 覚えるスキルのIDです。
        /// </summary>
        public int skillId;
    }
}