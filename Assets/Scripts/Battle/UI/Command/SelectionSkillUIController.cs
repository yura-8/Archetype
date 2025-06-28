using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 選択ウィンドウのUIを制御するクラスです。
    /// </summary>
    public class SelectionSkillUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// 説明テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _descriptionText;

        /// <summary>
        /// 1つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionSkillController _controller_0;

        /// <summary>
        /// 2つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionSkillController _controller_1;

        /// <summary>
        /// 3つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionSkillController _controller_2;

        /// <summary>
        /// 4つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionSkillController _controller_3;

        /// <summary>
        /// 前のページがあることを示すカーソルのオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjPrev;

        /// <summary>
        /// 次のページがあることを示すカーソルのオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjNext;

        /// <summary>
        /// 位置とコントローラの対応辞書です。
        /// </summary>
        Dictionary<int, SelectionSkillController> _controllerDictionary = new();

        /// <summary>
        /// 位置とコントローラの対応辞書をセットアップします。
        /// </summary>
        public void SetUpControllerDictionary()
        {
            _controllerDictionary = new Dictionary<int, SelectionSkillController>
            {
                { 0, _controller_0 },
                { 1, _controller_1 },
                { 2, _controller_2 },
                { 3, _controller_3 },
            };
        }

        /// <summary>
        /// 項目のカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _controller_0.HideCursor();
            _controller_1.HideCursor();
            _controller_2.HideCursor();
            _controller_3.HideCursor();
        }

        /// <summary>
        /// 選択中の項目のカーソルを表示します。
        /// </summary>
        /// <param name="selectedPosition">選択中の位置</param>
        public void ShowSelectedCursor(int selectedPosition)
        {
            HideAllCursor();

            _controllerDictionary.TryGetValue(selectedPosition, out SelectionSkillController selectedController);
            if (selectedController != null)
            {
                selectedController.ShowCursor();
            }
        }

        /// <summary>
        /// 指定した位置の項目のテキストをセットします。
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="skillName">項目名</param>
        /// <param name="skillNum">項目数</param>
        /// <param name="currentCount">現在の使用回数</param>
        /// <param name="maxCount">最大使用回数</param>
        /// <param name="canSelect">選択可能かどうか</param>
        public void SetSkillText(int position, string skillName, int skillNum, int currentCount, int maxCount, bool canSelect)
        {
            _controllerDictionary.TryGetValue(position, out SelectionSkillController controller);
            if (controller != null)
            {
                controller.SetSkillText(skillName, skillNum, currentCount, maxCount);
                controller.SetSkillTextColors(canSelect);
            }
        }

        /// <summary>
        /// 指定した位置の項目のテキストを初期化します。
        /// </summary>
        /// <param name="position">位置</param>
        public void ClearSkillText(int position)
        {
            _controllerDictionary.TryGetValue(position, out SelectionSkillController controller);
            if (controller != null)
            {
                controller.ClearSkillText();
            }
        }

        /// <summary>
        /// 全ての項目のテキストを初期化します。
        /// </summary>
        public void ClearAllSkillText()
        {
            foreach (var controller in _controllerDictionary.Values)
            {
                controller.ClearSkillText();
            }
        }

        /// <summary>
        /// 説明テキストをセットします。
        /// </summary>
        /// <param name="description">説明</param>
        public void SetDescriptionText(string description)
        {
            _descriptionText.text = description;
        }

        /// <summary>
        /// 説明テキストを初期化します。
        /// </summary>
        public void ClearDescriptionText()
        {
            _descriptionText.text = string.Empty;
        }

        /// <summary>
        /// 前のページがあることを示すカーソルを表示します。
        /// </summary>
        /// <param name="isVisible">表示するかどうか</param>
        public void SetPrevCursorVisibility(bool isVisible)
        {
            _cursorObjPrev.SetActive(isVisible);
        }

        /// <summary>
        /// 次のページがあることを示すカーソルを表示します。
        /// </summary>
        /// <param name="isVisible">表示するかどうか</param>
        public void SetNextCursorVisibility(bool isVisible)
        {
            _cursorObjNext.SetActive(isVisible);
        }

        /// <summary>
        /// UIを表示します。
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// UIを非表示にします。
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}