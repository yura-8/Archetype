namespace SimpleRpg
{
    /// <summary>
    /// 戦闘画面のウィンドウを制御するクラス向けのインタフェースです。
    /// </summary>
    public interface IBattleWindowController
    {
        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        void SetUpController(BattleManager battleManager);

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        void ShowWindow();

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        void HideWindow();
    }
}