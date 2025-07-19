using UnityEngine;

public class VendingTrigger : MonoBehaviour
{
    [SerializeField] private GameObject shopUI; // ショップUIのルートオブジェクト

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered VendingTrigger area");
            if (shopUI != null)
            {
                shopUI.SetActive(true); // UIを表示
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited VendingTrigger area");
            if (shopUI != null)
            {
                shopUI.SetActive(false); // UIを非表示
            }
        }
    }
}