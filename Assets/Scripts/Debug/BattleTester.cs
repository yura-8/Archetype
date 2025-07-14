using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘機能のテストを行うためのクラスです。（起動シーン用）
    /// </summary>
    public class BattleTester : MonoBehaviour
    {
        [Header("テスト用の設定")]
        [SerializeField]
        List<int> _partyCharacterIds = new List<int> { 1, 2, 3, 4 };

        [SerializeField]
        int _playerLevel = 1;

        [SerializeField]
        int _weaponId;

        [SerializeField]
        int _armorId;

        [SerializeField]
        List<PartyItemInfo> _partyItemInfoList = new();

        [Header("戦闘機能をテストする")]
        [SerializeField]
        bool _executeBattle;

        // Updateメソッドの型を async void に変更
        async void Update()
        {
            if (!_executeBattle)
            {
                return;
            }
            _executeBattle = false;

            // GameDataManagerの初期化が完了するのを待つ
            while (GameDataManager.Instance == null || !GameDataManager.Instance.IsDataInitialized)
            {
                // IsDataInitializedがtrueになるまで1フレームずつ待機
                await System.Threading.Tasks.Task.Yield();
            }

            // 戦闘開始の準備
            ReadyForBattle();
        }

        /// <summary>
        /// 戦闘を開始する準備を行います。
        /// </summary>
        void ReadyForBattle()
        {
            // GameDataManagerにテスト用のパーティ情報を設定
            GameDataManager.Instance.PartyCharacterIds.Clear();
            GameDataManager.Instance.PartyCharacterIds.AddRange(_partyCharacterIds);

            // リーダーのレベルに応じて敵の範囲を決定
            int leaderLevel = _playerLevel;
            int minEnemyId = 1;
            int maxEnemyId = 1;

            if (leaderLevel <= 5)
            {
                minEnemyId = 1;
                maxEnemyId = 4;
            }
            //else if (leaderLevel <= 10)
            //{
            //    minEnemyId = 5;
            //    maxEnemyId = 8;
            //}
            //else if (leaderLevel <= 15)
            //{
            //    minEnemyId = 5;
            //    maxEnemyId = 12;
            //}
            //else // Lv16以上
            //{
            //    minEnemyId = 9;
            //    maxEnemyId = 16;
            //}

            // ランダムな敵リストを生成
            List<int> randomEnemyList = new List<int>();
            int enemyCount = Random.Range(1, 5);
            for (int i = 0; i < enemyCount; i++)
            {
                int randomEnemyId = Random.Range(minEnemyId, maxEnemyId + 1);
                randomEnemyList.Add(randomEnemyId);
            }

            // 生成したリストをGameDataManagerに渡す
            GameDataManager.Instance.NextEncounterEnemyIds = randomEnemyList;

            // 戦闘シーンをロード
            UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
        }
    }
}