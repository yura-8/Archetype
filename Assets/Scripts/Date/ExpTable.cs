using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// �o���l�ƃ��x���̑Ή����`����N���X�ł��B
    /// </summary>
    [CreateAssetMenu(fileName = "ExpTable", menuName = "Scriptable Objects/SimpleRpg/ExpTable")]
    public class ExpTable : ScriptableObject
    {
        /// <summary>
        /// �o���l�ƃ��x���̑Ή����R�[�h�̃��X�g�ł��B
        /// </summary>
        public List<ExpRecord> expRecords;
    }
}