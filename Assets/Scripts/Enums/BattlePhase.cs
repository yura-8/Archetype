namespace SimpleRpg
{
    /// <summary>
    /// 戦闘に関する状態を定義する列挙型です。
    /// </summary>
    public enum BattlePhase
    {
        NotInBattle, //非戦闘
        ShowEnemy, //敵の表示
        InputCommand, //コマンド入力
        SelectItem, //スキル・アイテム選択
        Action, //行動
        Result, //戦闘結果
    }
}