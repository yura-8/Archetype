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
        /// 読み込んだ敵キャラクターのデータの一覧です。
        /// </summary>
        static List<EnemyData> _enemyData = new();

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