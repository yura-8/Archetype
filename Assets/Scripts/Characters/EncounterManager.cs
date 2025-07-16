using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private float encounterChance = 0.05f; // エンカウント確率（5%）
    [SerializeField] private LayerMask encounterAreaLayer; // エンカウントエリアのレイヤー
    private float encounterTimer; // エンカウント判定用タイマー
    private const float CHECK_INTERVAL = 1f; // 1秒ごとに判定

    void Update()
    {
        CheckEncounter();
    }

    void CheckEncounter()
    {
        // プレイヤーの位置でオーバーラップチェック
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, encounterAreaLayer);
        if (hitColliders.Length > 0) // エンカウントエリア内に入った
        {
            encounterTimer += Time.deltaTime;
            if (encounterTimer >= CHECK_INTERVAL)
            {
                encounterTimer = 0f;
                // 一定確率でエンカウント
                if (Random.value <= encounterChance)
                {
                    TriggerBattle();
                }
            }
        }
        else
        {
            encounterTimer = 0f; // エリア外ならリセット
        }
    }

    void TriggerBattle()
    {
        // BattleScene に遷移
        SceneManager.LoadScene("BattleScene"); // BattleScene のシーン名に変更
        // 必要に応じて戦闘データを保存（例：敵の種類）
        // PlayerPrefs や静的クラスを使用可能
    }

    // デバッグ用: エンカウントエリアの可視化
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}