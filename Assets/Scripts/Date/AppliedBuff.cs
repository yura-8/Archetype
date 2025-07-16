namespace SimpleRpg
{
    /// <summary>
    /// キャラクターに現在適用されているバフ・デバフの効果を保持するクラスです。
    /// </summary>
    public class AppliedBuff
    {
        public SkillParameter parameter; // 効果対象 (ATK, DEFなど)
        public int value;                // 変化量
        public int duration;             // 残り持続ターン
    }
}