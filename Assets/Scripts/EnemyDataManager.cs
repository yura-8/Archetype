// EnemyDataManager.cs
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks; 

namespace SimpleRpg
{
    public static class EnemyDataManager
    {
        // ★★★ 3. 不要な変数を削除 ★★★
        // public static List<int> enemies;

        static List<EnemyData> _enemyData = new();

        // ★★★ 3. 不要なメソッドを削除 ★★★
        // public static List<EnemyData> GetenemySatuses() { ... }

        /// <summary>
        /// 敵キャラクターのデータをロードします。
        /// </summary>
        public static async Task LoadEnemyData()
        {
            AsyncOperationHandle<IList<EnemyData>> handle = Addressables.LoadAssetsAsync<EnemyData>(AddressablesLabels.Enemy, null);
            await handle.Task;
            _enemyData = new List<EnemyData>(handle.Result);
            handle.Release();
        }

        public static EnemyData GetEnemyDataById(int enemyId)
        {
            return _enemyData.Find(enemy => enemy.enemyId == enemyId);
        }

        public static List<EnemyData> GetAllData()
        {
            return _enemyData;
        }
    }
}