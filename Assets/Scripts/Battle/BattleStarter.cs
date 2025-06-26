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
            ShowEnemyNameWindow();

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
            //int characterId = 1;
            //var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
            //if (characterStatus == null)
            //{
            //    SimpleLogger.Instance.LogWarning($"キャラクターステータスが取得できませんでした。 ID : {characterId}");
            //    return;
            //}

            //var controller = _battleManager.GetWindowManager().GetStatusWindowController();
            //controller.SetCharacterStatus(characterStatus);
            //controller.ShowWindow();

            // 1. 全てのステータスウィンドウのコントローラーを「リスト」として取得します。
            var statusControllers = _battleManager.GetWindowManager().GetStatusWindowController();

            // 2. 表示すべきパーティメンバー全員のステータスを「リスト」として取得します。
            //    (CharacterStatusManagerに、パーティメンバー全員の情報をリストで返すメソッドがあると仮定します)
            var partyStatuses = CharacterStatusManager.GetPartyMemberStatuses();

            // 3. ループ処理で、各コントローラーに各メンバーの情報をセットします。
            //    ウィンドウの数だけループを回します。
            for (int i = 0; i < statusControllers.Count; i++)
            {
                if (i < partyStatuses.Count)
                {
                    // i番目のパーティメンバーが存在する場合...

                    // i番目のコントローラーに、i番目のメンバーのステータスをセットして表示します。
                    var currentStatus = partyStatuses[i];
                    statusControllers[i].SetCharacterStatus(currentStatus); // controller[i] に対して呼び出す
                    statusControllers[i].ShowWindow();                   // controller[i] に対して呼び出す
                }
                else
                {
                    // 対応するパーティメンバーがいない（例：パーティが3人しかいない時の4番目のウィンドウ）
                    // そのコントローラーは非表示にしておきます。
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

        /// <summary>
        /// 敵キャラクターの名前表示ウィンドウを表示します。
        /// </summary>
        void ShowEnemyNameWindow()
        {
            //var controller = _battleManager.GetWindowManager().GetEnemyNameWindowController();
            //controller.ShowWindow();
            //int enemyId = _battleManager.EnemyId;
            //var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);
            //controller.SetEnemyName(enemyData.enemyName);


            var nameWindowControllers = _battleManager.GetWindowManager().GetEnemyNameWindowController();

            var enemyIdList = _battleManager.EnemyId;

            for (int i = 0; i < nameWindowControllers.Count; i++)
            {
                if (i < enemyIdList.Count)
                {
                    int enemyId = enemyIdList[i];
                    var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);

                    if (enemyData != null)
                    {
                        nameWindowControllers[i].SetEnemyName(enemyData.enemyName);
                        nameWindowControllers[i].ShowWindow();
                    }
                }
                else
                {
                    nameWindowControllers[i].HideWindow();
                }
            }

        }

        /// <summary>
        /// 敵キャラクターが出現したメッセージを表示します。
        /// </summary>
        void ShowEnemyAppearMessage()
        {

        }
    }
}