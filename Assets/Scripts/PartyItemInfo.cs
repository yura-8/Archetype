using System;

namespace SimpleRpg
{
    /// <summary>
    /// 所持しているアイテムの情報を保持するクラスです。
    /// </summary>
    [Serializable]
    public class PartyItemInfo
    {
        /// <summary>
        /// アイテムのIDです。
        /// </summary>
        public int itemId;

        /// <summary>
        /// 所有している個数です。
        /// </summary>
        public int itemNum;
    }
}