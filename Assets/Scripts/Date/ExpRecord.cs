using System;

namespace SimpleRpg
{
    /// <summary>
    /// レベルに対応する経験値を保持するクラスです。
    /// </summary>
    [Serializable]
    public class ExpRecord
    {
        /// <summary>
        /// レベルの値です。
        /// </summary>
        public int level;

        /// <summary>
        /// レベルに対応する経験値の値です。
        /// </summary>
        public int exp;
    }
}