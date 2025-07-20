using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 経験値とレベルの対応を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "ExpTable", menuName = "Scriptable Objects/SimpleRpg/ExpTable")]
    public class ExpTable : ScriptableObject
    {
        /// <summary>
        /// 経験値とレベルの対応レコードのリストです。
        /// </summary>
        public List<ExpRecord> expRecords;
    }
}