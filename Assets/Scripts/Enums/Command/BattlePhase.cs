namespace SimpleRpg
{
    /// <summary>
    /// 戦闘に関する状態を定義する列挙型です。
    /// </summary>
    public enum BattlePhase
    {
        NotInBattle, //非戦闘
        ShowEnemy, //敵の表示
        InputCommand_Main, //行動選択コマンド入力
        InputCommand_Attack, //攻撃コマンド入力
        SelectEnemy, //ターゲット選択
        SelectParty, //味方選択
        SelectItem, //アイテム選択
        SelectSkill, //スキル選択
        Action, //行動
        Result, //戦闘結果
    }
}