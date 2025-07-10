using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の逃走アクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorEscape : MonoBehaviour, IBattleActionProcessor
    {
        // --- 変数やSetReferencesメソッドは変更ありません ---
        private BattleActionProcessor _actionProcessor;
        private BattleManager _battleManager;
        private MessageWindowController _messageWindowController;
        private EnemyStatusManager _enemyStatusManager;
        private BattleSpriteController _battleSpriteController;

        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
            _battleSpriteController = _battleManager.GetBattleSpriteController();
        }

        /// <summary>
        /// 逃走のアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            // ★★★★★ここから下が修正の核心部★★★★★

            // 1. メイン処理を一時停止させる
            _actionProcessor.SetPauseProcess(true);

            // 2. 新しい、一連の処理を行うコルーチンを開始する
            StartCoroutine(ProcessEscapeActionCoroutine(action));
        }

        /// <summary>
        /// 逃走の一連のアクションを処理するコルーチンです。
        /// </summary>
        private IEnumerator ProcessEscapeActionCoroutine(BattleAction action)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);

            // --- 1.「〇〇は逃げ出した！」メッセージの表示が終わるのを待つ ---
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateEscapeMessage(actorName)
            );

            // --- 2. 逃走が成功したかどうかを判定 ---
            // 本来、逃走判定の相手は「敵全体で最も素早いキャラ」などになりますが、
            // 今回は元のロジックを維持し、仮の相手として一番目の敵のステータスを使います。
            var firstEnemyStatus = _enemyStatusManager.GetEnemyStatusList()[0];
            var firstEnemyParam = _actionProcessor.GetCharacterParameter(firstEnemyStatus.enemyBattleId, false);
            var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);

            bool isEscapeSuccess = BattleCalculator.CalculateCanEscape(actorParam.dex, firstEnemyParam.dex);

            if (isEscapeSuccess)
            {
                // --- 3A. 逃走成功の処理 ---
                if (action.isActorFriend)
                {
                    _battleManager.OnEscapeaway(); // 戦闘終了
                    yield break; // 処理を抜ける
                }
                else
                {
                    // 敵が逃げた場合の処理（現状はほぼ発生しませんが念のため）
                    _enemyStatusManager.OnEscapeEnemy(action.actorId);
                    if (_enemyStatusManager.IsAllEnemyDefeated())
                    {
                        _battleManager.OnEnemyDefeated(); // 全員逃げたら勝利
                    }
                    yield break; // 処理を抜ける
                }
            }
            else
            {
                // --- 3B. 逃走失敗の処理 ---
                yield return _messageWindowController.StartCoroutine(
                    _messageWindowController.GenerateEscapeFailedMessage()
                );
            }

            // --- 4. 逃走失敗の場合のみ、最後にメイン処理の停止を解除 ---
            _actionProcessor.SetPauseProcess(false);
        }
    }
}