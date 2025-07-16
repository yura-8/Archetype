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
            // アイテムの基本回復量
            int baseHealAmount = itemEffect.value;
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            // 属性と温度を取得
            var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);
            var currentTemp = _battleManager.CurrentTemperature;

            // 回復量補正係数を計算
            float recoveryModifier = 1.0f;

            // Pulse属性の回復量増加効果を、HP・BT問わず適用する
            if (targetParam.attribute == ElementAttribute.Pulse)
            {
                recoveryModifier *= 1.1f; // Pulseは常に10%増
            }

            // 温度と他属性によるBT被回復量補正 (BT回復の場合のみ適用)
            if (itemEffect.skillParameter == SkillParameter.bt)
            {
                if (currentTemp == TemperatureState.HOT)
                {
                    if (targetParam.attribute == ElementAttribute.Plasma) recoveryModifier *= 1.4f;
                    else if (targetParam.attribute == ElementAttribute.Cryo) recoveryModifier *= 1.15f;
                }
                else if (currentTemp == TemperatureState.COLD)
                {
                    if (targetParam.attribute == ElementAttribute.Plasma) recoveryModifier *= 0.9f;
                    else if (targetParam.attribute == ElementAttribute.Cryo) recoveryModifier *= 0.7f;
                }
            }

            // 最終的な回復量を計算
            int finalHealAmount = (int)(baseHealAmount * recoveryModifier);

            // 回復量をHPまたはBTの変動量に設定
            if (itemEffect.skillParameter == SkillParameter.hp)
            {
                hpDelta = finalHealAmount;
            }
            else if (itemEffect.skillParameter == SkillParameter.bt)
            {
                btDelta = finalHealAmount;
            }

            // ステータスを更新
            if (action.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.targetId, hpDelta, btDelta);
            }
            else
            {
                var status = _enemyStatusManager.GetEnemyStatusByBattleId(action.targetId);
                if (status == null || status.isDefeated) yield break;
                _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, btDelta);
            }

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();

            // メッセージを表示
            if (itemEffect.skillParameter == SkillParameter.hp)
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateHpHealMessage(targetName, finalHealAmount));
            else if (itemEffect.skillParameter == SkillParameter.bt)
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateBtHealMessage(targetName, finalHealAmount));
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