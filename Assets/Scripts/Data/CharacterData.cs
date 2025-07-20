using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターに関する設定を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/SimpleRpg/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        /// <summary>
        /// キャラクターのIDです。
        /// </summary>
        public int characterId;

        /// <summary>
        /// キャラクターの名前です。
        /// </summary>
        public string characterName;

        /// <summary>
        /// キャラクターの名前です。
        /// </summary>
        public string firstName;

        /// <summary>
        /// キャラクターの属性です。
        /// </summary>
        public ElementAttribute attribute;

        [Header("戦闘時のスプライト")]
        public Sprite idleSprite; // 待機中の画像
        public Sprite actionSprite; // 行動中の画像

        /// <summary>
        /// レベルと覚えるスキルの対応レコードのリストです。
        /// </summary>
        public List<CharacterSkillRecord> characterSkillRecords;
    }
}