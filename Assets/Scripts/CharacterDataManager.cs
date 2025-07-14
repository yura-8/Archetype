using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内の味方キャラクターのデータを管理するクラスです。
    /// </summary>
    public static class CharacterDataManager
    {
        /// <summary>
        /// 読み込んだキャラクターの経験値表の一覧です。
        /// </summary>
        static List<ExpTable> _expTables = new();

        /// <summary>
        /// 読み込んだキャラクターのパラメータ表の一覧です。
        /// </summary>
        static List<ParameterTable> _parameterTables = new();

        /// <summary>
        /// 読み込んだキャラクターのデータの一覧です。
        /// </summary>
        static List<CharacterData> _characterDataList = new();

        /// <summary>
        /// 経験値表のデータをロードします。
        /// </summary>
        public static async Task LoadExpTables()
        {
            AsyncOperationHandle<IList<ExpTable>> handle = Addressables.LoadAssetsAsync<ExpTable>(AddressablesLabels.ExpTable, null);
            await handle.Task;
            _expTables = new List<ExpTable>(handle.Result);
            handle.Release();
        }

        /// <summary>
        /// 経験値表のデータを取得します。
        /// </summary>
        public static ExpTable GetExpTable()
        {
            ExpTable expTable = null;
            if (_expTables.Count > 0)
            {
                expTable = _expTables[0];
            }
            return expTable;
        }

        /// <summary>
        /// パラメータ表のデータをロードします。
        /// </summary>
        public static async Task LoadParameterTables()
        {
            AsyncOperationHandle<IList<ParameterTable>> handle = Addressables.LoadAssetsAsync<ParameterTable>(AddressablesLabels.ParameterTable, null);
            await handle.Task;
            _parameterTables = new List<ParameterTable>(handle.Result);
            handle.Release();
        }

        /// <summary>
        /// IDからパラメータ表のデータを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static ParameterTable GetParameterTable(int characterId)
        {
            return _parameterTables.Find(parameterTable => parameterTable.characterId == characterId);
        }

        /// <summary>
        /// キャラクターの定義データをロードします。
        /// </summary>
        public static async Task LoadCharacterData()
        {
            AsyncOperationHandle<IList<CharacterData>> handle = Addressables.LoadAssetsAsync<CharacterData>(AddressablesLabels.Character, null);
            await handle.Task;
            _characterDataList = new List<CharacterData>(handle.Result);
            handle.Release();
        }

        /// <summary>
        /// キャラクターのIDからキャラクターの定義データを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static CharacterData GetCharacterData(int characterId)
        {
            return _characterDataList.Find(character => character.characterId == characterId);
        }

        /// <summary>
        /// キャラクターのIDからキャラクターのフル名前を取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static string GetCharacterName(int characterId)
        {
            var characterData = GetCharacterData(characterId);
            return characterData.characterName;
        }

        /// <summary>
        /// キャラクターのIDからキャラクターの名前を取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static string GetFirstName(int characterId)
        {
            var characterData = GetCharacterData(characterId);
            return characterData.firstName;
        }

        /// <summary>
        /// キャラクターのIDから覚えるスキルデータを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static List<CharacterSkillRecord> GetCharacterSkillList(int characterId)
        {
            var characterData = _characterDataList.Find(character => character.characterId == characterId);
            return characterData.characterSkillRecords;
        }

        /// <summary>
        /// キャラクターのIDとレベルから現在覚えられる魔法データ一覧を取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        /// <param name="level">キャラクターのレベル</param>
        public static List<SkillData> GetLearnableSkill(int characterId, int level)
        {
            var skillList = GetCharacterSkillList(characterId);
            var records = skillList.Where(x => x.level <= level);
            List<SkillData> skillDataList = new();
            foreach (var record in records)
            {
                var skillData = SkillDataManager.GetSkillDataById(record.skillId);
                skillDataList.Add(skillData);
            }
            return skillDataList;
        }
    }
}