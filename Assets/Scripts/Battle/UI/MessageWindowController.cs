using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メッセージウィンドウの動作を制御するクラスです。
    /// </summary>
    public class MessageWindowController : MonoBehaviour, IBattleWindowController
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// メッセージウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MessageUIController uiController;

        /// <summary>
        /// メッセージの表示間隔です。
        /// </summary>
        [SerializeField]
        float _messageInterval = 1.0f;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            uiController.ClearMessage();
            uiController.Show();
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            uiController.ClearMessage();
            uiController.Hide();
        }

        /// <summary>
        /// 攻撃時のメッセージを生成します。
        /// </summary>
        public void GenerateEnemyAppearMessage(string enemyName, float appearInterval)
        {
            uiController.ClearMessage();
            string message = $"{enemyName}{BattleMessage.EnemyAppearSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message, appearInterval));
        }

        /// <summary>
        /// 攻撃時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateAttackMessage(string attackerName)
        {
            uiController.ClearMessage();
            string message = $"{attackerName}{BattleMessage.AttackSuffix}";
            //StartCoroutine(ShowMessageAutoProcess(message));
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// ダメージ発生時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateDamageMessage(string targetName, int damage)
        {
            string message = $"{targetName}{BattleMessage.DefendSuffix} {damage} {BattleMessage.DamageSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 敵を撃破した時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateDefeateEnemyMessage(string targetName)
        {
            string message = $"{targetName}{BattleMessage.DefeatEnemySuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 味方がやられた時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateDefeateFriendMessage(string targetName)
        {
            string message = $"{targetName}{BattleMessage.DefeatFriendSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// スキルを唱えた時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateSkillCastMessage(string skillUserName, string skillName)
        {
            uiController.ClearMessage();
            string message = $"{skillUserName}{BattleMessage.SkillUserSuffix} {skillName} {BattleMessage.SkillNameSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 防御した時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateGuardMessage(string actorName)
        {
            uiController.ClearMessage();
            string message = $"{actorName}{BattleMessage.GuardSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// HPが回復する時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateHpHealMessage(string targetName, int healNum)
        {
            string message = $"{targetName}{BattleMessage.HealTargetSuffix} {healNum} {BattleMessage.HealNumSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// アイテムを使用した時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateUseItemMessage(string itemUserName, string itemName)
        {
            uiController.ClearMessage();
            string message = $"{itemUserName}{BattleMessage.ItemUserSuffix} {itemName} {BattleMessage.ItemNameSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 逃走した時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateEscapeMessage(string characterName)
        {
            uiController.ClearMessage();
            string message = $"{characterName}{BattleMessage.EscapenerSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 逃走が失敗した時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateEscapeFailedMessage()
        {
            string message = BattleMessage.EscapeFailed;
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// ゲームオーバー時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateGameoverMessage(string characterName)
        {
            uiController.ClearMessage();
            string message = $"{characterName}{BattleMessage.GameoverSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 戦闘勝利時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateWinMessage(string characterName)
        {
            uiController.ClearMessage();
            string message = $"{characterName}{BattleMessage.WinSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// Exp獲得時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateGetExpMessage(int exp)
        {
            string message = $"{exp} {BattleMessage.GetExpSuffixSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// ゴールド獲得時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateGetGoldMessage(int gold)
        {
            string message = $"{gold} {BattleMessage.GetGoldSuffixSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// レベルアップ時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateLevelUpMessage(string characterName, int level)
        {
            uiController.ClearMessage();
            string message = $"{characterName}{BattleMessage.LevelUpNameSuffix} {level} {BattleMessage.LevelUpNumberSuffix}";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// ページ送りのカーソルを表示します。
        /// </summary>
        public void ShowPager()
        {
            uiController.ShowCursor();
        }

        /// <summary>
        /// ページ送りのカーソルを非表示にします。
        /// </summary>
        public void HidePager()
        {
            uiController.HideCursor();
        }

        /// <summary>
        /// メッセージを順番に表示するコルーチンです。
        /// </summary>
        IEnumerator ShowMessageAutoProcess(string message)
        {
            uiController.AppendMessage(message);
            yield return new WaitForSeconds(_messageInterval);
            _battleManager.OnFinishedShowMessage();
        }

        /// <summary>
        /// メッセージを順番に表示するコルーチンです。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="interval">表示間隔</param>
        IEnumerator ShowMessageAutoProcess(string message, float interval)
        {
            uiController.AppendMessage(message);
            yield return new WaitForSeconds(interval);
            _battleManager.OnFinishedShowMessage();
        }

        /// <summary>
        /// メッセージを順番に表示するコルーチンです。
        /// </summary>
        private IEnumerator ShowMessageProcess(string message) // メソッド名を変更
        {
            uiController.AppendMessage(message);
            yield return new WaitForSeconds(_messageInterval);
        }

        /// <summary>
        /// パーティが逃走した時のメッセージを生成します。
        /// </summary>
        public IEnumerator GeneratePartyEscapeMessage()
        {
            uiController.ClearMessage();
            // メッセージは固定でOK
            string message = "パーティはにげだした！";
            yield return StartCoroutine(ShowMessageProcess(message));
        }
    }
}