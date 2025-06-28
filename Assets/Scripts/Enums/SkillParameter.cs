using System;

namespace SimpleRpg
{
    /// <summary>
    /// レベルに対応するパラメータを保持するクラスです。
    /// </summary>
    [Serializable]
    public enum SkillParameter
    {
        hp, //HP
        bt, //バッテリー
        atk, //攻撃力
        def, //防御力
        dex, //素早さ
    }
}