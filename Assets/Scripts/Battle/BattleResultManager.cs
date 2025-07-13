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
            KeyWait();
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
                totalExp += enemyStatus.enemyData.exp;
                gold += enemyStatus.enemyData.gold;
            }

            // プレイヤーの経験値とゴールドを更新します。
            CharacterStatusManager.IncreaseExp(totalExp);
            CharacterStatusManager.IncreaseGold(gold);

            StartCoroutine(WinMessageProcess(totalExp, gold));
        }

        /// <summary>
        /// 戦闘に勝利した時のメッセージ処理です。
        /// </summary>
        /// <param name="exp">獲得経験値</param>
        /// <param name="gold">獲得ゴールド</param>
        IEnumerator WinMessageProcess(int exp, int gold)
        {
            // パーティの最初のメンバーの名前を取得します。
            var firstMemberId = CharacterStatusManager.partyCharacter[0];
            var characterName = CharacterDataManager.GetCharacterName(firstMemberId);

            _pauseMessage = true;
            _messageWindowController.GenerateWinMessage(characterName);
            while (_pauseMessage)
            {
                yield return null;
            }

            if (exp > 0)
            {
                _pauseMessage = true;
                _messageWindowController.GenerateGetExpMessage(exp);
                while (_pauseMessage)
                {
                    yield return null;
                }
            }

            if (gold > 0)
            {
                _pauseMessage = true;
                _messageWindowController.GenerateGetGoldMessage(gold);
                while (_pauseMessage)
                {
                    yield return null;
                }
            }

            // キー入力を待ちます。
            yield return StartCoroutine(KeyWaitProcess());

            foreach (var id in CharacterStatusManager.partyCharacter)
            {
                var isLevelUp = CharacterStatusManager.CheckLevelUp(id);
                if (isLevelUp)
                {
                    _pauseMessage = true;
                    var characterStatus = CharacterStatusManager.GetCharacterStatusById(id);
                    var level = characterStatus.level;
                    characterName = CharacterDataManager.GetCharacterName(id);
                    _messageWindowController.GenerateLevelUpMessage(characterName, level);
                    while (_pauseMessage)
                    {
                        yield return null;
                    }

                    // キー入力を待ちます。
                    yield return StartCoroutine(KeyWaitProcess());
                }
            }

            // 処理の終了を通知します。
            _battleManager.OnFinishBattle();
        }

        /// <summary>
        /// キー入力の間処理を待機するコルーチンです。
        /// </summary>
        IEnumerator KeyWaitProcess()
        {
            _waitKeyInput = true;
            _messageWindowController.ShowPager();
            while (_waitKeyInput)
            {
                yield return null;
            }

            _messageWindowController.HidePager();
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
            // パーティの最初のメンバーの名前を取得します。
            var firstMemberId = CharacterStatusManager.partyCharacter[0];
            var characterName = CharacterDataManager.GetCharacterName(firstMemberId);

            _pauseMessage = true;
            _messageWindowController.GenerateGameoverMessage(characterName);
            while (_pauseMessage)
            {
                yield return null;
            }

            // キー入力を待ちます。
            yield return StartCoroutine(KeyWaitProcess());

            // 処理の終了を通知します。
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