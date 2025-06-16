using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// ���@�̏����`����N���X�ł��B
    /// </summary>
    [CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SimpleRpg/SkillData")]
    public class SkillDate : ScriptableObject
    {
        /// <summary>
        /// �X�L����ID�ł��B
        /// </summary>
        public int skillId;

        /// <summary>
        /// �X�L���̖��O�ł��B
        /// </summary>
        public string skillName;

        /// <summary>
        /// �X�L���̐����ł��B
        /// </summary>
        public string skillDesc;

        /// <summary>
        /// �X�L���̏���BT�ł��B
        /// </summary>
        public int cost;

        /// <summary>
        /// �X�L���̌��ʃ��X�g�ł��B
        /// </summary>
        //public List<SkillEffect> skillEffects;
    }
}