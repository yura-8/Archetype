using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターのステータスを管理するクラスです。
    /// </summary>
    public static class CharacterStatusManager
    {
        /// <summary>
        /// パーティ内にいるキャラクターのIDのリストです。
        /// </summary>
        public static List<int> partyCharacter;

        /// <summary>
        /// キャラクターのステータスのリストです。
        /// </summary>
        public static List<CharacterStatus> characterStatuses;

        /// <summary>
        /// プレイヤーの所持金です。
        /// </summary>
        public static int partyGold;

        /// <summary>
        /// プレイヤーの所持アイテムのリストです。
        /// </summary>
        public static List<PartyItemInfo> partyItemInfoList;

        /// <summary>
        /// パーティ内のキャラクターのステータスをIDで取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static CharacterStatus GetCharacterStatusById(int characterId)
        {
            return characterStatuses.Find(character => character.characterId == characterId);
        }

        /// <summary>
        /// パーティ内のキャラクターの装備も含めたパラメータをIDで取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static BattleParameter GetCharacterBattleParameterById(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            var parameterTable = CharacterDataManager.GetParameterTable(characterId);
            var parameterRecord = parameterTable.parameterRecords.Find(p => p.level == characterStatus.level);

            BattleParameter baseParameter = new()
            {
                atk = parameterRecord.atk,
                def = parameterRecord.def,
                dex = parameterRecord.dex,
            };

            BattleParameter equipmentParameter = EquipmentCalculator.GetEquipmentParameter(characterStatus.equipWeaponId, characterStatus.equipArmorId);
            baseParameter.atk += equipmentParameter.atk;
            baseParameter.def += equipmentParameter.def;
            baseParameter.dex += equipmentParameter.dex;

            return baseParameter;
        }

        /// <summary>
        /// 対象のキャラクターのステータスを増減させます。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        /// <param name="hpDelta">増減させるHP</param>
        /// <param name="btDelta">増減させるBT</param>
        public static void ChangeCharacterStatus(int characterId, int hpDelta, int btDelta)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (characterStatus == null)
            {
                Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {characterId}");
                return;
            }

            var parameterTable = CharacterDataManager.GetParameterTable(characterId);
            var parameterRecord = parameterTable.parameterRecords.Find(p => p.level == characterStatus.level);

            characterStatus.currentHp += hpDelta;
            if (characterStatus.currentHp > parameterRecord.hp)
            {
                characterStatus.currentHp = parameterRecord.hp;
            }
            else if (characterStatus.currentHp < 0)
            {
                characterStatus.currentHp = 0;
            }

            if (characterStatus.currentHp == 0)
            {
                characterStatus.isDefeated = true;
                return;
            }

            characterStatus.currentBt += btDelta;
            if (characterStatus.currentBt < 0)
            {
                characterStatus.currentBt = 0;
            }
        }

        /// <summary>
        /// 対象のキャラクターが倒れたかどうかを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static bool IsCharacterDefeated(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            return characterStatus.isDefeated;
        }

        /// <summary>
        /// 全てのキャラクターが倒れたかどうかを取得します。
        /// </summary>
        public static bool IsAllCharacterDefeated()
        {
            bool isAllDefeated = true;
            foreach (int characterId in partyCharacter)
            {
                var characterStatus = GetCharacterStatusById(characterId);
                if (!characterStatus.isDefeated)
                {
                    isAllDefeated = false;
                    break;
                }
            }
            return isAllDefeated;
        }

        /// <summary>
        /// 引数のアイテムを使用します。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        public static void UseItem(int itemId)
        {
            var partyItemInfo = partyItemInfoList.Find(info => info.itemId == itemId);
            if (partyItemInfo == null)
            {
                Debug.LogWarning($"対象のアイテムを所持していません。 ID : {itemId}");
                return;
            }

            partyItemInfo.usedNum++;

            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (partyItemInfo.usedNum >= itemData.numberOfUse && itemData.numberOfUse > 0)
            {
                partyItemInfo.itemNum--;
            }

            if (partyItemInfo.itemNum <= 0)
            {
                partyItemInfoList.Remove(partyItemInfo);
            }
        }

        /// <summary>
        /// HPが0でないパーティキャラクターの経験値を増加させます。
        /// </summary>
        /// <param name="exp">増加させる経験値</param>
        public static void IncreaseExp(int exp)
        {
            foreach (var characterId in partyCharacter)
            {
                var characterStatus = GetCharacterStatusById(characterId);
                if (!characterStatus.isDefeated)
                {
                    characterStatus.exp += exp;
                }
            }
        }

        /// <summary>
        /// パーティの所持金を増加させます。
        /// </summary>
        /// <param name="gold">増加させる金額</param>
        public static void IncreaseGold(int gold)
        {
            partyGold += gold;
        }

        /// <summary>
        /// 指定したキャラクターがレベルアップしたかどうかを返します。
        /// Trueでレベルアップしています。
        /// </summary>
        public static bool CheckLevelUp(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            var expTable = CharacterDataManager.GetExpTable();
            int targetLevel = 1;
            for (int i = 0; i < expTable.expRecords.Count; i++)
            {
                var expRecord = expTable.expRecords[i];
                if (characterStatus.exp >= expRecord.exp)
                {
                    targetLevel = expRecord.level;
                }
                else
                {
                    break;
                }
            }

            if (targetLevel > characterStatus.level)
            {
                characterStatus.level = targetLevel;
                return true;
            }

            return false;
        }
    }
}