using UnityEngine;

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
        /// HP回復量。
        /// </summary>
        public int hp;

        /// <summary>
        /// バッテリー回復量。
        /// </summary>
        public int bt;

        /// <summary>
        /// 使用可能回数です。
        /// </summary>
        public int numberOfUse;

        /// <summary>
        /// 攻撃力の補正値です。
        /// </summary>
        public int atk;

        /// <summary>
        /// 防御力の補正値です。
        /// </summary>
        public int def;

        /// <summary>
        /// 素早さの補正値です。
        /// </summary>
        public int dex;

        /// <summary>
        /// アイテムの価格です。
        /// </summary>
        public int price;
    }
}