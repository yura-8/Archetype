using UnityEngine;
using UnityEngine.InputSystem;

namespace SimpleRpg
{
    public class CharacterDetailStatusWindowController : MonoBehaviour, IBattleWindowController
    {
        [SerializeField] private CharacterDetailStatusUIController _uiController;
        private BattleManager _battleManager;

        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        void Update()
        {
            if (gameObject.activeSelf && (_battleManager.BattlePhase == BattlePhase.ShowStatusDetail) && InputGameKey.CancelButton())
            {
                _battleManager.OnStatusDetailCanceled();
            }
        }

        public void DisplayCharacter(int characterId)
        {
            CharacterStatus status = CharacterStatusManager.GetCharacterStatusById(characterId);
            ParameterRecord baseParams = CharacterDataManager.GetParameterTable(characterId)?.parameterRecords.Find(p => p.level == status.level);
            BattleParameter currentParams = CharacterStatusManager.GetCharacterBattleParameterById(characterId);

            if (status == null || baseParams == null) return;

            _uiController.SetName(CharacterDataManager.GetCharacterName(characterId));
            _uiController.SetLevel(status.level);

            _uiController.SetCurrentHp(status.currentHp);
            _uiController.SetMaxHp(baseParams.hp);
            _uiController.SetCurrentBt(status.currentBt);
            _uiController.SetMaxBt(baseParams.bt - status.maxBtPenalty);
            _uiController.SetCurrentBtColorByStatus(status.currentStatus); // BTの色を設定

            _uiController.SetAtk(baseParams.atk, currentParams.atk);
            _uiController.SetDef(baseParams.def, currentParams.def);
            _uiController.SetDex(baseParams.dex, currentParams.dex);

            _uiController.SetAttribute(status.attribute);
            _uiController.SetSpecialStatus(status.currentStatus);
        }

        public void ShowWindow() => _uiController.Show();
        public void HideWindow() => _uiController.Hide();
    }
}