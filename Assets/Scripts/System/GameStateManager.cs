namespace SimpleRpg
{
    /// <summary>
    /// ゲーム全体の状態を管理するクラスです。
    /// </summary>
    public static class GameStateManager
    {
        /// <summary>
        /// 現在の状態です。
        /// </summary>
        public static GameState CurrentState { get; private set; }

        /// <summary>
        /// 状態を初期化します。
        /// </summary>
        public static void Initialize()
        {
            CurrentState = GameState.None;
        }

        /// <summary>
        /// 状態を移動状態に変更します。
        /// </summary>
        public static void ChangeToMoving()
        {
            CurrentState = GameState.Moving;
        }

        /// <summary>
        /// 状態を戦闘状態に変更します。
        /// </summary>
        public static void ChangeToBattle()
        {
            CurrentState = GameState.Battle;
        }

        /// <summary>
        /// 状態をイベント状態に変更します。
        /// </summary>
        public static void ChangeToEvent()
        {
            CurrentState = GameState.Event;
        }
    }
}