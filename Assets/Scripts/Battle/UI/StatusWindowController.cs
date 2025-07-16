// StatusWindowController.cs

using UnityEngine;

namespace SimpleRpg
{
    public class StatusWindowController : MonoBehaviour, IBattleWindowController
    {
        // --- 既存の変数は変更なし ---
        [SerializeField]
        StatusUIController _uiController;

        // このクラスはもう_highlightImageを直接持たないため、以下の変数を削除します
        // [SerializeField] private Image _highlightImage;

        // --- SetUpController, SetCharacterStatus, UpdateStatus, UpdateAllCharacterStatus は変更なし ---
        public void SetUpController(BattleManager battleManager)
        {
        }

        public void SetCharacterStatus(CharacterStatus characterStatus)
        {
            if (characterStatus == null)
            {
                SimpleLogger.Instance.LogWarning("キャラクターステータスがnullです。");
                return;
            }

            var characterName = CharacterDataManager.GetCharacterName(characterStatus.characterId);
            _uiController.SetCharacterName(characterName);

            var level = characterStatus.level;
            var parameterTable = CharacterDataManager.GetParameterTable(characterStatus.characterId);
            var record = parameterTable.parameterRecords.Find(r => r.level == level);

            // ペナルティを考慮した「実効最大BT」を計算します
            int effectiveMaxBt = record.bt - characterStatus.maxBtPenalty;
            if (effectiveMaxBt < 1) effectiveMaxBt = 1; // 最小値は1

            _uiController.SetCurrentHp(characterStatus.currentHp);
            _uiController.SetMaxHp(record.hp);
            _uiController.SetCurrentBt(characterStatus.currentBt);
            _uiController.SetMaxBt(effectiveMaxBt);

            _uiController.SetStatusColor(characterStatus.currentStatus);
            _uiController.SetAttributeHighlightColor(characterStatus.attribute);
        }

        public void UpdateAllCharacterStatus()
        {
            foreach (var characterId in GameDataManager.Instance.PartyCharacterIds)
            {
                var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
                SetCharacterStatus(characterStatus);
            }
        }

        public void UpdateStatus(CharacterStatus characterStatus)
        {
            SetCharacterStatus(characterStatus);
        }

        /// <summary>
        /// このステータスウィンドウのハイライト表示を切り替えます。
        /// </summary>
        /// <param name="isActive">ハイライトを有効にする場合はtrue</param>
        public void SetHighlight(bool isActive)
        {
            // ★ 修正点：自分自身で表示を切り替えるのではなく、UIControllerに指示を出す
            if (_uiController != null)
            {
                _uiController.SetHighlightActive(isActive);
            }
        }

        // --- ShowWindow, HideWindow は変更なし ---
        public void ShowWindow()
        {
            _uiController.Show();
        }

        public void HideWindow()
        {
            _uiController.Hide();
        }
    }
}