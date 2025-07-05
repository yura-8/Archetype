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
        SpriteRenderer _backgroundRenderer;

        /// <summary>
        /// 敵キャラクターの表示用Spriteです。
        /// </summary>
        [SerializeField]
        List<SpriteRenderer> _enemyRenderers = new List<SpriteRenderer>();

        /// <summary>
        /// カメラへの参照です。
        /// </summary>
        Camera _mainCamera;

        /// <summary>
        /// 背景を表示します。
        /// </summary>
        public void ShowBackground()
        {
            _backgroundRenderer.gameObject.SetActive(true);
        }

        /// <summary>
        /// 背景を非表示にします。
        /// </summary>
        public void HideBackground()
        {
            _backgroundRenderer.gameObject.SetActive(false);
        }

        /// <summary>
        /// 背景と敵キャラクターの位置をカメラに合わせて設定します。
        /// </summary>
        public void SetSpritePosition()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            var cameraPos = _mainCamera.transform.position;
            var newPosition = new Vector3(cameraPos.x, cameraPos.y, 0);

            //var backgroundPosOffset = new Vector3(0, 0, 0);
            //_backgroundRenderer.transform.position = newPosition + backgroundPosOffset;

            //var enemyPosOffset = new Vector3(0, -0.5f, 0);
            //_backgroundRenderer.transform.position = newPosition + enemyPosOffset;
        }

        /// <summary>
        /// 敵キャラクターを表示します。
        /// </summary>
        /// <param name="enemyId">敵キャラクターのID</param>
        public void ShowEnemy(List<int> enemyIdList)
        {
            Sprite enemySprite = null;
            // 表示したい敵の数だけループ処理を行う
            for (int i = 0; i < enemyIdList.Count; i++)
            {
                // Inspectorで設定したSpriteRendererの数を超えないように安全チェック
                if (i >= _enemyRenderers.Count)
                {
                    //Debug.LogWarning("表示したい敵の数に対して、設定されているSpriteRendererの数が足りません。");
                    break; // ループを抜ける
                }

                // i番目の敵IDと、i番目のSpriteRendererを取得
                int enemyId = enemyIdList[i];
                SpriteRenderer currentRenderer = _enemyRenderers[i];


                var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);
                if (enemyData == null)
                {
                    SimpleLogger.Instance.LogWarning($"敵キャラクターの画像が取得できませんでした。 ID: {enemyId}");
                }
                else
                {
                    enemySprite = enemyData.sprite;
                }
                //currentRenderer.sprite = enemySprite;
                currentRenderer.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 敵キャラクターを非表示にします。
        /// </summary>
        public void HideEnemyByIndex(int index) // メソッド名も分かりやすく変更
        {
            // 不正なインデックスが渡された場合にエラーにならないよう、安全チェック
            if (index >= 0 && index < _enemyRenderers.Count)
            {
                SpriteRenderer currentRenderer = _enemyRenderers[index];
                currentRenderer.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"不正なインデックスが指定されました: {index}");
            }
        }
    }
}