using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleRpg;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private float encounterChance = 0.05f; // エンカウント確率（5%）
    [SerializeField] private LayerMask encounterAreaLayer;  // エンカウントエリアのレイヤー
    private float encounterTimer;                           // エンカウント用タイマー
    private const float CHECK_INTERVAL = 1f;                // チェック間隔（秒）
    private float encounterCooldownTimer;                   // エンカウント抑制用タイマー
    private const float COOLDOWN_DURATION = 5f;             // 抑制時間（5秒）

    void Start()
    {
        // シーン遷移を検知してタイマーをリセット
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // シーン破棄時にイベントを解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            encounterCooldownTimer = COOLDOWN_DURATION; // メインシーンに戻った際に5秒間抑制
            Debug.Log("Encounter cooldown started: " + encounterCooldownTimer + " seconds remaining");
        }
    }

    void Update()
    {
        // クールダウン中はエンカウントチェックをスキップ
        if (encounterCooldownTimer > 0)
        {
            encounterCooldownTimer -= Time.deltaTime;
            if (encounterCooldownTimer <= 0)
            {
                Debug.Log("Encounter cooldown ended");
            }
            return;
        }

        CheckEncounter();
    }

    void CheckEncounter()
    {
        // プレイヤーの位置に基づき、エンカウントエリアに入っているか判定
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, encounterAreaLayer);
        if (hitColliders.Length > 0)
        {
            encounterTimer += Time.deltaTime;
            if (encounterTimer >= CHECK_INTERVAL)
            {
                encounterTimer = 0f;
                if (Random.value <= encounterChance)
                {
                    TriggerRandomEncounter(); // エンカウント発生時の処理
                }
            }
        }
        else
        {
            encounterTimer = 0f;
        }
    }

    public void TriggerRandomEncounter()
    {
        // パーティが空でないことを確認
        if (GameDataManager.Instance.PartyCharacterIds == null || GameDataManager.Instance.PartyCharacterIds.Count == 0)
        {
            Debug.LogError("パーティメンバーがいません！");
            return;
        }

        // パーティリーダーのレベルを取得
        int leaderId = GameDataManager.Instance.PartyCharacterIds[0];
        var leaderStatus = CharacterStatusManager.GetCharacterStatusById(leaderId);
        if (leaderStatus == null)
        {
            Debug.LogError($"ID: {leaderId} のパーティリーダーのステータスが見つかりません。CharacterStatusManagerが正しく初期化されているか、またはデータが存在するか確認してください。");
            return; // leaderStatusがnullの場合、処理を中断
        }
        int leaderLevel = leaderStatus.level;

        // リーダーのレベルに応じた敵ID範囲を決定
        int minEnemyId, maxEnemyId;

        if (leaderLevel <= 5)
        {
            minEnemyId = 1;
            maxEnemyId = 4;
        }
        else if (leaderLevel <= 10)
        {
            minEnemyId = 5;
            maxEnemyId = 8;
        }
        else if (leaderLevel <= 15)
        {
            minEnemyId = 5;
            maxEnemyId = 12;
        }
        else
        {
            minEnemyId = 9;
            maxEnemyId = 16;
        }

        // ランダムに敵IDを選出
        List<int> randomEnemyList = new List<int>();
        int enemyCount = Random.Range(1, 5); // 敵の数（1〜4体）

        for (int i = 0; i < enemyCount; i++)
        {
            int randomEnemyId = Random.Range(minEnemyId, maxEnemyId + 1);
            randomEnemyList.Add(randomEnemyId);
        }

        // 出現敵リストを次の戦闘用に保存
        GameDataManager.Instance.NextEncounterEnemyIds = randomEnemyList;

        // プレイヤーの位置を保存
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SavePosition();
            Debug.Log("Player position saved before battle: " + playerMovement.transform.position);
        }

        SceneManager.LoadScene("BattleScene");
    }

    // デバッグ用：エンカウント範囲の表示
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
