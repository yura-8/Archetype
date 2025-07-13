using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘機能のテストを行うためのクラスです。
    /// </summary>
    public class BattleTester : MonoBehaviour
    {
        /// <summary>
        /// 戦闘機能を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleManager _battleManager;

        [Header("テスト用の設定")]
        /// <summary>
        /// 戦う対象の敵キャラクターのIDです。
        /// </summary>
        //[SerializeField]
        List<int> _enemyId = new List<int> { 1, 2, 1, 1 };

        /// <summary>
        /// 味方キャラクターのレベルです。
        /// </summary>
        [SerializeField]
        int _playerLevel;

        /// <summary>
        /// 装備中の武器のIDです。
        /// </summary>
        [SerializeField]
        int _weaponId;

        /// <summary>
        /// 装備中の防具のIDです。
        /// </summary>
        [SerializeField]
        int _armorId;

        /// <summary>
        /// アイテム所持数の設定です。
        /// </summary>
        [SerializeField]
        List<PartyItemInfo> _partyItemInfoList = new();

        [Header("戦闘機能をテストする")]
        /// <summary>
        /// 戦闘機能をテストするフラグです。
        /// Inspectorウィンドウからチェックを入れると、戦闘機能をテストします。
        /// </summary>
        [SerializeField]
        bool _executeBattle;

        private static bool _isGameInitialized = false;

        void Update()
        {
            CheckStartFlag();
        }

        /// <summary>
        /// 戦闘機能をテストするフラグの状態をチェックします。
        /// </summary>
        void CheckStartFlag()
        {
            // 定義データのロードを待つため、最初の5フレームは処理を抜けます。
            if (Time.frameCount < 5)
            {
                return;
            }

            if (!_executeBattle)
            {
                return;
            }

            _executeBattle = false;

            // 戦闘中でない場合のみ、戦闘を開始します。
            if (_battleManager.BattlePhase != BattlePhase.NotInBattle)
            {
                SimpleLogger.Instance.Log("戦闘中のためBattleTesterの処理を抜けます。");
                return;
            }

            ReadyForBattle();
        }

        /// <summary>
        /// 戦闘を開始する準備を行います。
        /// </summary>
        void ReadyForBattle()
        {
            SetPlayerStatus();
            SetEnemyId();
            StartBattle();
        }

        /// <summary>
        /// 味方キャラクターのステータスをセットします。
        /// </summary>
        void SetPlayerStatus()
        {
            // もしゲームの初期設定が完了しているなら、
            // 何もせずにこのメソッドを抜けます。
            if (_isGameInitialized)
            {
                // これにより、2回目以降のステータス上書きを防ぎます。
                return;
            }

            // --- ここから下は、初回実行時のみ通る処理 ---

            SimpleLogger.Instance.Log("初回設定：キャラクターのステータスを生成します。");

            // 経験値表を使って、レベルから経験値を取得します。
            var expTable = CharacterDataManager.GetExpTable();
            var expRecord = expTable.expRecords.Find(record => record.level == _playerLevel);
            var exp = expRecord.exp;

            // レベルに対応するパラメータデータを取得します。
            int charcterId = 1;
            var parameterTable = CharacterDataManager.GetParameterTable(charcterId);
            if (expRecord == null)
            {
                Debug.LogError($"ExpTable にレベル {_playerLevel} がありません");
                return;
            }

            var parameterRecord = parameterTable.parameterRecords.Find(record => record.level == _playerLevel);
            if (parameterRecord == null)
            {
                Debug.LogError($"ParameterTable にレベル {_playerLevel} がありません");
                return;
            }

            var skillList = GetSkillIdList(charcterId, _playerLevel);

            CharacterStatus status = new()
            {
                characterId = charcterId,
                level = _playerLevel,
                exp = exp,
                currentHp = parameterRecord.hp,
                currentBt = parameterRecord.bt,
                equipWeaponId = _weaponId,
                equipArmorId = _armorId,
                SkillList = skillList,
            };

            CharacterStatusManager.characterStatuses = new()
            {
                status
            };
            CharacterStatusManager.partyCharacter = new()
            {
                charcterId
            };

            charcterId = 2;
            parameterTable = CharacterDataManager.GetParameterTable(charcterId);
            parameterRecord = parameterTable.parameterRecords.Find(record => record.level == _playerLevel);
            skillList = GetSkillIdList(charcterId, _playerLevel);

            status = new()
            {
                characterId = charcterId,
                level = _playerLevel,
                exp = exp,
                currentHp = parameterRecord.hp,
                currentBt = parameterRecord.bt,
                equipWeaponId = _weaponId,
                equipArmorId = _armorId,
                SkillList = skillList,
            };

            CharacterStatusManager.characterStatuses.Add(status);
            CharacterStatusManager.partyCharacter.Add(charcterId);
            CharacterStatusManager.partyItemInfoList = _partyItemInfoList;

            // すべての初期設定が終わったら、フラグを true にします。
            // これで、次回以降はこの処理がスキップされるようになります。
            _isGameInitialized = true;
        }

        /// <summary>
        /// キャラクターが覚えているスキルのIDリストを返します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        /// <param name="level">キャラクターのレベル</param>
        List<int> GetSkillIdList(int characterId, int level)
        {
            var learnableSkill = CharacterDataManager.GetLearnableSkill(characterId, level);
            List<int> skillList = new();
            foreach (var record in learnableSkill)
            {
                skillList.Add(record.skillId);
            }
            return skillList;
        }

        /// <summary>
        /// 敵キャラクターのIDをセットします。
        /// </summary>
        void SetEnemyId()
        {
            //_battleManager.SetUpEnemyStatus(_enemyId);


            // 1. 出現する敵のIDを格納するための、新しい空のリストを作成します。
            List<int> randomEnemyList = new List<int>();

            // 2. 出現させる敵の数を、1～4体の間でランダムに決定します。
            int enemyCount = Random.Range(1, 5); // 1, 2, 3, 4 のいずれかの数がランダムで選ばれます。

            // 3. 決まった数だけループ処理を行います。
            for (int i = 0; i < enemyCount; i++)
            {
                // 4. 敵の種類（ID）を、1～5の中からランダムに選びます。
                int randomEnemyId = Random.Range(1, 3); // 1, 2, 3, 4, 5 のいずれかの数がランダムで選ばれます。
                
                // 5. 選ばれた敵IDをリストに追加します。
                randomEnemyList.Add(randomEnemyId);
            }

            // --- ここまでが修正部分 ---

            // 6. 完成したランダムな敵リストを使って、戦闘のセットアップを依頼します。
            _battleManager.SetUpEnemyStatus(randomEnemyList);
        }

        /// <summary>
        /// 戦闘を開始します。
        /// </summary>
        void StartBattle()
        {
            _battleManager.StartBattle();
        }
    }
}