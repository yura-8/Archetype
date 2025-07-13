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
        /// 現在パーティにいるメンバー全員のステータス情報（CharacterStatus）をリストで返します。
        /// </summary>
        public static List<CharacterStatus> GetPartyMemberStatuses()
        {
            // もしパーティメンバーのリストが未設定（null）なら、空のリストを返してエラーを防ぐ
            if (partyCharacter == null)
            {
                return new List<CharacterStatus>();
            }

            List<CharacterStatus> partyStatuses = new();
            foreach (int id in partyCharacter)
            {
                // partyCharacterリストに入っているIDを元に、
                // characterStatusesリストから該当するキャラクターの情報を探します。
                CharacterStatus status = GetCharacterStatusById(id);
                if (status != null)
                {
                    // 見つかったステータス情報を、返す用の新しいリストに追加します。
                    partyStatuses.Add(status);
                }
            }

            // 完成した「パーティメンバーだけのステータスリスト」を返します。
            return partyStatuses;
        }

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

            int effectiveMaxBt = parameterRecord.bt - characterStatus.maxBtPenalty;
            if (effectiveMaxBt < 1) effectiveMaxBt = 1;

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

            // 特殊状態による補正
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

            // BT残量による補正
            float btRate = (float)characterStatus.currentBt / effectiveMaxBt;
            if (btRate >= 0.9f)
            {
                baseParameter.atk = (int)(baseParameter.atk * 1.05f);
            }
            else if (btRate <= 0.1f)
            {
                baseParameter.atk = (int)(baseParameter.atk * 0.95f);
            }


            foreach (var buff in characterStatus.buffs)
            {
                switch (buff.parameter)
                {
                    case SkillParameter.atk: baseParameter.atk += buff.value; break;
                    case SkillParameter.def: baseParameter.def += buff.value; break;
                    case SkillParameter.dex: baseParameter.dex += buff.value; break;
                }
            }

            // パラメータが0未満にならないように調整
            if (baseParameter.atk < 0) baseParameter.atk = 0;
            if (baseParameter.def < 0) baseParameter.def = 0;
            if (baseParameter.dex < 0) baseParameter.dex = 0;

            return baseParameter;
        }

        /// <summary>
        /// 対象のキャラクターのステータスを増減させます。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        /// <param name="hpDelta">増減させるHP</param>
        /// <param name="btDelta">増減させるBT</param>
        public static SpecialStatusType ChangeCharacterStatus(int characterId, int hpDelta, int btDelta)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (characterStatus == null) return SpecialStatusType.None;
            var parameterRecord = CharacterDataManager.GetParameterTable(characterId).parameterRecords.Find(p => p.level == characterStatus.level);

            SpecialStatusType newStatus = SpecialStatusType.None; // 発生したステータスを記録する変数

            // HP変動
            characterStatus.currentHp += hpDelta;
            if (characterStatus.currentHp > parameterRecord.hp) characterStatus.currentHp = parameterRecord.hp;
            if (characterStatus.currentHp < 0) characterStatus.currentHp = 0;
            if (characterStatus.currentHp == 0) characterStatus.isDefeated = true;

            // BT変動
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
                    characterStatus.maxBtPenalty = (int)(parameterRecord.bt * 0.05f);
                }
                else if (characterStatus.currentStatus == SpecialStatusType.Overcharge && characterStatus.currentBt <= effectiveMaxBt)
                {
                    characterStatus.currentStatus = SpecialStatusType.None;
                    characterStatus.maxBtPenalty = 0;
                }
            }
            return newStatus;
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

        /// <summary>
        /// キャラクターの防御状態を設定します。
        /// </summary>
        public static void SetGuardingState(int characterId, bool isGuarding)
        {
            var status = GetCharacterStatusById(characterId);
            if (status != null)
            {
                status.isGuarding = isGuarding;
            }
        }

        /// <summary>
        /// ターン開始時に全キャラクターの防御状態を解除します。
        /// </summary>
        public static void ResetAllGuardingStates()
        {
            var partyStatuses = GetPartyMemberStatuses();
            foreach (var status in partyStatuses)
            {
                status.isGuarding = false;
            }
        }

        /// <summary>
        /// COLD状態の効果として、全パーティメンバーの最大BTにペナルティを与えます。
        /// </summary>
        /// <param name="percentage">最大BTから減少させる割合（例: 0.1fで10%）</param>
        public static void ApplyColdPenaltyToAll(float percentage)
        {
            foreach (var status in characterStatuses)
            {
                if (status.isDefeated) continue;

                var parameterRecord = CharacterDataManager.GetParameterTable(status.characterId).parameterRecords.Find(p => p.level == status.level);
                if (parameterRecord != null)
                {
                    // 現在の実効最大BTを基準にペナルティ量を計算します
                    int currentEffectiveMaxBt = parameterRecord.bt - status.maxBtPenalty;
                    int penalty = (int)(currentEffectiveMaxBt * percentage);

                    // 計算したペナルティを加算します
                    status.maxBtPenalty += penalty;

                    // 新しい最大BTを元に、現在BTを調整します
                    int newEffectiveMaxBt = parameterRecord.bt - status.maxBtPenalty;
                    if (newEffectiveMaxBt < 1) newEffectiveMaxBt = 1;
                    if (status.currentBt > newEffectiveMaxBt)
                    {
                        status.currentBt = newEffectiveMaxBt;
                    }
                }
            }
        }
    }
}