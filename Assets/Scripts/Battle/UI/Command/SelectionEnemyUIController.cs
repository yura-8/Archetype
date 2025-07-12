using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するUIを制御するクラスです。
    /// </summary>
    public class SelectionEnemyUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// 1つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionEnemyController _controller_0;

        /// <summary>
        /// 2つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionEnemyController _controller_1;

        /// <summary>
        /// 3つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionEnemyController _controller_2;

        /// <summary>
        /// 4つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionEnemyController _controller_3;

        /// <summary>
        /// 位置とコントローラの対応辞書です。
        /// </summary>
        Dictionary<int, SelectionEnemyController> _controllerDictionary = new();

        /// <summary>
        /// 位置とコントローラの対応辞書をセットアップします。
        /// </summary>
        public void SetUpControllerDictionary()
        {
            _controllerDictionary = new Dictionary<int, SelectionEnemyController>
            {
                { 0, _controller_0 },
                { 1, _controller_1 },
                { 2, _controller_2 },
                { 3, _controller_3 },
            };
        }

        /// <summary>
        /// コマンドのカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _controller_0.HideCursor();
            _controller_1.HideCursor();
            _controller_2.HideCursor();
            _controller_3.HideCursor();
        }

        /// <summary>
        /// 選択中のコマンドのカーソルを表示します。
        /// </summary>
        public void ShowSelectedCursor(int selectedPosition)
        {
            HideAllCursor();

            _controllerDictionary.TryGetValue(selectedPosition, out SelectionEnemyController selectedController);
            if (selectedController != null)
            {
                selectedController.ShowCursor();
            }
        }
       

        /// <summary>
        /// 敵キャラクターの名前をセットします。
        /// </summary>
        /// <param name="enemyName">敵キャラクターの名前</param>
        public void SetEnemyName(int position, string enemyName)
        {
            _controllerDictionary.TryGetValue(position, out SelectionEnemyController controller);
            if (controller != null)
            {
                controller.SetEnemyName(enemyName);
            }
        }

        /// <summary>
        /// 敵キャラクターの名前を空欄にします。
        /// </summary>
        public void ClearEnemyName(int position)
        {
            _controllerDictionary.TryGetValue(position, out SelectionEnemyController controller);
            if (controller != null)
            {
                controller.ClearEnemyName();
            }
        }

        /// <summary>
        /// 全ての項目のテキストを初期化します。
        /// </summary>
        public void ClearAllEnemyName()
        {
            foreach (var controller in _controllerDictionary.Values)
            {
                controller.ClearEnemyName();
            }
        }

        /// <summary>
        /// 敵リストの情報でUIを更新します。敵の数に応じて表示/非表示も行います。
        /// </summary>
        //public void UpdateEnemyList(List<EnemyData> activeEnemies)
        //{
        //    SetUpControllerDictionary();

        //    for (int i = 0; i < 4; i++)
        //    {
        //        if (_controllerDictionary.TryGetValue(i, out SelectionEnemyController controller))
        //        {
        //            if (i < activeEnemies.Count)
        //            {
        //                SetEnemyName(i,activeEnemies[i].enemyName);
        //                controller.Show();
        //            }
        //            else
        //            {
        //                controller.Hide();
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 敵リストの情報でUIを更新します。敵の数に応じて表示/非表示も行います。
        /// </summary>
        public void UpdateEnemyList(List<EnemyStatus> allEnemies) // ★ 引数をEnemyStatusのリストに変更
        {
            if (_controllerDictionary.Count == 0) // 安全のため、辞書が空なら初期化
            {
                SetUpControllerDictionary();
            }

            for (int i = 0; i < 4; i++)
            {
                if (_controllerDictionary.TryGetValue(i, out SelectionEnemyController controller))
                {
                    // このスロットに対応する敵がいるか？
                    if (i < allEnemies.Count)
                    {
                        var currentEnemyStatus = allEnemies[i];
                        SetEnemyName(i, currentEnemyStatus.enemyData.enemyName);

                        // ★★★★★ ここからが修正点 ★★★★★
                        // 倒されているかどうかで文字色を変える
                        if (currentEnemyStatus.isDefeated)
                        {
                            controller.SetTextColor(Color.gray); // 倒れていたら灰色
                        }
                        else
                        {
                            controller.SetTextColor(Color.white); // 生きていたら白（または元の色）
                        }
                        // ★★★★★ ここまで ★★★★★

                        controller.Show();
                    }
                    else
                    {
                        controller.Hide();
                    }
                }
            }
        }

        /// <summary>
        /// UIを表示します。
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// UIを非表示にします。
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}