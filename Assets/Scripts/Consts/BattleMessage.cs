namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中に表示するメッセージの定義クラスです。
    /// </summary>
    public class BattleMessage
    {
        /// <summary>
        /// 敵キャラクター出現のメッセージのフォーマットです。
        /// </summary>
        public static readonly string EnemyAppearSuffix = "があらわれた！";

        /// <summary>
        /// 攻撃時のメッセージのフォーマットです。
        /// </summary>>
        public static readonly string AttackSuffix = "の攻撃！";

        /// <summary>
        /// 攻撃を受ける側のメッセージのフォーマットです。
        /// </summary>
        public static readonly string DefendSuffix = "に";

        /// <summary>
        /// ダメージ量のメッセージのフォーマットです。
        /// </summary>
        public static readonly string DamageSuffix = "のダメージ！";

        public static readonly string GuardSuffix = "の防御！！";

        /// <summary>
        /// スキル使用時の使用者のメッセージのフォーマットです。
        /// </summary>
        public static readonly string SkillUserSuffix = "は";

        /// <summary>
        /// スキル使用時のスキル名のメッセージのフォーマットです。
        /// </summary>
        public static readonly string SkillNameSuffix = "を唱えた！";

        /// <summary>
        /// アイテム使用時の使用者のメッセージのフォーマットです。
        /// </summary>
        public static readonly string ItemUserSuffix = "は";

        /// <summary>
        /// アイテム使用時のアイテム名のメッセージのフォーマットです。
        /// </summary>
        public static readonly string ItemNameSuffix = "を使った！";

        /// <summary>
        /// 逃走時の代表者のメッセージのフォーマットです。
        /// </summary>
        public static readonly string EscapenerSuffix = "は逃げ出した！";

        /// <summary>
        /// 逃走失敗時のメッセージのフォーマットです。
        /// </summary>
        public static readonly string EscapeFailed = "しかし回り込まれてしまった！";

        /// <summary>
        /// HP回復のターゲットのメッセージのフォーマットです。
        /// </summary>
        public static readonly string HealTargetSuffix = "のHPが";

        /// <summary>
        /// HP回復の効果量のメッセージのフォーマットです。
        /// </summary>
        public static readonly string HealNumSuffix = "回復した！";

        /// <summary>
        /// 敵を倒した時のメッセージのフォーマットです。
        /// </summary>
        public static readonly string DefeatEnemySuffix = "をやっつけた！";

        /// <summary>
        /// 敵に倒された時のメッセージのフォーマットです。
        /// </summary>
        public static readonly string DefeatFriendSuffix = "はやられてしまった！";

        /// <summary>
        /// 全員が敵に倒された時のメッセージのフォーマットです。
        /// </summary>
        public static readonly string GameoverSuffix = "は目の前が真っ暗になった……。";

        /// <summary>
        /// 敵に勝利した時のメッセージのフォーマットです。
        /// </summary>
        public static readonly string WinSuffix = "は戦いに勝利した！";

        /// <summary>
        /// 経験値を得た時のメッセージのフォーマットです。
        /// </summary>
        public static readonly string GetExpSuffixSuffix = "の経験値を獲得。";

        /// <summary>
        /// ゴールドを得た時のメッセージのフォーマットです。
        /// </summary>
        public static readonly string GetGoldSuffixSuffix = "ゴールドを手に入れた。";

        /// <summary>
        /// レベルが上がった時の名前の後ろに表示するメッセージのフォーマットです。
        /// </summary>
        public static readonly string LevelUpNameSuffix = "のレベルが";

        /// <summary>
        /// レベルが上がった時の数値の後ろに表示するメッセージのフォーマットです。
        /// </summary>
        public static readonly string LevelUpNumberSuffix = "に上がった！";
    }
}