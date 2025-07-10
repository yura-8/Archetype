using UnityEngine;
using UnityEngine.InputSystem;

namespace SimpleRpg
{
    /// <summary>
    /// Yes/No確認ウィンドウのロジックを制御するクラスです。
    /// </summary>
    public class ConfirmationWindowController : MonoBehaviour, IBattleWindowController
    {
        [SerializeField] private ConfirmationUIController _uiController;
        private BattleManager _battleManager;
        private bool _isYesSelected = true;
        private bool _canSelect = false;

        void Update()
        {
            if (!_canSelect || _battleManager == null || _uiController == null) return;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame ||
                Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _isYesSelected = !_isYesSelected;
                _uiController.ShowSelectedCursor(_isYesSelected);
            }

            if (InputGameKey.ConfirmButton()) OnConfirm();
            if (InputGameKey.CancelButton()) OnCancel();
        }

        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        public void ShowWindow()
        {
            gameObject.SetActive(true);   // ルートごと表示
            _uiController.Show();
        }

        public void HideWindow()
        {
            _uiController.Hide();
            gameObject.SetActive(false);  // ルートごと非表示
            _canSelect = false;           // 次回表示まで入力禁止
        }

        public void Activate(string question)
        {
            _isYesSelected = true;
            _uiController.SetQuestionText(question);
            _uiController.ShowSelectedCursor(true);
            ShowWindow();
        }

        public void SetCanSelect(bool canSelect)
        {
            _canSelect = canSelect;
        }

        void OnConfirm()
        {
            HideWindow();

            if (_isYesSelected)
                _battleManager.OnConfirmationAccepted();
            else
                _battleManager.OnConfirmationCanceled();
        }

        void OnCancel()
        {
            HideWindow();
            _battleManager.OnConfirmationCanceled();
        }
    }
}
