using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の攻撃アクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorAttack : MonoBehaviour, IBattleActionProcessor
    {
        // --- 変数やSetReferencesメソッドは変更ありません ---
        private BattleActionProcessor _actionProcessor;
        private BattleManager _battleManager;
        private MessageWindowController _messageWindowController;
        private EnemyStatusManager _enemyStatusManager;
        private BattleSpriteController _battleSpriteController;
        private const int NORMAL_ATTACK_BT_COST = 20;

        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
            _battleSpriteController = _battleManager.GetBattleSpriteController();
        }

        /// <summary>
        /// 攻撃のアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
            var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);
            int damage = BattleCalculator.CalculateDamage(actorParam.atk, targetParam.def, targetParam.isGuarding);

            // ★ 修正点：ダメージとコストの処理を分離

            // --- ダメージ処理（ターゲットが受ける） ---
            int hpDelta = damage * -1;
            bool isTargetDefeated = false;
            if (action.isTargetFriend) // ターゲットが味方の場合
            {
                CharacterStatusManager.ChangeCharacterStatus(action.targetId, hpDelta, 0); // BT変化は0
                isTargetDefeated = CharacterStatusManager.IsCharacterDefeated(action.targetId);
            }
            else // ターゲットが敵の場合
            {
                _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, 0); // BT変化は0
                isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(action.targetId);
                if (isTargetDefeated)
                {
                    _enemyStatusManager.OnDefeatEnemy(action.targetId);
                }
            }

            // --- コスト処理（アクター（行動者）が支払う） ---
            int btDelta = NORMAL_ATTACK_BT_COST * -1;
            if (action.isActorFriend) // 行動者が味方の場合
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, 0, btDelta); // HP変化は0
            }
            else // 行動者が敵の場合
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, 0, btDelta); // HP変化は0
            }

            // --- メッセージ表示のコルーチン開始（変更なし） ---
            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ShowAttackMessageCoroutine(action, damage, isTargetDefeated));
        }

        /// <summary>
        /// 攻撃の一連のメッセージと処理を順番に行うコルーチン。
        /// </summary>
        private IEnumerator ShowAttackMessageCoroutine(BattleAction action, int damage, bool isTargetDefeated)
        {
            // --- 登場人物の名前を取得 ---
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            // --- ここからが「数珠つなぎ」の処理 ---

            // 1.「〇〇の攻撃！」メッセージが終わるのを待つ
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateAttackMessage(actorName)
            );

            // 2.「〇〇は XX のダメージを受けた！」メッセージが終わるのを待つ
            _battleManager.OnUpdateStatus(); // メッセージ表示の前にステータスUIを更新
            _battleManager.UpdateEnemyStatusUI(); // ★ 敵ステータスUI更新を追加

            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateDamageMessage(targetName, damage)
            );

            // 3. 相手を倒していなければ、ここで処理を終了
            if (!isTargetDefeated)
            {
                _actionProcessor.SetPauseProcess(false); // メイン処理の停止を解除して終了
                yield break;
            }

            // --- 4. 相手を倒した場合の追加処理 ---
            if (action.isTargetFriend) // 味方が倒れた場合
            {
                yield return _messageWindowController.StartCoroutine(
                    _messageWindowController.GenerateDefeateFriendMessage(targetName)
                );

                if (CharacterStatusManager.IsAllCharacterDefeated())
                {
                    _battleManager.OnGameover();
                    yield break; // ゲームオーバーなのでこれ以上処理しない
                }
            }
            else // 敵を倒した場合
            {
                // 敵スプライトを消す
                var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(action.targetId);
                if (enemyStatus != null)
                {
                    // 以前のHideEnemyByIndexは、敵が複数いるとズレる可能性があるため、
                    // BattleIdで直接指定するメソッドをBattleSpriteControllerに作るとより安全です。
                    // ここでは既存のメソッドを流用します。
                    int enemyIndex = _battleManager.EnemyId.IndexOf(enemyStatus.enemyData.enemyId);
                    if (enemyIndex != -1)
                    {
                        _battleSpriteController.HideEnemyByIndex(enemyIndex);
                    }
                }

                // 撃破メッセージが終わるのを待つ
                yield return _messageWindowController.StartCoroutine(
                    _messageWindowController.GenerateDefeateEnemyMessage(targetName)
                );

                if (_enemyStatusManager.IsAllEnemyDefeated())
                {
                    _battleManager.OnEnemyDefeated();
                    yield break; // 戦闘勝利なのでこれ以上処理しない
                }
            }

            // 5. すべての処理が完了したら、メイン処理の一時停止を解除
            _actionProcessor.SetPauseProcess(false);
        }
    }
}