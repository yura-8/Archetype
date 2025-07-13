using System.Linq; // ★ Linqを使用するために追加

namespace SimpleRpg
{
    /// <summary>
    /// 装備品による補正を計算するクラスです。
    /// </summary>
    public static class EquipmentCalculator
    {
        /// <summary>
        /// 装備品のIDから補正値を取得します。
        /// </summary>
        public static BattleParameter GetEquipmentParameter(int weaponId, int armorId)
        {
            BattleParameter battleParameter = new();
            CalculateBattleParameter(weaponId, battleParameter);
            CalculateBattleParameter(armorId, battleParameter);
            return battleParameter;
        }

        /// <summary>
        /// IDからアイテムデータを取得して、戦闘パラメータに加算します。
        /// </summary>
        static void CalculateBattleParameter(int itemId, BattleParameter battleParameter)
        {
            ItemData itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData != null && itemData.itemEffects != null)
            {
                // itemEffectsリストから能力値上昇効果を探して加算する
                foreach (var effect in itemData.itemEffects)
                {
                    // サポート効果で、かつ永続効果（durationが0以下）のものを装備補正値とみなす
                    if (effect.itemEffectCategory == ItemEffectCategory.Support && effect.duration <= 0)
                    {
                        switch (effect.skillParameter)
                        {
                            case SkillParameter.atk:
                                battleParameter.atk += effect.value;
                                break;
                            case SkillParameter.def:
                                battleParameter.def += effect.value;
                                break;
                            case SkillParameter.dex:
                                battleParameter.dex += effect.value;
                                break;
                        }
                    }
                }
            }
        }
    }
}