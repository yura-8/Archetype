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

            // 属性相性を考慮したダメージ計算
            int baseDamage = BattleCalculator.CalculateDamage(actorParam.atk, targetParam.def, targetParam.isGuarding);
            float attributeModifier = GetAttributeModifier(actorParam.attribute, targetParam.attribute);
            int finalDamage = Mathf.Max(1, (int)(baseDamage * attributeModifier));

            int hpDelta = finalDamage * -1;
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

            // 温度と属性に応じたBT消費量補正
            float costMultiplier = 1.0f;
            if (_battleManager.CurrentTemperature == TemperatureState.HOT)
            {
                // デフォルトのHOT補正は1.5倍
                costMultiplier = 1.5f;
                // 属性による上書き
                if (actorParam.attribute == ElementAttribute.Plasma) costMultiplier = 1.8f; // 80%増
                if (actorParam.attribute == ElementAttribute.Cryo) costMultiplier = 1.2f; // 20%増
            }
            int actualCost = (int)(20 * costMultiplier); // 20は元のコードの消費量
            int btDelta = actualCost * -1;

            if (action.isActorFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, 0, btDelta);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, 0, btDelta);
            }

            _actionProcessor.SetPauseProcess(true);
            // finalDamageをメッセージ表示に渡す
            StartCoroutine(ShowAttackMessageCoroutine(action, finalDamage, isTargetDefeated, newStatus));
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

            _actionProcessor.SetPauseProcess(false);
        }

        // 属性相性係数を返すメソッド
        private float GetAttributeModifier(ElementAttribute attacker, ElementAttribute defender)
        {
            if (attacker == ElementAttribute.None || defender == ElementAttribute.None) return 1.0f;

            if ((attacker == ElementAttribute.Plasma && defender == ElementAttribute.Cryo) ||
                (attacker == ElementAttribute.Cryo && defender == ElementAttribute.Pulse) ||
                (attacker == ElementAttribute.Pulse && defender == ElementAttribute.Plasma))
            {
                // 有利属性
                return 1.1f;
            }

            if ((attacker == ElementAttribute.Cryo && defender == ElementAttribute.Plasma) ||
                (attacker == ElementAttribute.Pulse && defender == ElementAttribute.Cryo) ||
                (attacker == ElementAttribute.Plasma && defender == ElementAttribute.Pulse))
            {
                // 不利属性
                return 0.9f;
            }

            // 同属性または三すくみ外
            return 1.0f;
        }
    }
}