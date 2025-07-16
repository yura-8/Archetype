using System;

namespace SimpleRpg
{
    /// <summary>
    /// レベルに対応するパラメータを保持するクラスです。
    /// </summary>
    [Serializable]
    public class ParameterRecord
    {
        /// <summary>
        /// レベルの値です。
        /// </summary>
        public int level;

        /// <summary>
        /// HPの値です。
        /// </summary>
        public int hp;

        /// <summary>
        /// バッテリーの値です。
        /// </summary>
        public int bt;

        /// <summary>
        /// 攻撃力の値です。
        /// </summary>
        public int atk;

        /// <summary>
        /// 防御の値です。
        /// </summary>
        public int def;

        /// <summary>
        /// 速度の値です。
        /// </summary>
        public int dex;
    }
}