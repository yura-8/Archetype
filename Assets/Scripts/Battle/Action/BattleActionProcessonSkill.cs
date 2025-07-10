using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のスキルアクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorSkill : MonoBehaviour, IBattleActionProcessor
    {
        // --- 変数やSetReferencesメソッドは変更ありません ---
        private BattleActionProcessor _actionProcessor;
        private BattleManager _battleManager;
        private MessageWindowController _messageWindowController;
        private EnemyStatusManager _enemyStatusManager;

        // --- 独自のbool _pauseSkillEffect; は不要になるため削除します ---

        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        }

        /// <summary>
        /// スキルのアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            var skillData = SkillDataManager.GetSkillDataById(action.itemId);
            if (skillData == null)
            {
                Debug.LogError($"スキルデータが見つかりません。ID: {action.itemId}");
                return;
            }

            // --- 消費MPの処理は変更ありません ---
            int hpDelta = 0;
            int mpDelta = skillData.cost * -1;
            if (action.isActorFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);
            }

            // ★★★★★ここから下が修正の核心部★★★★★

            // 1. メイン処理を一時停止させる
            _actionProcessor.SetPauseProcess(true);

            // 2. 新しい、一連の処理を行うコルーチンを開始する
            StartCoroutine(ProcessSkillActionCoroutine(action, skillData));
        }

        /// <summary>
        /// スキルの一連のアクションを処理するコルーチンです。
        /// </summary>
        private IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
        {
            // --- 1. まず「〇〇は△△をとなえた！」メッセージの表示が終わるのを待つ ---
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateSkillCastMessage(actorName, skillData.skillName)
            );

            // --- 2. スキルが持つ効果を一つずつ順番に処理する ---
            foreach (var skillEffect in skillData.skillEffect)
            {
                // 現在のスキル効果のターゲットを決定する
                var effectTargetAction = DetermineEffectTarget(action, skillEffect);

                // --- 3. スキルの種類に応じて、それぞれの処理を実行し、完了を待つ ---
                switch (skillEffect.skillCategory)
                {
                    case SkillCategory.Recovery:
                        yield return StartCoroutine(ProcessRecoveryEffect(effectTargetAction, skillEffect));
                        break;

                    // case SkillCategory.Attack:
                    //     yield return StartCoroutine(ProcessAttackEffect(effectTargetAction, skillEffect));
                    //     break;

                    default:
                        Debug.LogWarning($"未定義のスキル効果カテゴリです: {skillEffect.skillCategory}");
                        break;
                }
            }

            // --- 4. すべての効果処理が終わったら、メイン処理の停止を解除 ---
            _actionProcessor.SetPauseProcess(false);
        }

        /// <summary>
        /// 回復効果の処理とメッセージ表示を行います。
        /// </summary>
        private IEnumerator ProcessRecoveryEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            // 回復量を計算し、ステータスを更新
            int hpDelta = BattleCalculator.CalculateHealValue(skillEffect.value);
            if (effectTargetAction.isTargetFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(effectTargetAction.targetId, hpDelta, 0);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(effectTargetAction.targetId, hpDelta, 0);
            }

            // ステータスUIを更新し、回復メッセージの表示完了を待つ
            _battleManager.OnUpdateStatus();
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateHpHealMessage(targetName, hpDelta)
            );
        }

        /// <summary>
        /// スキル効果の対象を決定します。
        /// </summary>
        private BattleAction DetermineEffectTarget(BattleAction originalAction, SkillEffect skillEffect)
        {
            // 元のアクション情報をコピーして、ターゲット情報だけを上書きできるようにする
            BattleAction effectTargetAction = originalAction.Clone();

            bool isTargetFriend = (skillEffect.effectTarget == EffectTarget.Own ||
                                   skillEffect.effectTarget == EffectTarget.FriendSolo ||
                                   skillEffect.effectTarget == EffectTarget.FriendAll);

            if (isTargetFriend)
            {
                // 自分自身や味方が対象の場合
                effectTargetAction.isTargetFriend = true;
                // "自分自身" を解決する
                if (skillEffect.effectTarget == EffectTarget.Own)
                {
                    effectTargetAction.targetId = originalAction.actorId;
                }
                // （注：もし全体化スキルを作る場合は、ここでターゲットIDのリストを扱う処理が必要）
            }
            else
            {
                // 敵が対象の場合
                effectTargetAction.isTargetFriend = false;
            }

            return effectTargetAction;
        }
    }
}