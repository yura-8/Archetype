namespace SimpleRpg
{
    /// <summary>
    /// アイテム使用時の効果の種別を定義する列挙型です。
    /// </summary>
    public enum ItemEffectCategory
    {
        None, //カテゴリなし
        Recovery, //回復系
        Damage,　//ダメージ
        Support,　//補助
        Movement,　//移動
    }
}