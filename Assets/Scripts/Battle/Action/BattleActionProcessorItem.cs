using System.Collections;
using System.Linq;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のアイテムアクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorItem : MonoBehaviour, IBattleActionProcessor
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
            var itemData = ItemDataManager.GetItemDataById(action.itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {action.itemId}");
                return;
            }

            if (action.isActorFriend && itemData.itemCategory == ItemCategory.ConsumableItem)
            {
                CharacterStatusManager.UseItem(action.itemId);
            }

            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ProcessItemActionCoroutine(action, itemData));
        }

        private IEnumerator ProcessItemActionCoroutine(BattleAction action, ItemData itemData)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateUseItemMessage(actorName, itemData.itemName)
            );

            if (itemData.itemEffects == null || itemData.itemEffects.Count == 0)
            {
                SimpleLogger.Instance.LogWarning($"{itemData.itemName} には効果が設定されていません。");
                _actionProcessor.SetPauseProcess(false);
                yield break;
            }

            foreach (var itemEffect in itemData.itemEffects)
            {
                bool isAllTarget = itemEffect.effectTarget == EffectTarget.FriendAll || itemEffect.effectTarget == EffectTarget.EnemyAll;

                // ★ StatusCureのcaseを削除
                switch (itemEffect.itemEffectCategory)
                {
                    case ItemEffectCategory.Recovery:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllItemRecoveryEffect(action, itemEffect));
                        else
                            yield return StartCoroutine(ProcessRecoveryEffect(action, itemEffect));
                        break;

                    case ItemEffectCategory.Damage:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllItemDamageEffect(action, itemEffect));
                        else
                            yield return StartCoroutine(ProcessDamageEffect(action, itemEffect));
                        break;

                    case ItemEffectCategory.Support:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllItemSupportEffect(action, itemEffect));
                        else
                            yield return StartCoroutine(ProcessSupportEffect(action, itemEffect));
                        break;

                    case ItemEffectCategory.Temperature:
                        yield return StartCoroutine(ProcessTemperatureEffect(itemEffect));
                        break;

                    default:
                        Debug.LogWarning($"未定義のアイテム効果カテゴリです: {itemEffect.itemEffectCategory}");
                        break;
                }

                if (_battleManager.IsBattleFinished)
                {
                    yield break;
                }
            }

            _actionProcessor.SetPauseProcess(false);
        }

        private IEnumerator ProcessTemperatureEffect(ItemEffect itemEffect)
        {
            TemperatureState newState = _battleManager.CurrentTemperature;
            string message = "";

            if (itemEffect.value > 0)
            {
                newState = TemperatureState.HOT;
                message = "アイテムによって戦場の気温が急上昇した！";
            }
            else if (itemEffect.value < 0)
            {
                newState = TemperatureState.COLD;
                message = "アイテムによってあたりが急速に冷却された…";
            }
            else
            {
                newState = TemperatureState.NORMAL;
                message = "アイテムによって気温が平常に戻った。";
            }

            if (newState == _battleManager.CurrentTemperature)
            {
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateGeneralMessage("しかし何も変わらなかった…"));
                yield break;
            }

            _battleManager.ChangeTemperature(newState);
            yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateGeneralMessage(message));
        }

        private IEnumerator ProcessRecoveryEffect(BattleAction action, ItemEffect itemEffect)
        {
            int hpDelta = 0;
            int btDelta = 0;
            int healAmount = 0;
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            if (action.isTargetFriend)
            {
                var status = CharacterStatusManager.GetCharacterStatusById(action.targetId);
                var param = CharacterDataManager.GetParameterTable(status.characterId).parameterRecords.Find(p => p.level == status.level);

                if (itemEffect.skillParameter == SkillParameter.hp)
                {
                    healAmount = itemEffect.value;
                    hpDelta = healAmount;
                }
                else if (itemEffect.skillParameter == SkillParameter.bt)
                {
                    healAmount = itemEffect.value;
                    btDelta = healAmount;
                }
                CharacterStatusManager.ChangeCharacterStatus(action.targetId, hpDelta, btDelta);
            }
            else
            {
                var status = _enemyStatusManager.GetEnemyStatusByBattleId(action.targetId);
                if (status == null || status.isDefeated) yield break;

                if (itemEffect.skillParameter == SkillParameter.hp)
                {
                    healAmount = itemEffect.value;
                    hpDelta = healAmount;
                }
                else if (itemEffect.skillParameter == SkillParameter.bt)
                {
                    healAmount = itemEffect.value;
                    btDelta = healAmount;
                }
                _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, btDelta);
            }

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();

            if (itemEffect.skillParameter == SkillParameter.hp)
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateHpHealMessage(targetName, healAmount));
            else if (itemEffect.skillParameter == SkillParameter.bt)
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateBtHealMessage(targetName, healAmount));
        }

        private IEnumerator ProcessDamageEffect(BattleAction action, ItemEffect itemEffect)
        {
            var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);
            int damage = BattleCalculator.CalculateDamage(itemEffect.value, targetParam.def, targetParam.isGuarding);
            int hpDelta = damage * -1;
            bool isTargetDefeated = false;
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            if (action.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.targetId, hpDelta, 0);
                isTargetDefeated = CharacterStatusManager.IsCharacterDefeated(action.targetId);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, 0);
                isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(action.targetId);
                if (isTargetDefeated) _enemyStatusManager.OnDefeatEnemy(action.targetId);
            }

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();

            yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateDamageMessage(targetName, damage));

            if (isTargetDefeated)
            {
                _battleManager.UpdateEnemyVisuals();
                if (action.isTargetFriend)
                    yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateDefeateFriendMessage(targetName));
                else
                    yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateDefeateEnemyMessage(targetName));
            }
        }

        private IEnumerator ProcessSupportEffect(BattleAction action, ItemEffect itemEffect)
        {
            if (itemEffect.duration > 0)
            {
                var newBuff = new AppliedBuff
                {
                    parameter = itemEffect.skillParameter,
                    value = itemEffect.value,
                    duration = itemEffect.duration
                };

                if (action.isTargetFriend)
                    CharacterStatusManager.GetCharacterStatusById(action.targetId)?.buffs.Add(newBuff);
                else
                    _enemyStatusManager.GetEnemyStatusByBattleId(action.targetId)?.buffs.Add(newBuff);
            }

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateSupportEffectMessage(targetName, itemEffect.skillParameter, itemEffect.value)
            );
        }

        private IEnumerator ProcessAllItemRecoveryEffect(BattleAction action, ItemEffect itemEffect)
        {
            if (itemEffect.effectTarget == EffectTarget.FriendAll)
            {
                foreach (var targetStatus in CharacterStatusManager.GetPartyMemberStatuses().ToList())
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    yield return StartCoroutine(ProcessRecoveryEffect(tempAction, itemEffect));
                }
            }
            else if (itemEffect.effectTarget == EffectTarget.EnemyAll)
            {
                foreach (var targetStatus in _enemyStatusManager.GetEnemyStatusList().ToList())
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessRecoveryEffect(tempAction, itemEffect));
                }
            }
        }

        private IEnumerator ProcessAllItemDamageEffect(BattleAction action, ItemEffect itemEffect)
        {
            if (itemEffect.effectTarget == EffectTarget.EnemyAll)
            {
                foreach (var targetStatus in _enemyStatusManager.GetEnemyStatusList().ToList())
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessDamageEffect(tempAction, itemEffect));
                }
            }
            else if (itemEffect.effectTarget == EffectTarget.FriendAll)
            {
                foreach (var targetStatus in CharacterStatusManager.GetPartyMemberStatuses().ToList())
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true;
                    yield return StartCoroutine(ProcessDamageEffect(tempAction, itemEffect));
                }
            }
        }

        private IEnumerator ProcessAllItemSupportEffect(BattleAction action, ItemEffect itemEffect)
        {
            if (itemEffect.effectTarget == EffectTarget.FriendAll)
            {
                foreach (var targetStatus in CharacterStatusManager.GetPartyMemberStatuses().ToList())
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, itemEffect));
                }
            }
            else if (itemEffect.effectTarget == EffectTarget.EnemyAll)
            {
                foreach (var targetStatus in _enemyStatusManager.GetEnemyStatusList().ToList())
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, itemEffect));
                }
            }
        }
    }
}