using System;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターの個別のステータス情報を保持するクラスです。
    /// </summary>
    [Serializable]
    public class CharacterStatus
    {
        /// <summary>
        /// キャラクターのIDです。
        /// </summary>
        public int characterId;

        /// <summary>
        /// キャラクターのレベルです。
        /// </summary>
        public int level;

        /// <summary>
        /// キャラクターの経験値です。
        /// </summary>
        public int exp;

        /// <summary>
        /// 現在のHPです。
        /// </summary>
        public int currentHp;

        /// <summary>
        /// 現在のBTです。
        /// </summary>
        public int currentBt;

        /// <summary>
        /// 装備中の武器のIDです。
        /// </summary>
        public int equipWeaponId;

        /// <summary>
        /// 装備中の防具のIDです。
        /// </summary>
        public int equipArmorId;

        /// <summary>
        /// 覚えたスキルのIDのリストです。
        /// </summary>
        public List<int> SkillList;

        /// <summary>
        /// キャラクターが倒されたフラグです。
        /// </summary>
        public bool isDefeated;

        public bool isGuarding; // trueなら防御中

        public List<AppliedBuff> buffs = new List<AppliedBuff>();
    }
}