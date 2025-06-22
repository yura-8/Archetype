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
            if (itemData != null)
            {
                battleParameter.atk += itemData.atk;
                battleParameter.def += itemData.def;
                battleParameter.dex += itemData.dex;
            }
        }
    }
}