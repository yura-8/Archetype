﻿namespace SimpleRpg
{
    /// <summary>
    /// スキルやアイテムの効果範囲を定義する列挙型です。
    /// </summary>
    public enum EffectTarget
    {
        Own, //自分自身
        FriendSolo, //味方単体
        FriendAll, //味方全体
        EnemySolo, //敵単体
        EnemyAll, //敵全体
    }
}