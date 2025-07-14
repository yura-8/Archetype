using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 選択ウィンドウにてアイテムに関する処理を制御するクラスです。
    /// </summary>
    public class SelectionWindowItemController : MonoBehaviour
    {
        /// <summary>
        /// 項目オブジェクトとアイテムIDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _itemIdDictionary = new();

        /// <summary>
        /// 表示対象となるアイテムのリストです。
        /// </summary>
        private List<PartyItemInfo> _displayItemList = new();

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        public bool IsValidIndex(int index)
        {
            // bool isValid = index >= 0 && index < CharacterStatusManager.partyItemInfoList.Count;
            bool isValid = index >= 0 && index < _displayItemList.Count;
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// 魔法の場合は消費MPを確認、アイテムの場合は所持数を確認します。
        /// </summary>
        /// <param name="selectedIndex">選択中のインデックス</param>
        public bool IsValidSelection(int selectedIndex)
        {
            bool isValid = false;
            int indexInPage = selectedIndex % 4;
            if (!_itemIdDictionary.ContainsKey(indexInPage))
            {
                return isValid;
            }

            var itemId = _itemIdDictionary[indexInPage];
            var partyItemInfo = GameDataManager.Instance.PartyItems.Find(info => info.itemId == itemId);
            isValid = partyItemInfo.itemNum > 0;
            return isValid;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        public int GetMaxPageNum()
        {
            // int maxPage = Mathf.CeilToInt(CharacterStatusManager.partyItemInfoList.Count * 1.0f / 4.0f);
            int maxPage = Mathf.CeilToInt(_displayItemList.Count * 1.0f / 4.0f);
            return maxPage;
        }

        /// <summary>
        /// パーティが所持しているアイテムを表示用リストにセットします。
        /// </summary>
        public void SetDisplayItems()
        {
            _displayItemList.Clear();

            // パーティの全所持アイテムを取得
            var allItems = GameDataManager.Instance.PartyItems;

            // ここで表示するアイテムを絞り込むことができる
            foreach (var itemInfo in allItems)
            {
                // 例えば、所持数が1以上のアイテムだけを表示対象にする
                if (itemInfo.itemNum > 0)
                {
                    // ここでさらに「戦闘中に使えるか」などの条件で絞り込んでも良い
                    _displayItemList.Add(itemInfo);
                }
            }
        }

        /// <summary>
        /// ページ内のアイテムの項目をセットします。
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetPageItem(int page, SelectionItemUIController uiController)
        {
            _itemIdDictionary.Clear();
            int startIndex = page * 4;
            for (int i = startIndex; i < startIndex + 4; i++)
            {
                int positionIndex = i - startIndex;
                // if (i < CharacterStatusManager.partyItemInfoList.Count)
                if (i < _displayItemList.Count)
                {
                    var partyItemInfo = _displayItemList[i];
                    var itemData = ItemDataManager.GetItemDataById(partyItemInfo.itemId);
                    string itemName = itemData.itemName;
                    int itemNum = partyItemInfo.itemNum;
                    bool canSelect = CanSelectItem(itemData.itemId);
                    uiController.SetItemText(positionIndex, itemName, itemNum, canSelect);
                    uiController.SetDescriptionText(itemData.itemDesc);
                    _itemIdDictionary.Add(positionIndex, itemData.itemId);
                }
                else
                {
                    uiController.ClearItemText(positionIndex);
                }
            }

            if (_itemIdDictionary.Count == 0)
            {
                string noItemText = "* 選択できるアイテムがありません！ *";
                uiController.SetDescriptionText(noItemText);
            }
        }

        /// <summary>
        /// アイテムを使えるか確認します。
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        bool CanSelectItem(int itemId)
        {
            var partyItemInfo = GameDataManager.Instance.PartyItems.Find(info => info.itemId == itemId);
            return partyItemInfo.itemNum > 0;
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public PartyItemInfo GetItemInfo(int selectedIndex)
        {
            //PartyItemInfo itemInfo = null;
            //if (selectedIndex >= 0 && selectedIndex < CharacterStatusManager.partyItemInfoList.Count)
            //{
            //    itemInfo = CharacterStatusManager.partyItemInfoList[selectedIndex];
            //}
            //return itemInfo;

            if (IsValidIndex(selectedIndex))
            {
                return _displayItemList[selectedIndex];
            }
            return null;
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public ItemData GetItemData(int selectedIndex)
        {
            // 1. まずは、選択中のインデックスが有効か確認
            if (!IsValidIndex(selectedIndex))
            {
                // 無効なら、null（データなし）を返す
                return null;
            }

            // 2. 表示用リストから、選択中の「所持情報(PartyItemInfo)」を取得
            PartyItemInfo partyItemInfo = _displayItemList[selectedIndex];

            // 3. その所持情報に含まれるIDを使って、ItemDataManagerから「基本データ(ItemData)」を取得して返す
            return ItemDataManager.GetItemDataById(partyItemInfo.itemId);
        }
    }
}