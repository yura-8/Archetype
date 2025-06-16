
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// ���x���ƃp�����[�^�̑Ή����`����N���X�ł��B
    /// </summary>
    [CreateAssetMenu(fileName = "ParameterTable", menuName = "Scriptable Objects/SimpleRpg/ParameterTable")]
    public class ParameterTable : ScriptableObject
    {
        /// <summary>
        /// ���x���ƃp�����[�^�̑Ή����R�[�h�̃��X�g�ł��B
        /// </summary>
        public List<ParameterRecord> parameterRecords;
    }
}