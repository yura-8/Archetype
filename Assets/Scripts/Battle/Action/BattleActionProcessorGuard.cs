using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の防御アクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorGuard : MonoBehaviour, IBattleActionProcessor
    {
        // --- 変数やSetReferencesメソッドは変更ありません ---
        private BattleActionProcessor _actionProcessor;
        private BattleManager _battleManager;
        private MessageWindowController _messageWindowController;

        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        }

        /// <summary>
        /// 防御のアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            // ★★★★★ここから下が修正の核心部★★★★★

            // 1. メイン処理を一時停止させる
            _actionProcessor.SetPauseProcess(true);

            // 2. 新しい、一連の処理を行うコルーチンを開始する
            StartCoroutine(ProcessGuardActionCoroutine(action));
        }

        /// <summary>
        /// 防御の一連のアクションを処理するコルーチンです。
        /// </summary>
        private IEnumerator ProcessGuardActionCoroutine(BattleAction action)
        {
            // --- 1. キャラクターの防御状態フラグを立てる ---
            if (action.isActorFriend)
            {
                CharacterStatusManager.SetGuardingState(action.actorId, true);
            }
            else
            {
                _battleManager.GetEnemyStatusManager().SetGuardingState(action.actorId, true);
            }

            // --- 2. 「〇〇は身をまもっている！」メッセージの表示が終わるのを待つ ---
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateGuardMessage(actorName)
            );

            // --- 3. すべての処理が終わったら、メイン処理の停止を解除 ---
            _actionProcessor.SetPauseProcess(false);
        }
    }
}