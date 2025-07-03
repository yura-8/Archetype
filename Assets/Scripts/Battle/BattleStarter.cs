using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘の開始処理を行うクラスです。
    /// </summary>
    public class BattleStarter : MonoBehaviour
    {
        /// <summary>
        /// 戦闘の管理を行うクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 戦闘の開始処理を行います。
        /// </summary>
        public void StartBattle(BattleManager battleManager)
        {
            // SimpleLogger.Instance.Log("BattleStarterのStartBattleが呼ばれました。");
            _battleManager = battleManager;

            // 戦闘関連のUIを非表示にします。
            HideAllUI();

            // スプライトを表示します。
            ShowSprites();

            // ステータスのUIを表示します。
            ShowStatus();

            // コマンドウィンドウを表示します。
            //ShowCommand();

            // 敵の名前ウィンドウを表示します。
            //ShowEnemyNameWindow();

            // 敵出現のメッセージを表示します。
            ShowEnemyAppearMessage();

            // テスト用機能
            _battleManager.StartInputCommandPhase();
        }

        /// <summary>
        /// 戦闘関連のUIを全て非表示にします。
        /// </summary>
        void HideAllUI()
        {
            _battleManager.GetWindowManager().HideAllWindow();
        }

        /// <summary>
        /// 戦闘関連のスプライトを表示します。
        /// </summary>
        void ShowSprites()
        {
            var battleSpriteController = _battleManager.GetBattleSpriteController();
            //battleSpriteController.SetSpritePosition();
            battleSpriteController.ShowBackground();
            battleSpriteController.ShowEnemy(_battleManager.EnemyId);
        }

        /// <summary>
        /// 現在のステータスを表示します。
        /// </summary>
        void ShowStatus()
        {
            var statusControllers = _battleManager.GetWindowManager().GetStatusWindowController();
            var partyStatuses = CharacterStatusManager.GetPartyMemberStatuses();

            for (int i = 0; i < statusControllers.Count; i++)
            {
                if (i < partyStatuses.Count)
                {
                    var currentStatus = partyStatuses[i];
                    statusControllers[i].SetCharacterStatus(currentStatus);
                    statusControllers[i].ShowWindow();
                }
                else
                {
                    statusControllers[i].HideWindow();
                }
            }
        }

        /// <summary>
        /// コマンド入力のUIを表示します。
        /// </summary>
        void ShowCommand()
        {
            var controller = _battleManager.GetWindowManager().GetCommandWindowController();
            controller.ShowWindow();
            controller.InitializeCommand();
        }

        ///// <summary>
        ///// 敵キャラクターの名前表示ウィンドウを表示します。
        ///// </summary>
        //void ShowEnemyNameWindow()
        //{
        //    //var controller = _battleManager.GetWindowManager().GetEnemyNameWindowController();
        //    //controller.ShowWindow();
        //    //int enemyId = _battleManager.EnemyId;
        //    //var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);
        //    //controller.SetEnemyName(enemyData.enemyName);


        //    var nameWindowControllers = _battleManager.GetWindowManager().GetEnemyNameWindowController();

        //    var enemyIdList = _battleManager.EnemyId;

        //    for (int i = 0; i < nameWindowControllers.Count; i++)
        //    {
        //        if (i < enemyIdList.Count)
        //        {
        //            int enemyId = enemyIdList[i];
        //            var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);

        //            if (enemyData != null)
        //            {
        //                nameWindowControllers[i].SetEnemyName(enemyData.enemyName);
        //                nameWindowControllers[i].ShowWindow();
        //            }
        //        }
        //        else
        //        {
        //            nameWindowControllers[i].HideWindow();
        //        }
        //    }

        //}

        /// <summary>
        /// 敵キャラクターが出現したメッセージを表示します。
        /// </summary>
        void ShowEnemyAppearMessage()
        {

        }
    }
}