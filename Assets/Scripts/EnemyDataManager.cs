using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内の敵キャラクターのデータを管理するクラスです。
    /// </summary>
    public static class EnemyDataManager
    {
        /// <summary>
        /// パーティ内にいるキャラクターのIDのリストです。
        /// </summary>
        public static List<int> enemies;

        /// <summary>
        /// 読み込んだ敵キャラクターのデータの一覧です。
        /// </summary>
        static List<EnemyData> _enemyData = new();

        /// <summary>
        /// 現在の敵全員ののステータス情報（EnemyData）をリストで返します。
        /// </summary>
        public static List<EnemyData> GetenemySatuses()
        {
            // もしパーティメンバーのリストが未設定（null）なら、空のリストを返してエラーを防ぐ
            if (enemies == null)
            {
                return new List<EnemyData>();
            }

            List<EnemyData> enemyStatuses = new();
            foreach (int id in enemies)
            {
                EnemyData status = GetEnemyDataById(id);
                if (status != null)
                {
                    // 見つかったステータス情報を、返す用の新しいリストに追加します。
                    enemyStatuses.Add(status);
                }
            }

            // 完成した「相手の敵だけのステータスリスト」を返します。
            return enemyStatuses;
        }

        /// <summary>
        /// 敵キャラクターのデータをロードします。
        /// </summary>
        public static async void LoadEnemyData()
        {
            AsyncOperationHandle<IList<EnemyData>> handle = Addressables.LoadAssetsAsync<EnemyData>(AddressablesLabels.Enemy, null);
            await handle.Task;
            _enemyData = new List<EnemyData>(handle.Result);
            handle.Release();
        }

        /// <summary>
        /// IDから敵キャラクターのデータを取得します。
        /// </summary>
        public static EnemyData GetEnemyDataById(int enemyId)
        {
            return _enemyData.Find(enemy => enemy.enemyId == enemyId);
        }

        /// <summary>
        /// 全てのデータを取得します。
        /// </summary>
        public static List<EnemyData> GetAllData()
        {
            return _enemyData;
        }
    }
}