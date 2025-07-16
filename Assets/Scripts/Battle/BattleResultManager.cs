using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘の結果処理を管理するクラスです。
    /// </summary>
    public class BattleResultManager : MonoBehaviour
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 戦闘中の敵キャラクターのデータを管理するクラスへの参照です。
        /// </summary>
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// メッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MessageWindowController _messageWindowController;

        /// <summary>
        /// メッセージのキー入力を待つかどうかのフラグです。
        /// </summary>
        bool _waitKeyInput;

        public bool IsWaitingKeyInput { get; private set; }

        /// <summary>
        /// メッセージをポーズするかどうかのフラグです。
        /// </summary>
        bool _pauseMessage;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラスへの参照</param>
        public void SetReferences(BattleManager battleManager)
        {
            _battleManager = battleManager;
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        }

        void Update()
        {
            if (!IsWaitingKeyInput)
            {
                return;
            }
            if (InputGameKey.ConfirmButton() || InputGameKey.CancelButton())
            {
                IsWaitingKeyInput = false;
            }
        }

        /// <summary>
        /// 戦闘に勝利した時の処理です。
        /// </summary>
        public void OnWin()
        {
            // 倒した敵の経験値とゴールドを集計します。
            int totalExp = 0;
            int gold = 0;
            foreach (var enemyStatus in _enemyStatusManager.GetEnemyStatusList())
            {
                if (enemyStatus.isDefeated)
                {
                    totalExp += enemyStatus.enemyData.exp;
                    gold += enemyStatus.enemyData.gold;
                }
            }

            // ゴールドを加算します。
            if (gold > 0)
            {
                CharacterStatusManager.IncreaseGold(gold);
            }

            // 経験値を加算します。
            // IncreaseExpメソッドが内部で生存確認を行っているため、この呼び出しだけでOKです。
            if (totalExp > 0)
            {
                CharacterStatusManager.IncreaseExp(totalExp);
            }

            // メッセージ表示処理のコルーチンを開始します。
            StartCoroutine(WinMessageProcess(totalExp, gold));
        }

        /// <summary>
        /// 戦闘に勝利した時のメッセージ処理です。
        /// </summary>
        /// <param name="exp">獲得経験値</param>
        /// <param name="gold">獲得ゴールド</param>
        IEnumerator WinMessageProcess(int exp, int gold)
        {
            // --- 0. 開始前にウィンドウをクリア ---
            _messageWindowController.ClearMessage();

            var firstMemberId = GameDataManager.Instance.PartyCharacterIds[0];
            var characterName = CharacterDataManager.GetCharacterName(firstMemberId);

            // --- 1. 勝利メッセージ、経験値、ゴールドを順番に表示 ---
            // ShowMessageWithPagingという新しいメソッドを使って、1行ずつ表示とページ送りを制御します
            yield return StartCoroutine(ShowMessageWithPaging(
                _messageWindowController.GenerateWinMessage(characterName)
            ));

            if (exp > 0)
            {
                yield return StartCoroutine(ShowMessageWithPaging(
                    _messageWindowController.GenerateGetExpMessage(exp)
                ));
            }

            if (gold > 0)
            {
                yield return StartCoroutine(ShowMessageWithPaging(
                    _messageWindowController.GenerateGetGoldMessage(gold)
                ));
            }

            // --- 2. レベルアップしたキャラクターを一人ずつ表示 ---
            foreach (var id in GameDataManager.Instance.PartyCharacterIds)
            {
                if (CharacterStatusManager.CheckLevelUp(id))
                {
                    var status = CharacterStatusManager.GetCharacterStatusById(id);
                    var name = CharacterDataManager.GetCharacterName(id);
                    yield return StartCoroutine(ShowMessageWithPaging(
                        _messageWindowController.GenerateLevelUpMessage(name, status.level)
                    ));
                }
            }

            // --- 3. 全てのメッセージ表示後、最後のキー入力を待つ ---
            _messageWindowController.ShowPager();
            IsWaitingKeyInput = true;
            yield return new WaitUntil(() => !IsWaitingKeyInput);
            _messageWindowController.HidePager();

            // --- 4. 戦闘終了処理を呼び出し、シーンを遷移させる ---
            _battleManager.OnFinishBattle();
        }

        // 新しいヘルパーメソッド（ページ送り機能の中核）をここに追加します。
        private IEnumerator ShowMessageWithPaging(IEnumerator messageCoroutine)
        {
            // 表示前に、ウィンドウの行数が3行以上ならページ送りを行う
            if (_messageWindowController.GetUIMessageLineCount() >= 3)
            {
                _messageWindowController.ShowPager();
                IsWaitingKeyInput = true;
                yield return new WaitUntil(() => !IsWaitingKeyInput);
                _messageWindowController.HidePager();
                _messageWindowController.ClearMessage();
            }

            // 渡されたメッセージ表示コルーチンを実行（これで1行メッセージが表示される）
            yield return StartCoroutine(messageCoroutine);
        }

        /// <summary>
        /// キー入力の間処理を待機するコルーチンです。
        /// </summary>
        IEnumerator KeyWaitProcess()
        {
            _waitKeyInput = true;
            //_messageWindowController.ShowPager();
            while (_waitKeyInput)
            {
                yield return null;
            }

            //_messageWindowController.HidePager();
        }

        /// <summary>
        /// キー入力を待つ処理です。
        /// </summary>
        public void KeyWait()
        {
            if (!_waitKeyInput)
            {
                return;
            }

            if (InputGameKey.ConfirmButton() || InputGameKey.CancelButton())
            {
                _waitKeyInput = false;
            }
        }

        /// <summary>
        /// 戦闘に敗北した時の処理です。
        /// </summary>
        public void OnLose()
        {
            StartCoroutine(LoseMessageProcess());
        }

        /// <summary>
        /// 戦闘に敗北した時のメッセージ処理です。
        /// </summary>
        IEnumerator LoseMessageProcess()
        {
            _messageWindowController.ClearMessage();
            var firstMemberId = GameDataManager.Instance.PartyCharacterIds[0];
            var characterName = CharacterDataManager.GetCharacterName(firstMemberId);

            yield return StartCoroutine(ShowMessageWithPaging(
                 _messageWindowController.GenerateGameoverMessage(characterName)
            ));

            _messageWindowController.ShowPager();
            IsWaitingKeyInput = true;
            yield return new WaitUntil(() => !IsWaitingKeyInput);
            _messageWindowController.HidePager();

            _battleManager.OnFinishBattleWithGameover();
        }

        /// <summary>
        /// 次のメッセージを表示します。
        /// </summary>
        public void ShowNextMessage()
        {
            _pauseMessage = false;
        }
    }
}