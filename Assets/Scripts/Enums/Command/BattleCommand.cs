namespace SimpleRpg
{
    /// <summary>
    /// 戦闘のコマンドを定義する列挙型です。
    /// </summary>
    public enum BattleCommand
    {
        Attack, //攻撃
        Guard, //防御
        Item, //アイテム
        Status, //詳細ステータス
        Escape, //逃げる
    }
}