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
        /// 補助効果のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateSupportEffectMessage(string targetName, SkillParameter parameter, int value)
        {
            // パラメータ名と変動メッセージを決定
            string parameterName = parameter.ToString(); // ATK, DEFなど
            string changeMessage = value > 0 ? "あがった" : "さがった";

            string message = $"{targetName} の {parameterName} が {changeMessage}！";
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
            //uiController.ClearMessage();
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
            //uiController.ClearMessage();
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
        private IEnumerator ShowMessageProcess(string message) 
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

        /// <summary>
        /// 特殊状態になった時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateStatusEffectMessage(string targetName, SpecialStatusType statusType)
        {
            string statusMessage = "";
            switch (statusType)
            {
                case SpecialStatusType.Overheat:
                    statusMessage = "は オーバーヒートした！";
                    break;
                case SpecialStatusType.Overcharge:
                    statusMessage = "は 過充電状態になった！";
                    break;
                case SpecialStatusType.Stun:
                    statusMessage = "は スタンした！";
                    break;
            }

            if (!string.IsNullOrEmpty(statusMessage))
            {
                string message = targetName + statusMessage;
                yield return StartCoroutine(ShowMessageProcess(message));
            }
        }

        /// <summary>
        /// 特殊状態から回復した時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateStatusRecoveryMessage(string targetName)
        {
            string message = $"{targetName} の 様子が元に戻った。";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 過充電によるダメージメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateOverchargeDamageMessage(string targetName, int damage)
        {
            string message = $"過充電により {targetName} は {damage} のダメージを受けた！";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 緊急バッテリー発動メッセージを生成します。
        /// </summary>
        public IEnumerator GenerateEmergencyBatteryMessage(string targetName)
        {
            string message = $"緊急バッテリーが作動！ {targetName} のBTが回復した！";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// スタンで行動不能な時のメッセージを生成します。
        /// </summary>
        public IEnumerator GenerateStunnedMessage(string targetName)
        {
            string message = $"{targetName} は スタンしていて動けない！";
            yield return StartCoroutine(ShowMessageProcess(message));
        }

        /// <summary>
        /// 複数行のメッセージを一度に表示します。
        /// </summary>
        public IEnumerator GenerateMultiLineMessage(string message)
        {
            uiController.ClearMessage();
            uiController.AppendMessage(message);
            yield return StartCoroutine(ShowMessageProcess("")); // 既存の待機処理を流用
        }

        public int GetUIMessageLineCount()
        {
            return uiController.GetLineCount();
        }

        public void ClearMessage()
        {
            uiController.ClearMessage();
        }

    }
}