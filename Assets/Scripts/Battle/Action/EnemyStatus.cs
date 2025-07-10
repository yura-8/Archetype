namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の敵キャラクターのステータスを保持するクラスです。
    /// </summary>
    public class EnemyStatus
    {
        /// <summary>
        /// 敵キャラクターのIDです。
        /// </summary>
        public int enemyId;

        /// <summary>
        /// 敵キャラクターの戦闘中の通しIDです。
        /// </summary>
        public int enemyBattleId;

        /// <summary>
        /// 敵キャラクターの定義データです。
        /// </summary>
        public EnemyData enemyData;

        /// <summary>
        /// 敵キャラクターの現在のHPです。
        /// </summary>
        public int currentHp;

        /// <summary>
        /// 敵キャラクターの現在のMPです。
        /// </summary>
        public int currentBt;

        /// <summary>
        /// 敵キャラクターが倒されたフラグです。
        /// </summary>
        public bool isDefeated;

        public bool isGuarding;

        /// <summary>
        /// 敵キャラクターが逃げたフラグです。
        /// </summary>
        public bool isRunaway;
    }
}