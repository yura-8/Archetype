using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターのステータスに関する処理を提供するクラス（データはGameDataManagerが保持）
    /// </summary>
    public static class CharacterStatusManager
    {
        // ★★★ このクラスはデータを保持しないので、以下の変数を全て削除します ★★★
        // public static List<int> partyCharacter;
        // public static List<CharacterStatus> characterStatuses;
        // public static int partyGold;
        // public static List<PartyItemInfo> partyItemInfoList;

        /// <summary>
        /// 現在パーティにいるメンバー全員のステータス情報（CharacterStatus）をリストで返します。
        /// </summary>
        public static List<CharacterStatus> GetPartyMemberStatuses()
        {
            var gameData = GameDataManager.Instance;
            if (gameData == null || gameData.PartyCharacterIds == null)
            {
                return new List<CharacterStatus>();
            }

            List<CharacterStatus> partyStatuses = new List<CharacterStatus>();
            foreach (int id in gameData.PartyCharacterIds)
            {
                CharacterStatus status = GetCharacterStatusById(id);
                if (status != null)
                {
                    partyStatuses.Add(status);
                }
            }
            return partyStatuses;
        }

        /// <summary>
        /// キャラクターのステータスをIDで取得します。
        /// </summary>
        public static CharacterStatus GetCharacterStatusById(int characterId)
        {
            if (GameDataManager.Instance == null) return null;
            return GameDataManager.Instance.CharacterStatuses.Find(character => character.characterId == characterId);
        }

        // --- ここから先の全てのメソッドも、GameDataManager.Instance を参照するように修正 ---
        // (長いですが、以下に完全なコードを記載します)

        public static BattleParameter GetCharacterBattleParameterById(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (characterStatus == null) return new BattleParameter();

            var parameterTable = CharacterDataManager.GetParameterTable(characterId);
            var parameterRecord = parameterTable.parameterRecords.Find(p => p.level == characterStatus.level);

            int effectiveMaxBt = parameterRecord.bt - characterStatus.maxBtPenalty;
            if (effectiveMaxBt < 1) effectiveMaxBt = 1;

            BattleParameter baseParameter = new() { atk = parameterRecord.atk, def = parameterRecord.def, dex = parameterRecord.dex, };
            BattleParameter equipmentParameter = EquipmentCalculator.GetEquipmentParameter(characterStatus.equipWeaponId, characterStatus.equipArmorId);
            baseParameter.atk += equipmentParameter.atk;
            baseParameter.def += equipmentParameter.def;
            baseParameter.dex += equipmentParameter.dex;

            switch (characterStatus.currentStatus)
            {
                case SpecialStatusType.Overheat:
                    baseParameter.atk = (int)(baseParameter.atk * 1.2f);
                    baseParameter.def = (int)(baseParameter.def * 0.8f);
                    break;
                case SpecialStatusType.Overcharge:
                    baseParameter.atk = (int)(baseParameter.atk * 1.3f);
                    break;
            }

            float btRate = (float)characterStatus.currentBt / effectiveMaxBt;
            if (btRate >= 0.9f) baseParameter.atk = (int)(baseParameter.atk * 1.05f);
            else if (btRate <= 0.1f) baseParameter.atk = (int)(baseParameter.atk * 0.95f);

            foreach (var buff in characterStatus.buffs)
            {
                switch (buff.parameter)
                {
                    case SkillParameter.atk: baseParameter.atk += buff.value; break;
                    case SkillParameter.def: baseParameter.def += buff.value; break;
                    case SkillParameter.dex: baseParameter.dex += buff.value; break;
                }
            }

            if (baseParameter.atk < 0) baseParameter.atk = 0;
            if (baseParameter.def < 0) baseParameter.def = 0;
            if (baseParameter.dex < 0) baseParameter.dex = 0;
            return baseParameter;
        }

        public static SpecialStatusType ChangeCharacterStatus(int characterId, int hpDelta, int btDelta)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (characterStatus == null) return SpecialStatusType.None;
            var parameterRecord = CharacterDataManager.GetParameterTable(characterId).parameterRecords.Find(p => p.level == characterStatus.level);
            SpecialStatusType newStatus = SpecialStatusType.None;
            characterStatus.currentHp += hpDelta;
            if (characterStatus.currentHp > parameterRecord.hp) characterStatus.currentHp = parameterRecord.hp;
            if (characterStatus.currentHp < 0) characterStatus.currentHp = 0;
            if (characterStatus.currentHp == 0) characterStatus.isDefeated = true;
            if (btDelta != 0)
            {
                int effectiveMaxBt = parameterRecord.bt - characterStatus.maxBtPenalty;
                if (effectiveMaxBt < 1) effectiveMaxBt = 1;
                if (btDelta < 0)
                {
                    if (Mathf.Abs(btDelta) >= effectiveMaxBt * 0.2f)
                    {
                        if (characterStatus.currentStatus != SpecialStatusType.Overheat) newStatus = SpecialStatusType.Overheat;
                        characterStatus.currentStatus = SpecialStatusType.Overheat;
                        characterStatus.statusDuration = 2;
                    }
                }
                characterStatus.currentBt += btDelta;
                if (characterStatus.currentBt <= 0)
                {
                    characterStatus.currentBt = 0;
                    if (characterStatus.currentStatus != SpecialStatusType.Stun) newStatus = SpecialStatusType.Stun;
                    characterStatus.currentStatus = SpecialStatusType.Stun;
                    characterStatus.statusDuration = 2;
                }
                else if (characterStatus.currentBt > effectiveMaxBt)
                {
                    int overchargeLimit = (int)(parameterRecord.bt * 1.3f);
                    if (characterStatus.currentBt > overchargeLimit) characterStatus.currentBt = overchargeLimit;
                    if (characterStatus.currentStatus != SpecialStatusType.Overcharge) newStatus = SpecialStatusType.Overcharge;
                    characterStatus.currentStatus = SpecialStatusType.Overcharge;
                    characterStatus.statusDuration = 99;
                    int penalty = (int)(parameterRecord.bt * 0.05f);
                    if (characterStatus.attribute == ElementAttribute.Pulse) penalty = (int)(penalty * 1.5f);
                    characterStatus.maxBtPenalty = penalty;
                }
                else if (characterStatus.currentStatus == SpecialStatusType.Overcharge && characterStatus.currentBt <= effectiveMaxBt)
                {
                    characterStatus.currentStatus = SpecialStatusType.None;
                    characterStatus.maxBtPenalty = 0;
                }
            }
            return newStatus;
        }

        public static bool IsCharacterDefeated(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            return characterStatus != null && characterStatus.isDefeated;
        }


        public static bool IsAllCharacterDefeated()
        {
            foreach (int characterId in GameDataManager.Instance.PartyCharacterIds)
            {
                var characterStatus = GetCharacterStatusById(characterId);
                if (characterStatus != null && !characterStatus.isDefeated)
                {
                    return false;
                }
            }
            return true;
        }

        public static void UseItem(int itemId)
        {
            var inventory = GameDataManager.Instance.PartyItems;
            var partyItemInfo = inventory.Find(info => info.itemId == itemId);
            if (partyItemInfo == null) return;

            // 1. アイテムの所持数を1減らす
            partyItemInfo.itemNum--;

            // 2. もし所持数が0になったら、インベントリから削除する
            if (partyItemInfo.itemNum <= 0)
            {
                inventory.Remove(partyItemInfo);
            }
        }

        public static void IncreaseExp(int exp)
        {
            foreach (var characterId in GameDataManager.Instance.PartyCharacterIds)
            {
                var characterStatus = GetCharacterStatusById(characterId);
                if (characterStatus != null && !characterStatus.isDefeated)
                {
                    characterStatus.exp += exp;
                }
            }
        }

        public static void IncreaseGold(int gold)
        {
            GameDataManager.Instance.PartyGold += gold;
        }

        public static bool CheckLevelUp(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (characterStatus == null) return false;
            var expTable = CharacterDataManager.GetExpTable();
            int targetLevel = characterStatus.level;
            foreach (var expRecord in expTable.expRecords)
            {
                if (characterStatus.exp >= expRecord.exp) targetLevel = expRecord.level;
                else break;
            }
            if (targetLevel > characterStatus.level)
            {
                characterStatus.level = targetLevel;
                return true;
            }
            return false;
        }

        public static void SetGuardingState(int characterId, bool isGuarding)
        {
            var status = GetCharacterStatusById(characterId);
            if (status != null) status.isGuarding = isGuarding;
        }

        public static void ResetAllGuardingStates()
        {
            foreach (var status in GetPartyMemberStatuses())
            {
                status.isGuarding = false;
            }
        }

        public static void ApplyColdPenaltyToAll(float basePercentage)
        {
            foreach (var status in GameDataManager.Instance.CharacterStatuses)
            {
                if (status.isDefeated) continue;
                var parameterRecord = CharacterDataManager.GetParameterTable(status.characterId).parameterRecords.Find(p => p.level == status.level);
                if (parameterRecord == null) continue;
                float finalPercentage = basePercentage;
                if (status.attribute == ElementAttribute.Plasma) finalPercentage = 0.05f;
                if (status.attribute == ElementAttribute.Cryo) finalPercentage = 0.15f;
                int currentEffectiveMaxBt = parameterRecord.bt - status.maxBtPenalty;
                int penalty = (int)(currentEffectiveMaxBt * finalPercentage);
                if (status.attribute == ElementAttribute.Pulse) penalty = (int)(penalty * 1.5f);
                status.maxBtPenalty += penalty;
                int newEffectiveMaxBt = parameterRecord.bt - status.maxBtPenalty;
                if (newEffectiveMaxBt < 1) newEffectiveMaxBt = 1;
                if (status.currentBt > newEffectiveMaxBt) status.currentBt = newEffectiveMaxBt;
            }
        }
    }
}