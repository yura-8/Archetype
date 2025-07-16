namespace SimpleRpg
{
    /// <summary>
    /// 戦闘画面のウィンドウ内のUIを制御するクラス向けのインタフェースです。
    /// </summary>
    public interface IBattleUIController
    {
        /// <summary>
        /// UIを表示します。
        /// </summary>
        void Show();

        /// <summary>
        /// UIを非表示にします。
        /// </summary>
        void Hide();
    }
}