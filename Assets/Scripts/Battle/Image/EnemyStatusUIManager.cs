using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の全ての敵ステータスUIを管理します。
    /// </summary>
    public class EnemyStatusUIManager : MonoBehaviour
    {
        // Inspectorで、シーンに配置した敵ステータスUIのスロットを全て設定する
        [SerializeField] private List<EnemyStatusUIController> _uiSlots;

        /// <summary>
        /// 現在の敵全体のステータスをUIに反映させます。
        /// </summary>
        public void UpdateAllEnemyStatuses(List<EnemyStatus> allEnemies)
        {
            // 生きている敵だけをリストアップ
            var aliveEnemies = allEnemies.Where(e => !e.isDefeated && !e.isRunaway).ToList();

            for (int i = 0; i < _uiSlots.Count; i++)
            {
                // このUIスロットに表示すべき生きている敵がいるか？
                if (i < aliveEnemies.Count)
                {
                    _uiSlots[i].UpdateDisplay(aliveEnemies[i]);
                    _uiSlots[i].Show();
                }
                else
                {
                    // 対応する敵がいない、または倒されたスロットは非表示
                    _uiSlots[i].Hide();
                }
            }
        }
    }
}