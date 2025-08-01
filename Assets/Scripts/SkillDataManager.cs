﻿// SkillDataManager.cs
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks; 

namespace SimpleRpg
{
    public static class SkillDataManager
    {
        static List<SkillData> _skillDataList = new();

        /// <summary>
        /// スキルデータをロードします。
        /// </summary>
        public static async Task LoadSkillData()
        {
            AsyncOperationHandle<IList<SkillData>> handle = Addressables.LoadAssetsAsync<SkillData>(AddressablesLabels.Skill, null);
            await handle.Task;
            _skillDataList = new List<SkillData>(handle.Result);
        }

        public static SkillData GetSkillDataById(int skillId)
        {
            return _skillDataList.Find(skill => skill.skillId == skillId);
        }

        public static List<SkillData> GetAllData()
        {
            return _skillDataList;
        }
    }
}