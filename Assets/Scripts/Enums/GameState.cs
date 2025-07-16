namespace SimpleRpg
{
    /// <summary>
    /// ゲーム全体の状態に関する状態を定義する列挙型です。
    /// </summary>
    public enum GameState
    {
        None, 
        Moving, //キャラクター移動パート
        Battle, //戦闘パート
        Event, //イベント処理パート
    }
}