using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SimpleRpg
{
    public class ShopUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerGoldText; // 所持金テキスト
        [SerializeField] private Transform itemListParent; // アイテムリストの親
        [SerializeField] private GameObject itemButtonPrefab; // アイテムボタンのプレファブ

        void Start()
        {
            gameObject.SetActive(false); // 初期非表示
            PopulateItemList(); // アイテムリストを生成
        }

        public void OnPurchaseButtonClick(int itemIdToBuy)
        {
            ItemData itemData = ItemDataManager.GetItemDataById(itemIdToBuy);
            if (itemData == null)
            {
                Debug.LogError($"ID:{itemIdToBuy} のアイテムデータが見つかりません。");
                return;
            }

            int playerGold = GameDataManager.Instance.PartyGold;

            if (playerGold >= itemData.price)
            {
                GameDataManager.Instance.PartyGold -= itemData.price;
                Debug.Log($"{itemData.itemName} を {itemData.price}G で購入しました。残り所持金: {GameDataManager.Instance.PartyGold}G");
                AddItemToInventory(itemIdToBuy, 1);
                UpdateShopUI();
            }
            else
            {
                Debug.Log("お金が足りません！");
            }
        }

        private void AddItemToInventory(int itemId, int quantity)
        {
            var inventory = GameDataManager.Instance.PartyItems;
            PartyItemInfo existingItem = inventory.Find(itemInfo => itemInfo.itemId == itemId);

            if (existingItem != null)
            {
                existingItem.itemNum += quantity;
                Debug.Log($"所持中の {ItemDataManager.GetItemDataById(itemId).itemName} の数が {existingItem.itemNum} になりました。");
            }
            else
            {
                PartyItemInfo newItem = new PartyItemInfo { itemId = itemId, itemNum = quantity };
                inventory.Add(newItem);
                Debug.Log($"新しいアイテム {ItemDataManager.GetItemDataById(itemId).itemName} をインベントリに追加しました。");
            }
        }

        public void UpdateShopUI()
        {
            if (playerGoldText != null)
            {
                playerGoldText.text = GameDataManager.Instance.PartyGold.ToString() + "G";
            }
        }

        private void PopulateItemList()
        {
            if (itemListParent == null || itemButtonPrefab == null) return;

            var itemIds = new int[] { 1, 2, 3 }; // 例: アイテムID
            foreach (int itemId in itemIds)
            {
                ItemData itemData = ItemDataManager.GetItemDataById(itemId);
                if (itemData != null)
                {
                    GameObject button = Instantiate(itemButtonPrefab, itemListParent);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = $"{itemData.itemName} - {itemData.price}G";
                    button.GetComponent<Button>().onClick.AddListener(() => OnPurchaseButtonClick(itemId));
                }
            }
        }
    }
}