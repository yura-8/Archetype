namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のアクションを表すクラスです。
    /// </summary>
    public class BattleAction
    {
        /// <summary>
        /// アクションを行うキャラクターのIDです。
        /// 敵キャラクターの場合は戦闘中IDです。
        /// </summary>
        public int actorId;

        /// <summary>
        /// アクションを行うキャラクターが味方かどうかのフラグです。
        /// </summary>
        public bool isActorFriend;

        /// <summary>
        /// アクションの対象となるキャラクターのIDです。
        /// 敵キャラクターの場合は戦闘中IDです。
        /// </summary>
        public int targetId;

        /// <summary>
        /// アクションの対象となるキャラクターが味方かどうかのフラグです。
        /// </summary>
        public bool isTargetFriend;

        /// <summary>
        /// アクションの種類です。
        /// 敵キャラクターもコマンドを使って判定します。
        /// </summary>
        public BattleCommand battleCommand;

        /// <summary>
        /// スキルか通常攻撃かを示すコマンドです。
        /// </summary>
        public AttackCommand attackCommand;

        /// <summary>
        /// スキルまたはアイテムのIDです。
        /// スキルかアイテムかはbattleCommandで判定します。
        /// </summary>
        public int itemId;

        /// <summary>
        /// アクションを行うキャラクターの素早さです。
        /// </summary>
        public int actorSpeed;

        /// <summary>
        /// アクションの優先度です。
        /// </summary>
        public int priority;

        public BattleAction Clone()
        {
            // 現在のオブジェクトの全メンバーを新しいオブジェクトにコピーして返す
            return (BattleAction)this.MemberwiseClone();
        }
    }
}