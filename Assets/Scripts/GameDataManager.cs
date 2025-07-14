using System.Collections.Generic;
using System.Threading.Tasks; // Taskを使うために必要
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// ゲーム全体の永続的なデータを管理するクラス（シングルトン）
    /// </summary>
    public class GameDataManager : MonoBehaviour
    {
        public static GameDataManager Instance { get; private set; }

        // --- 永続データ ---
        public List<CharacterStatus> CharacterStatuses { get; private set; }
        public List<int> PartyCharacterIds { get; set; } // BattleTesterから設定できるようpublic setに変更
        public List<PartyItemInfo> PartyItems { get; set; }
        public int PartyGold { get; set; }

        // --- シーン間のデータ受け渡し用 ---
        public List<int> NextEncounterEnemyIds { get; set; }

        // ★★★ エラーの原因だった IsDataInitialized プロパティを追加 ★★★
        public bool IsDataInitialized { get; private set; } = false;

        private bool _isInitializing = false; // 重複実行防止用のフラグ

        // Awakeの戻り値の型を async void に変更
        private async void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                await InitializeAllData(); // 全てのデータを非同期でロードする
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// ゲームに必要な全てのデータをロードし、プレイヤーデータを初期化します。
        /// </summary>
        public async Task InitializeAllData()
        {
            if (IsDataInitialized || _isInitializing) return;
            _isInitializing = true;
            Debug.Log("GameDataManager: 全データロードを開始します...");

            await CharacterDataManager.LoadExpTables();
            await CharacterDataManager.LoadParameterTables();
            await CharacterDataManager.LoadCharacterData();
            await EnemyDataManager.LoadEnemyData();
            await ItemDataManager.LoadItemData();
            await SkillDataManager.LoadSkillData();

            Debug.Log("GameDataManager: 全データロード完了。");

            InitializeNewGameData();
            _isInitializing = false;
            IsDataInitialized = true;
        }

        /// <summary>
        /// 新規ゲーム開始時のデータ初期化処理
        /// </summary>
        private void InitializeNewGameData()
        {
            // このメソッドの中身は変更なし
            CharacterStatuses = new List<CharacterStatus>();
            PartyCharacterIds = new List<int> { 1, 2, 3, 4 };
            PartyItems = new List<PartyItemInfo>
            {
                new PartyItemInfo { itemId = 1, itemNum = 5 },
                new PartyItemInfo { itemId = 2, itemNum = 3 }
            };
            PartyGold = 500;

            int[] allCharacterIds = { 1, 2, 3, 4 };
            foreach (var id in allCharacterIds)
            {
                var characterData = CharacterDataManager.GetCharacterData(id);
                var parameterTable = CharacterDataManager.GetParameterTable(id);
                if (characterData == null || parameterTable == null) continue;

                int initialLevel = 1;
                var paramRecord = parameterTable.parameterRecords.Find(r => r.level == initialLevel);
                if (paramRecord == null) continue;

                var expTable = CharacterDataManager.GetExpTable();
                var expRecord = expTable.expRecords.Find(r => r.level == initialLevel);

                CharacterStatus status = new()
                {
                    characterId = id,
                    level = initialLevel,
                    exp = expRecord?.exp ?? 0,
                    currentHp = paramRecord.hp,
                    currentBt = paramRecord.bt,
                    equipWeaponId = 0,
                    equipArmorId = 0,
                    SkillList = CharacterDataManager.GetLearnableSkill(id, initialLevel).ConvertAll(s => s.skillId),
                    attribute = characterData.attribute
                };
                CharacterStatuses.Add(status);
            }
        }
    }
}