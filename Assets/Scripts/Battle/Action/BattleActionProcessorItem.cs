using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のアイテムアクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorItem : MonoBehaviour, IBattleActionProcessor
    {
        // --- 変数やSetReferencesメソッドは変更ありません ---
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

        /// <summary>
        /// アイテムのアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            var itemData = ItemDataManager.GetItemDataById(action.itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {action.itemId}");
                return;
            }

            // --- 消費アイテムの処理は変更ありません ---
            if (action.isActorFriend && itemData.itemCategory == ItemCategory.ConsumableItem)
            {
                CharacterStatusManager.UseItem(action.itemId);
            }

            // ★★★★★ここから下が修正の核心部★★★★★

            // 1. メイン処理を一時停止させる
            _actionProcessor.SetPauseProcess(true);

            // 2. 新しい、一連の処理を行うコルーチンを開始する
            StartCoroutine(ProcessItemActionCoroutine(action, itemData));
        }

        /// <summary>
        /// アイテムの一連のアクションを処理するコルーチンです。
        /// </summary>
        private IEnumerator ProcessItemActionCoroutine(BattleAction action, ItemData itemData)
        {
            // --- 1. まず「〇〇は△△を使った！」メッセージの表示が終わるのを待つ ---
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateUseItemMessage(actorName, itemData.itemName)
            );

            // --- 2. アイテムの効果に応じて、それぞれの処理を実行し、完了を待つ ---
            switch (itemData.itemEffect.itemEffectCategory)
            {
                case ItemEffectCategory.Recovery:
                    yield return StartCoroutine(ProcessRecoveryEffect(action, itemData.itemEffect));
                    break;

                // case ItemEffectCategory.Attack:
                //     yield return StartCoroutine(ProcessAttackEffect(action, itemData.itemEffect));
                //     break;

                default:
                    Debug.LogWarning($"未定義のアイテム効果カテゴリです: {itemData.itemEffect.itemEffectCategory}");
                    break;
            }

            // --- 3. すべての効果処理が終わったら、メイン処理の停止を解除 ---
            _actionProcessor.SetPauseProcess(false);
        }

        /// <summary>
        /// 回復効果の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessRecoveryEffect(BattleAction action, ItemEffect itemEffect)
        {
            // 回復量を計算し、ステータスを更新
            int healValue = BattleCalculator.CalculateHealValue(itemEffect.value);
            if (action.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.targetId, healValue, 0);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.targetId, healValue, 0);
            }

            // ステータスUIを更新し、回復メッセージの表示完了を待つ
            _battleManager.OnUpdateStatus();
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateHpHealMessage(targetName, healValue)
            );
        }
    }
}