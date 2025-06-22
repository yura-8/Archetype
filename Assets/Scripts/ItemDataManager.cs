using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内のアイテムを管理するクラスです。
    /// </summary>
    public static class ItemDataManager
    {
        /// <summary>
        /// 読み込んだアイテムデータの一覧です。
        /// </summary>
        static List<ItemData> _itemDataList = new();

        /// <summary>
        /// アイテムデータをロードします。
        /// </summary>
        public static async void LoadItemData()
        {
            AsyncOperationHandle<IList<ItemData>> handle = Addressables.LoadAssetsAsync<ItemData>(AddressablesLabels.Item, null);
            await handle.Task;
            _itemDataList = new List<ItemData>(handle.Result);
            handle.Release();
        }

        /// <summary>
        /// IDからアイテムデータを取得します。
        /// </summary>
        public static ItemData GetItemDataById(int itemId)
        {
            return _itemDataList.Find(item => item.itemId == itemId);
        }

        /// <summary>
        /// 全てのデータを取得します。
        /// </summary>
        public static List<ItemData> GetAllData()
        {
            return _itemDataList;
        }
    }
}