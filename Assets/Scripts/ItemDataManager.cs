// ItemDataManager.cs
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks; 

namespace SimpleRpg
{
    public static class ItemDataManager
    {
        static List<ItemData> _itemDataList = new();

        /// <summary>
        /// アイテムデータをロードします。
        /// </summary>
        public static async Task LoadItemData()
        {
            AsyncOperationHandle<IList<ItemData>> handle = Addressables.LoadAssetsAsync<ItemData>(AddressablesLabels.Item, null);
            await handle.Task;
            _itemDataList = new List<ItemData>(handle.Result);
        }

        public static ItemData GetItemDataById(int itemId)
        {
            return _itemDataList.Find(item => item.itemId == itemId);
        }

        public static List<ItemData> GetAllData()
        {
            return _itemDataList;
        }
    }
}