// BattleSpriteController.cs

using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘関連のスプライトを制御するクラスです。
    /// </summary>
    public class BattleSpriteController : MonoBehaviour
    {
        /// <summary>
        /// 背景の表示用Spriteです。
        /// </summary>
        [SerializeField]
        private SpriteRenderer _backgroundRenderer;

        /// <summary>
        /// 敵キャラクターの表示用Spriteです。
        /// Inspectorで表示したい数だけ（例：4つ）設定します。
        /// </summary>
        [SerializeField]
        private List<SpriteRenderer> _enemyRenderers = new List<SpriteRenderer>();

        /// <summary>
        /// 背景を表示します。
        /// </summary>
        public void ShowBackground()
        {
            if (_backgroundRenderer != null)
                _backgroundRenderer.gameObject.SetActive(true);
        }

        /// <summary>
        /// 背景を非表示にします。
        /// </summary>
        public void HideBackground()
        {
            if (_backgroundRenderer != null)
                _backgroundRenderer.gameObject.SetActive(false);
        }

        /// <summary>
        /// 敵キャラクターを表示します。
        /// </summary>
        /// <param name="enemyIdList">表示したい敵のIDリスト</param>
        public void ShowEnemy(List<int> enemyIdList)
        {
            // まず、すべての敵表示スロットを非表示にしてリセットします。
            foreach (var renderer in _enemyRenderers)
            {
                if (renderer != null)
                {
                    renderer.gameObject.SetActive(false);
                }
            }

            // 表示したい敵の数だけループ処理を行います。
            for (int i = 0; i < enemyIdList.Count; i++)
            {
                // Inspectorで設定したスロットの数を超えないように安全チェック
                if (i >= _enemyRenderers.Count)
                {
                    Debug.LogWarning("表示したい敵の数に対して、設定されているSpriteRendererの数が足りません。");
                    break; // ループを抜ける
                }

                // 必要なデータを取得
                int enemyId = enemyIdList[i];
                SpriteRenderer currentRenderer = _enemyRenderers[i];
                EnemyData enemyData = EnemyDataManager.GetEnemyDataById(enemyId);

                // データが正しく取得できた場合のみ、スプライトを設定して表示する
                if (currentRenderer != null && enemyData != null && enemyData.sprite != null)
                {
                    currentRenderer.sprite = enemyData.sprite;
                    currentRenderer.gameObject.SetActive(true);
                }
                else
                {
                    SimpleLogger.Instance.LogWarning($"敵キャラクターのデータまたはスプライトが取得できませんでした。 ID: {enemyId}");
                }
            }
        }

        /// <summary>
        /// 指定されたインデックスの敵キャラクターを非表示にします。
        /// </summary>
        /// <param name="index">非表示にしたい敵のインデックス（0から始まる）</param>
        public void HideEnemyByIndex(int index)
        {
            // 不正なインデックスが渡された場合にエラーにならないよう、安全チェック
            if (index >= 0 && index < _enemyRenderers.Count && _enemyRenderers[index] != null)
            {
                _enemyRenderers[index].gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"不正なインデックスが指定されました: {index}");
            }
        }

        /// <summary>
        /// 敵リストの状態に基づいて、全ての敵スプライトの表示/非表示を更新します。
        /// </summary>
        /// <param name="allEnemies">現在の戦闘における全ての敵ステータスのリスト</param>
        public void UpdateAllEnemySprites(List<EnemyStatus> allEnemies)
        {
            // Inspectorで設定したスロットの数だけループ
            for (int i = 0; i < _enemyRenderers.Count; i++)
            {
                // このスロットに対応する敵がリストに存在し、かつ倒されていないか？
                if (i < allEnemies.Count && !allEnemies[i].isDefeated && !allEnemies[i].isRunaway)
                {
                    // 生きていれば表示
                    if (_enemyRenderers[i] != null)
                    {
                        _enemyRenderers[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    // 対応する敵がいない、または倒されている場合は非表示
                    if (_enemyRenderers[i] != null)
                    {
                        _enemyRenderers[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}