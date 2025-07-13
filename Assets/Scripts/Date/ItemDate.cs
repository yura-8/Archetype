using UnityEngine;
using System.Collections.Generic; // ★ Listを使用するために追加

namespace SimpleRpg
{
    /// <summary>
    /// アイテムの情報を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/SimpleRpg/ItemData")]
    public class ItemData : ScriptableObject
    {
        /// <summary>
        /// アイテムのIDです。
        /// </summary>
        public int itemId;

        /// <summary>
        /// アイテムの名前です。
        /// </summary>
        public string itemName;

        /// <summary>
        /// アイテムの説明です。
        /// </summary>
        public string itemDesc;

        /// <summary>
        /// アイテムのカテゴリです。
        /// </summary>
        public ItemCategory itemCategory;

        /// <summary>
        /// アイテムの効果リストです。
        /// </summary>
        public List<ItemEffect> itemEffects;

        /// <summary>
        /// 使用可能回数です。
        /// </summary>
        public int numberOfUse;

        /// <summary>
        /// アイテムの価格です。
        /// </summary>
        public int price;
    }
}