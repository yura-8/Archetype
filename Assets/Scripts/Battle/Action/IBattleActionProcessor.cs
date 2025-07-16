namespace SimpleRpg
{
    /// <summary>
    /// 戦闘時の行動を処理するクラス向けのインタフェースです。
    /// </summary>
    public interface IBattleActionProcessor
    {
        /// <summary>
        /// 参照をセットします。
        /// </summary>
        void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor);

        /// <summary>
        /// 行動を処理します。
        /// </summary>
        void ProcessAction(BattleAction action);
    }
}