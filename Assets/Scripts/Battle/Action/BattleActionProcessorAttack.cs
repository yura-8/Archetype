// BattleActionProcessorAttack.cs

using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    public class BattleActionProcessorAttack : MonoBehaviour, IBattleActionProcessor
    {
        private BattleActionProcessor _actionProcessor;
        private BattleManager _battleManager;
        private MessageWindowController _messageWindowController;
        private EnemyStatusManager _enemyStatusManager;
        private const int NORMAL_ATTACK_BT_COST = 20;

        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
            // BattleSpriteControllerへの参照は不要になります
        }

        public void ProcessAction(BattleAction action)
        {
            var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
            var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);
            int damage = BattleCalculator.CalculateDamage(actorParam.atk, targetParam.def, targetParam.isGuarding);

            // --- ダメージ処理 ---
            int hpDelta = damage * -1;
            bool isTargetDefeated = false;
            if (action.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.targetId, hpDelta, 0);
                isTargetDefeated = CharacterStatusManager.IsCharacterDefeated(action.targetId);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, 0);
                isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(action.targetId);
                if (isTargetDefeated)
                {
                    _enemyStatusManager.OnDefeatEnemy(action.targetId);
                }
            }

            // --- コスト処理 ---
            int btDelta = NORMAL_ATTACK_BT_COST * -1;
            if (action.isActorFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, 0, btDelta);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, 0, btDelta);
            }

            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ShowAttackMessageCoroutine(action, damage, isTargetDefeated));
        }

        private IEnumerator ShowAttackMessageCoroutine(BattleAction action, int damage, bool isTargetDefeated)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            // 1.「〇〇の攻撃！」
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateAttackMessage(actorName)
            );

            // 2. ダメージメッセージの前にUIとスプライトを更新
            _battleManager.OnUpdateStatus();       // 味方ステータス更新
            _battleManager.UpdateEnemyVisuals();    // ★★★ 敵の見た目をまとめて更新！

            // 3.「〇〇は XX のダメージを受けた！」
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateDamageMessage(targetName, damage)
            );

            // 4. 相手を倒していなければ、ここで処理を終了
            if (!isTargetDefeated)
            {
                _actionProcessor.SetPauseProcess(false);
                yield break;
            }

            // --- 5. 相手を倒した場合の追加処理 ---
            _battleManager.UpdateEnemyVisuals(); // 倒した後の見た目を再更新

            if (action.isTargetFriend)
            {
                yield return _messageWindowController.StartCoroutine(
                    _messageWindowController.GenerateDefeateFriendMessage(targetName)
                );
                if (CharacterStatusManager.IsAllCharacterDefeated())
                {
                    _battleManager.OnGameover();
                    yield break;
                }
            }
            else // 敵を倒した場合
            {
                yield return _messageWindowController.StartCoroutine(
                    _messageWindowController.GenerateDefeateEnemyMessage(targetName)
                );
                if (_enemyStatusManager.IsAllEnemyDefeated())
                {
                    _battleManager.OnEnemyDefeated();
                    yield break;
                }
            }

            _actionProcessor.SetPauseProcess(false);
        }
    }
}