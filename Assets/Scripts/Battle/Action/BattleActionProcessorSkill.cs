using System.Collections;
using System.Linq;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のスキルアクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorSkill : MonoBehaviour, IBattleActionProcessor
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
            var skillData = SkillDataManager.GetSkillDataById(action.itemId);
            if (skillData == null)
            {
                Debug.LogError($"スキルデータが見つかりません。ID: {action.itemId}");
                return;
            }

            // 温度と属性に応じたBT消費量補正
            var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
            float costMultiplier = 1.0f;
            if (_battleManager.CurrentTemperature == TemperatureState.HOT)
            {
                // デフォルトのHOT補正は1.5倍
                costMultiplier = 1.5f;
                // 属性による上書き
                if (actorParam.attribute == ElementAttribute.Plasma) costMultiplier = 1.8f; // 80%増
                if (actorParam.attribute == ElementAttribute.Cryo) costMultiplier = 1.2f; // 20%増
            }
            int actualCost = (int)(skillData.cost * costMultiplier);
            if (action.isActorFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, 0, actualCost * -1);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, 0, actualCost * -1);
            }

            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ProcessSkillActionCoroutine(action, skillData));
        }

        private IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateSkillCastMessage(actorName, skillData.skillName)
            );

            foreach (var skillEffect in skillData.skillEffect)
            {
                bool isAllTarget = skillEffect.effectTarget == EffectTarget.FriendAll || skillEffect.effectTarget == EffectTarget.EnemyAll;

                switch (skillEffect.skillCategory)
                {
                    case SkillCategory.Recovery:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllRecoveryEffect(action, skillEffect));
                        else
                            yield return StartCoroutine(ProcessRecoveryEffect(DetermineEffectTarget(action, skillEffect), skillEffect));
                        break;
                    case SkillCategory.Damage:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllDamageEffect(action, skillEffect));
                        else
                            yield return StartCoroutine(ProcessDamageEffect(DetermineEffectTarget(action, skillEffect), skillEffect));
                        break;
                    case SkillCategory.Support:
                        if (isAllTarget)
                            yield return StartCoroutine(ProcessAllSupportEffect(action, skillEffect));
                        else
                            yield return StartCoroutine(ProcessSupportEffect(DetermineEffectTarget(action, skillEffect), skillEffect));
                        break;
                    case SkillCategory.Temperature:
                        yield return StartCoroutine(ProcessTemperatureEffect(skillEffect));
                        break;
                    default:
                        Debug.LogWarning($"未定義または移動スキルです: {skillEffect.skillCategory}");
                        yield return null;
                        break;
                }
                if (_battleManager.IsBattleFinished)
                {
                    yield break;
                }
            }
            _actionProcessor.SetPauseProcess(false);
        }


        //回復処理 (Recovery)
        private IEnumerator ProcessRecoveryEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            int hpDelta = 0;
            int btDelta = 0;
            int baseHealAmount = 0;
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);
            var targetParam = _actionProcessor.GetCharacterParameter(effectTargetAction.targetId, effectTargetAction.isTargetFriend);
            var currentTemp = _battleManager.CurrentTemperature;

            if (effectTargetAction.isTargetFriend)
            {
                var status = CharacterStatusManager.GetCharacterStatusById(effectTargetAction.targetId);
                var param = CharacterDataManager.GetParameterTable(status.characterId).parameterRecords.Find(p => p.level == status.level);

                if (skillEffect.skillParameter == SkillParameter.hp)
                {
                    baseHealAmount = (int)(param.hp * skillEffect.value);
                }
                else if (skillEffect.skillParameter == SkillParameter.bt)
                {
                    int maxBt = param.bt - status.maxBtPenalty;
                    baseHealAmount = (int)(maxBt * skillEffect.value);
                }
            }
            else // 敵への回復
            {
                var status = _enemyStatusManager.GetEnemyStatusByBattleId(effectTargetAction.targetId);
                if (status == null || status.isDefeated) yield break;
                var param = status.enemyData;

                if (skillEffect.skillParameter == SkillParameter.hp)
                {
                    baseHealAmount = (int)(param.hp * skillEffect.value);
                }
                else if (skillEffect.skillParameter == SkillParameter.bt)
                {
                    int maxBt = param.bt - status.maxBtPenalty;
                    baseHealAmount = (int)(maxBt * skillEffect.value);
                }
            }

            float recoveryModifier = 1.0f;

            // Pulse属性の回復量増加効果を、HP・BT問わず適用する
            if (targetParam.attribute == ElementAttribute.Pulse)
            {
                recoveryModifier *= 1.1f; // Pulseは常に10%増
            }

            // 温度と他属性によるBT被回復量補正 (BT回復の場合のみ適用)
            if (skillEffect.skillParameter == SkillParameter.bt)
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

            int finalHealAmount = (int)(baseHealAmount * recoveryModifier);

            if (skillEffect.skillParameter == SkillParameter.hp) hpDelta = finalHealAmount;
            else if (skillEffect.skillParameter == SkillParameter.bt) btDelta = finalHealAmount;

            if (effectTargetAction.isTargetFriend)
                CharacterStatusManager.ChangeCharacterStatus(effectTargetAction.targetId, hpDelta, btDelta);
            else
                _enemyStatusManager.ChangeEnemyStatus(effectTargetAction.targetId, hpDelta, btDelta);

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();

            if (skillEffect.skillParameter == SkillParameter.hp)
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateHpHealMessage(targetName, finalHealAmount));
            else if (skillEffect.skillParameter == SkillParameter.bt)
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateBtHealMessage(targetName, finalHealAmount));
        }

        private IEnumerator ProcessAllRecoveryEffect(BattleAction action, SkillEffect skillEffect)
        {
            // ★ ターゲットの所属を正しく判定
            var tempActionForAll = DetermineEffectTarget(action, skillEffect);

            // ★ プレイヤーサイドへの全体回復
            if (tempActionForAll.isTargetFriend)
            {
                foreach (var targetStatus in CharacterStatusManager.GetPartyMemberStatuses().ToList())
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true; // 対象は味方
                    yield return StartCoroutine(ProcessRecoveryEffect(tempAction, skillEffect));
                }
            }
            // ★ 敵サイドへの全体回復
            else
            {
                foreach (var targetStatus in _enemyStatusManager.GetEnemyStatusList().ToList())
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false; // 対象は敵
                    yield return StartCoroutine(ProcessRecoveryEffect(tempAction, skillEffect));
                }
            }
        }


        #region ダメージ処理 (Damage)
        private IEnumerator ProcessDamageEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            var actorParam = _actionProcessor.GetCharacterParameter(effectTargetAction.actorId, effectTargetAction.isActorFriend);
            var targetParam = _actionProcessor.GetCharacterParameter(effectTargetAction.targetId, effectTargetAction.isTargetFriend);

            int totalPower = (int)(actorParam.atk * (1 + skillEffect.value));

            // 属性相性を考慮したダメージ計算
            int baseDamage = BattleCalculator.CalculateDamage(totalPower, targetParam.def, targetParam.isGuarding);
            float attributeModifier = GetAttributeModifier(actorParam.attribute, targetParam.attribute);
            int finalDamage = Mathf.Max(1, (int)(baseDamage * attributeModifier));

            int hpDelta = finalDamage * -1;
            bool isTargetDefeated = false;
            SpecialStatusType newStatus;

            if (effectTargetAction.isTargetFriend)
            {
                newStatus = CharacterStatusManager.ChangeCharacterStatus(effectTargetAction.targetId, hpDelta, 0);
                isTargetDefeated = CharacterStatusManager.IsCharacterDefeated(effectTargetAction.targetId);
            }
            else
            {
                newStatus = _enemyStatusManager.ChangeEnemyStatus(effectTargetAction.targetId, hpDelta, 0);
                isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(effectTargetAction.targetId);
                if (isTargetDefeated) _enemyStatusManager.OnDefeatEnemy(effectTargetAction.targetId);
            }

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);

            yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateDamageMessage(targetName, finalDamage));

            if (newStatus != SpecialStatusType.None)
            {
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateStatusEffectMessage(targetName, newStatus));
            }

            if (isTargetDefeated)
            {
                _battleManager.UpdateEnemyVisuals();
                if (effectTargetAction.isTargetFriend)
                    yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateDefeateFriendMessage(targetName));
                else
                    yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateDefeateEnemyMessage(targetName));
            }
        }

        private IEnumerator ProcessAllDamageEffect(BattleAction action, SkillEffect skillEffect)
        {
            // 全体ダメージは敵→味方、味方→敵のみを想定
            if (skillEffect.effectTarget == EffectTarget.FriendAll)
            {
                foreach (var targetStatus in CharacterStatusManager.GetPartyMemberStatuses().ToList())
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true;
                    yield return StartCoroutine(ProcessDamageEffect(tempAction, skillEffect));
                }
            }
            else if (skillEffect.effectTarget == EffectTarget.EnemyAll)
            {
                foreach (var targetStatus in _enemyStatusManager.GetEnemyStatusList().ToList())
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessDamageEffect(tempAction, skillEffect));
                }
            }
        }
        #endregion

        // 属性相性係数を返すメソッド
        private float GetAttributeModifier(ElementAttribute attacker, ElementAttribute defender)
        {
            if (attacker == ElementAttribute.None || defender == ElementAttribute.None) return 1.0f;

            if ((attacker == ElementAttribute.Plasma && defender == ElementAttribute.Cryo) ||
                (attacker == ElementAttribute.Cryo && defender == ElementAttribute.Pulse) ||
                (attacker == ElementAttribute.Pulse && defender == ElementAttribute.Plasma))
            {
                return 1.1f; // 有利
            }

            if ((attacker == ElementAttribute.Cryo && defender == ElementAttribute.Plasma) ||
                (attacker == ElementAttribute.Pulse && defender == ElementAttribute.Cryo) ||
                (attacker == ElementAttribute.Plasma && defender == ElementAttribute.Pulse))
            {
                return 0.9f; // 不利
            }

            return 1.0f; // 等倍
        }

        #region 補助処理 (Support)
        private IEnumerator ProcessSupportEffect(BattleAction effectTargetAction, SkillEffect skillEffect)
        {
            int buffValue = 0;
            // 基礎パラメータを取得
            if (effectTargetAction.isTargetFriend)
            {
                var status = CharacterStatusManager.GetCharacterStatusById(effectTargetAction.targetId);
                var param = CharacterDataManager.GetParameterTable(status.characterId).parameterRecords.Find(p => p.level == status.level);
                switch (skillEffect.skillParameter)
                {
                    case SkillParameter.atk: buffValue = (int)(param.atk * skillEffect.value); break;
                    case SkillParameter.def: buffValue = (int)(param.def * skillEffect.value); break;
                    case SkillParameter.dex: buffValue = (int)(param.dex * skillEffect.value); break;
                }
            }
            else
            {
                var status = _enemyStatusManager.GetEnemyStatusByBattleId(effectTargetAction.targetId);
                switch (skillEffect.skillParameter)
                {
                    case SkillParameter.atk: buffValue = (int)(status.enemyData.atk * skillEffect.value); break;
                    case SkillParameter.def: buffValue = (int)(status.enemyData.def * skillEffect.value); break;
                    case SkillParameter.dex: buffValue = (int)(status.enemyData.dex * skillEffect.value); break;
                }
            }

            // バフを適用
            var newBuff = new AppliedBuff
            {
                parameter = skillEffect.skillParameter,
                value = buffValue, // 計算した効果量を設定
                duration = skillEffect.duration
            };
            if (effectTargetAction.isTargetFriend)
                CharacterStatusManager.GetCharacterStatusById(effectTargetAction.targetId)?.buffs.Add(newBuff);
            else
                _enemyStatusManager.GetEnemyStatusByBattleId(effectTargetAction.targetId)?.buffs.Add(newBuff);

            _battleManager.OnUpdateStatus();
            _battleManager.UpdateEnemyVisuals();
            string targetName = _actionProcessor.GetCharacterName(effectTargetAction.targetId, effectTargetAction.isTargetFriend);
            yield return _messageWindowController.StartCoroutine(
                _messageWindowController.GenerateSupportEffectMessage(targetName, skillEffect.skillParameter, buffValue)
            );
        }

        private IEnumerator ProcessAllSupportEffect(BattleAction action, SkillEffect skillEffect)
        {
            // ★ ターゲットの所属を正しく判定
            var tempActionForAll = DetermineEffectTarget(action, skillEffect);

            // ★ プレイヤーサイドへの全体補助
            if (tempActionForAll.isTargetFriend)
            {
                foreach (var targetStatus in CharacterStatusManager.GetPartyMemberStatuses().ToList())
                {
                    if (targetStatus.currentHp <= 0) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.characterId;
                    tempAction.isTargetFriend = true;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, skillEffect));
                }
            }
            // ★ 敵サイドへの全体補助
            else
            {
                foreach (var targetStatus in _enemyStatusManager.GetEnemyStatusList().ToList())
                {
                    if (targetStatus.isDefeated || targetStatus.isRunaway) continue;
                    var tempAction = action.Clone();
                    tempAction.targetId = targetStatus.enemyBattleId;
                    tempAction.isTargetFriend = false;
                    yield return StartCoroutine(ProcessSupportEffect(tempAction, skillEffect));
                }
            }
        }
        #endregion

        #region 温度変化処理 (Temperature) - 新規
        private IEnumerator ProcessTemperatureEffect(SkillEffect skillEffect)
        {
            TemperatureState newState = _battleManager.CurrentTemperature;
            string message = "";

            // skillEffect.valueに応じて温度を決定 (例: 1=HOT, -1=COLD)
            if (skillEffect.value > 0)
            {
                newState = TemperatureState.HOT;
                message = "スキルによって戦場の気温が急上昇した！";
            }
            else if (skillEffect.value < 0)
            {
                newState = TemperatureState.COLD;
                message = "スキルによってあたりが急速に冷却された…";
            }
            else
            {
                newState = TemperatureState.NORMAL;
                message = "スキルによって気温が平常に戻った。";
            }

            // 現在の温度と同じなら何もしない
            if (newState == _battleManager.CurrentTemperature)
            {
                yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateGeneralMessage("しかし何も変わらなかった…"));
                yield break;
            }

            _battleManager.ChangeTemperature(newState);
            yield return _messageWindowController.StartCoroutine(_messageWindowController.GenerateGeneralMessage(message));
        }
        #endregion

        // 術者が誰であってもターゲットの所属を正しく判定するように修正
        private BattleAction DetermineEffectTarget(BattleAction originalAction, SkillEffect skillEffect)
        {
            BattleAction effectTargetAction = originalAction.Clone();

            // isTargetOnSameSide: ターゲットが術者と同じサイドか (true) か、反対のサイドか (false) か
            bool isTargetOnSameSide;
            switch (skillEffect.effectTarget)
            {
                case EffectTarget.Own:
                case EffectTarget.FriendSolo:
                case EffectTarget.FriendAll:
                    isTargetOnSameSide = true;
                    break;
                case EffectTarget.EnemySolo:
                case EffectTarget.EnemyAll:
                    isTargetOnSameSide = false;
                    break;
                default:
                    // 不明なターゲットタイプは、とりあえず反対サイド（敵対行動）とみなす
                    isTargetOnSameSide = false;
                    break;
            }

            // isActorFriend: 術者がプレイヤーサイドか (true)
            // isTargetFriend: ターゲットがプレイヤーサイドか (true)
            //
            // パターン:
            // 術者(Player)が味方(SameSide)へ -> isTargetFriend = true  (true == true)
            // 術者(Player)が敵(Opposite)へ -> isTargetFriend = false (true != true)
            // 術者(Enemy) が味方(SameSide)へ -> isTargetFriend = false (false == false)
            // 術者(Enemy) が敵(Opposite)へ -> isTargetFriend = true  (false != true)
            //
            // 法則: isTargetFriend = (originalAction.isActorFriend == isTargetOnSameSide)
            //
            // ★ただし、このプロジェクトの isTargetFriend は「プレイヤーサイドかどうか」ではなく、
            // 「術者にとって味方かどうか」で判定されている箇所があるため、元のロジックを尊重しつつ拡張します。
            // isTargetFriend は「プレイヤーサイドかどうか」を意味するものとして統一します。

            if (isTargetOnSameSide)
            {
                // ターゲットは術者と同じサイド
                // 術者がプレイヤーならターゲットもプレイヤー(true)、術者が敵ならターゲットも敵(false)
                effectTargetAction.isTargetFriend = originalAction.isActorFriend;
            }
            else
            {
                // ターゲットは術者と反対のサイド
                // 術者がプレイヤーならターゲットは敵(false)、術者が敵ならターゲットはプレイヤー(true)
                effectTargetAction.isTargetFriend = !originalAction.isActorFriend;
            }

            // Own の場合はターゲットIDを術者IDに設定する
            if (skillEffect.effectTarget == EffectTarget.Own)
            {
                effectTargetAction.targetId = originalAction.actorId;
            }

            return effectTargetAction;
        }
    }
}