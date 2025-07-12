using System.Collections;
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

            var itemEffect = itemData.itemEffect;
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

                default:
                    Debug.LogWarning($"未定義のアイテム効果カテゴリです: {itemEffect.itemEffectCategory}");
                    break;
            }

            _actionProcessor.SetPauseProcess(false);
        }

        /// <summary>
        /// 単体への回復効果を処理します。
        /// </summary>
        private IEnumerator ProcessRecoveryEffect(BattleAction action, ItemEffect itemEffect)
        {
            int healValue = BattleCalculator.CalculateHealValue(itemEffect.value);

            if (action.isTargetFriend)
                CharacterStatusManager.ChangeCharacterStatus(action.targetId, healValue, 0);
            else
                _enemyStatusManager.ChangeEnemyStatus(action.targetId, healValue, 0);

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyStatusUI();
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateHpHealMessage(targetName, healValue)
            );
        }

        /// <summary>
        /// ★【新規】単体へのダメージ効果を処理します。
        /// </summary>
        private IEnumerator ProcessDamageEffect(BattleAction action, ItemEffect itemEffect)
        {
            var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);
            int damage = BattleCalculator.CalculateDamage(itemEffect.value, targetParam.def, targetParam.isGuarding);
            int hpDelta = damage * -1;

            if (action.isTargetFriend)
                CharacterStatusManager.ChangeCharacterStatus(action.targetId, hpDelta, 0);
            else
                _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, 0);

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyStatusUI();
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateDamageMessage(targetName, damage)
            );
        }

        /// <summary>
        /// ★【新規】単体への補助効果を処理します。
        /// </summary>
        private IEnumerator ProcessSupportEffect(BattleAction action, ItemEffect itemEffect)
        {
            if (itemEffect.duration > -1)
            {
                var newBuff = new AppliedBuff
                {
                    parameter = itemEffect.skillParameter,
                    value = itemEffect.value,
                    duration = itemEffect.duration
                };

                if (action.isTargetFriend)
                {
                    var status = CharacterStatusManager.GetCharacterStatusById(action.targetId);
                    if (status != null) status.buffs.Add(newBuff);
                }
                else
                {
                    var status = _enemyStatusManager.GetEnemyStatusByBattleId(action.targetId);
                    if (status != null) status.buffs.Add(newBuff);
                }
            }

            _battleManager.OnUpdateStatus();
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateSupportEffectMessage(targetName, itemEffect.skillParameter, itemEffect.value)
            );
        }

        /// <summary>
        /// 全体への回復効果を処理します。
        /// </summary>
        private IEnumerator ProcessAllItemRecoveryEffect(BattleAction action, ItemEffect itemEffect)
        {
            if (itemEffect.effectTarget == EffectTarget.FriendAll)
            {
                var targetList = CharacterStatusManager.GetPartyMemberStatuses();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true;
                    yield return StartCoroutine(ProcessRecoveryEffect(tempAction, itemEffect));
                }
            }
            else if (itemEffect.effectTarget == EffectTarget.EnemyAll)
            {
                var targetList = _enemyStatusManager.GetEnemyStatusList();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessRecoveryEffect(tempAction, itemEffect));
                }
            }
        }

        /// <summary>
        /// ★【新規】全体へのダメージ効果を処理します。
        /// </summary>
        private IEnumerator ProcessAllItemDamageEffect(BattleAction action, ItemEffect itemEffect)
        {
            if (itemEffect.effectTarget == EffectTarget.FriendAll)
            {
                var targetList = CharacterStatusManager.GetPartyMemberStatuses();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true;
                    yield return StartCoroutine(ProcessDamageEffect(tempAction, itemEffect));
                }
            }
            else if (itemEffect.effectTarget == EffectTarget.EnemyAll)
            {
                var targetList = _enemyStatusManager.GetEnemyStatusList();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessDamageEffect(tempAction, itemEffect));
                }
            }
        }

        /// <summary>
        /// ★【新規】全体への補助効果を処理します。
        /// </summary>
        private IEnumerator ProcessAllItemSupportEffect(BattleAction action, ItemEffect itemEffect)
        {
            if (itemEffect.effectTarget == EffectTarget.FriendAll)
            {
                var targetList = CharacterStatusManager.GetPartyMemberStatuses();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, itemEffect));
                }
            }
            else if (itemEffect.effectTarget == EffectTarget.EnemyAll)
            {
                var targetList = _enemyStatusManager.GetEnemyStatusList();
                foreach (var targetStatus in targetList)
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, itemEffect));
                }
            }
        }
    }
}