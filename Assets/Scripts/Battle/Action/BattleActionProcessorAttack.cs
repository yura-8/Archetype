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

        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        }

        public void ProcessAction(BattleAction action)
        {
            var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
            var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);
            int damage = BattleCalculator.CalculateDamage(actorParam.atk, targetParam.def, targetParam.isGuarding);

            int hpDelta = damage * -1;
            bool isTargetDefeated = false;
            SpecialStatusType newStatus = SpecialStatusType.None;

            if (action.isTargetFriend)
            {
                newStatus = CharacterStatusManager.ChangeCharacterStatus(action.targetId, hpDelta, 0);
                isTargetDefeated = CharacterStatusManager.IsCharacterDefeated(action.targetId);
            }
            else
            {
                newStatus = _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, 0);
                isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(action.targetId);
                if (isTargetDefeated)
                {
                    _enemyStatusManager.OnDefeatEnemy(action.targetId);
                }
            }

            int btDelta = 20 * -1; // NORMAL_ATTACK_BT_COST
            if (action.isActorFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, 0, btDelta);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, 0, btDelta);
            }

            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ShowAttackMessageCoroutine(action, damage, isTargetDefeated, newStatus));
        }

        private IEnumerator ShowAttackMessageCoroutine(BattleAction action, int damage, bool isTargetDefeated, SpecialStatusType newStatus)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateAttackMessage(actorName)
            );

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();

            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateDamageMessage(targetName, damage)
            );

            if (newStatus != SpecialStatusType.None)
            {
                yield return _messageWindowController.StartCoroutine(
                    _messageWindowController.GenerateStatusEffectMessage(targetName, newStatus)
                );
            }

            if (isTargetDefeated)
            {
                _battleManager.UpdateEnemyVisuals();
                if (action.isTargetFriend)
                {
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateDefeateFriendMessage(targetName)
                    );
                }
                else
                {
                    yield return _messageWindowController.StartCoroutine(
                        _messageWindowController.GenerateDefeateEnemyMessage(targetName)
                    );
                }
            }

            // このクラスでの勝利判定は行わず、必ず一時停止を解除して処理を終える
            _actionProcessor.SetPauseProcess(false);
        }
    }
}