using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    public class EnemyStatusUIManager : MonoBehaviour
    {
        [SerializeField] private List<EnemyStatusUIController> _uiSlots;

        /// <summary>
        /// 現在の敵全体のステータスをUIに反映させます。
        /// </summary>
        public void UpdateAllEnemyStatuses(List<EnemyStatus> allEnemies)
        {
            // var aliveEnemies = allEnemies.Where(e => !e.isDefeated && !e.isRunaway).ToList();

            for (int i = 0; i < _uiSlots.Count; i++)
            {
                // このUIスロットに対応する敵がリスト内に存在するか？
                if (i < allEnemies.Count)
                {
                    var enemyStatus = allEnemies[i];

                    // その敵は生きているか？
                    if (!enemyStatus.isDefeated && !enemyStatus.isRunaway)
                    {
                        // 生きていれば、表示を更新してUIを有効化
                        _uiSlots[i].UpdateDisplay(enemyStatus);
                        _uiSlots[i].Show();
                    }
                    else
                    {
                        // 倒されている、または逃げた場合はUIを非表示
                        _uiSlots[i].Hide();
                    }
                }
                else
                {
                    // 戦闘開始時から敵が割り当てられていないスロットは非表示
                    _uiSlots[i].Hide();
                }
            }
        }
    }
}