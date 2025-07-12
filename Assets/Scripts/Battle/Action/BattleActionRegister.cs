using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のアクションを登録するクラスです。
    /// </summary>
    public class BattleActionRegister : MonoBehaviour
    {
        /// <summary>
        /// 戦闘中のアクションを処理するクラスへの参照です。
        /// </summary>
        BattleActionProcessor _actionProcessor;

        /// <summary>
        /// このクラスを初期化します。
        /// </summary>
        /// <param name="actionProcessor">戦闘中のアクションを処理するクラスへの参照</param>
        public void InitializeRegister(BattleActionProcessor actionProcessor)
        {
            _actionProcessor = actionProcessor;
        }

        /// <summary>
        /// キャラクターのパラメータレコードを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public ParameterRecord GetCharacterParameterRecord(int characterId)
        {
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
            var parameterTable = CharacterDataManager.GetParameterTable(characterId);
            var parameterRecord = parameterTable.parameterRecords.Find(p => p.level == characterStatus.level);
            return parameterRecord;
        }

        /// <summary>
        /// 攻撃コマンドのアクションをセットします。
        /// </summary>
        /// <param name="actorId">アクションを行うキャラクターのID</param>
        /// <param name="targetId">攻撃対象のキャラクターのID</param>
        public void SetFriendAttackAction(int actorId, int targetId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                targetId = targetId,
                isTargetFriend = false,
                battleCommand = BattleCommand.Attack,
                attackCommand = AttackCommand.Normal,
                actorSpeed = characterParam.dex,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 敵キャラクターの攻撃コマンドのアクションをセットします。
        /// </summary>
        /// <param name="actorId">アクションを行う敵キャラクターの戦闘中ID</param>
        /// <param name="targetId">攻撃対象のキャラクターの戦闘中ID</param>
        /// <param name="enemyData">敵キャラクターのデータ</param>
        public void SetEnemyAttackAction(int actorId, int targetId, EnemyData enemyData)
        {
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = false,
                targetId = targetId,
                isTargetFriend = true,
                battleCommand = BattleCommand.Attack,
                attackCommand = AttackCommand.Normal,
                actorSpeed = enemyData.dex,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// スキルコマンドのアクションをセットします。
        /// </summary>
        /// <param name="actorId">アクションを行うキャラクターのID</param>
        /// <param name="targetId">攻撃対象のキャラクターのID</param>
        /// <param name="skillId">スキルのID</param>
        public void SetFriendSkillAction(int actorId, int targetId, int skillId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);

            // ★ スキルデータの取得とターゲット判定
            var skillData = SkillDataManager.GetSkillDataById(skillId);
            bool isTargetFriend = false;
            if (skillData != null && skillData.skillEffect.Count > 0)
            {
                var effectTarget = skillData.skillEffect[0].effectTarget;
                if (effectTarget == EffectTarget.Own || effectTarget == EffectTarget.FriendSolo || effectTarget == EffectTarget.FriendAll)
                {
                    isTargetFriend = true;
                }
            }

            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                targetId = targetId,
                // ★ ターゲットが味方か敵かを設定
                isTargetFriend = isTargetFriend,
                battleCommand = BattleCommand.Attack,
                attackCommand = AttackCommand.Skill,
                itemId = skillId,
                actorSpeed = characterParam.dex,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 敵キャラクターのスキルコマンドのアクションをセットします。
        /// </summary>
        /// <param name="actorId">アクションを行う敵キャラクターの戦闘中ID</param>
        /// <param name="targetId">攻撃対象のキャラクターの戦闘中ID</param>
        /// <param name="skillId">スキルのID</param>
        /// <param name="enemyData">敵キャラクターのデータ</param>
        public void SetEnemySkillAction(int actorId, int targetId, int skillId, EnemyData enemyData)
        {
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = false,
                targetId = targetId,
                battleCommand = BattleCommand.Attack,
                attackCommand = AttackCommand.Skill,
                itemId = skillId,
                actorSpeed = enemyData.dex,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// アイテムコマンドのアクションをセットします。
        /// </summary>
        /// <param name="actorId">アクションを行うキャラクターのID</param>
        /// <param name="enemyBattleId">攻撃対象のキャラクターの戦闘中ID</param>
        /// <param name="itemId">アイテムのID</param>
        public void SetFriendItemAction(int actorId, int targetId, int itemId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);

            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogError($"選択されたIDのアイテムは見つかりませんでした。ID : {itemId}");
                return;
            }

            
            bool isTargetFriend = false;
            if (itemData.itemEffect.effectTarget == EffectTarget.Own
                || itemData.itemEffect.effectTarget == EffectTarget.FriendAll
                || itemData.itemEffect.effectTarget == EffectTarget.FriendSolo)
            {
                isTargetFriend = true;
            }

            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                targetId = targetId,
                isTargetFriend = isTargetFriend,
                battleCommand = BattleCommand.Item,
                itemId = itemId,
                actorSpeed = characterParam.dex,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 逃げるコマンドのアクションをセットします。
        /// </summary>
        /// <param name="actorId">アクションを行うキャラクターのID</param>
        public void SetFriendRunAction(int actorId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                battleCommand = BattleCommand.Escape,
                actorSpeed = characterParam.dex,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 防御コマンドのアクションをセットします。
        /// </summary>
        /// <param name="actorId">アクションを行うキャラクターのID</param>
        public void SetFriendGuardAction(int actorId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                battleCommand = BattleCommand.Guard,
                actorSpeed = characterParam.dex,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 敵キャラクターの防御コマンドのアクションをセットします。
        /// </summary>
        /// <param name="actorId">アクションを行う敵キャラクターの戦闘中ID</param>
        /// <param name="enemyData">敵キャラクターのデータ</param>
        public void SetEnemyGuardAction(int actorId, EnemyData enemyData)
        {
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = false,
                targetId = -1, // 防御はターゲット不要
                isTargetFriend = false,
                battleCommand = BattleCommand.Guard,
                actorSpeed = enemyData.dex, // 速度は設定するが、優先度決定ロジックで上書きされる
            };

            _actionProcessor.RegisterAction(action);
        }
    }
}