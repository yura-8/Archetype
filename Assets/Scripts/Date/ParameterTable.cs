
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// レベルとパラメータの対応を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "ParameterTable", menuName = "Scriptable Objects/SimpleRpg/ParameterTable")]
    public class ParameterTable : ScriptableObject
    {
        /// <summary>
        /// レベルとパラメータの対応レコードのリストです。
        /// </summary>
        public List<ParameterRecord> parameterRecords;
    }
}