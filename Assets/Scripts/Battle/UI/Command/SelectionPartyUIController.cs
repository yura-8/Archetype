using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するUIを制御するクラスです。
    /// </summary>
    public class SelectionPartyUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// 1つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionPartyController _controller_0;

        /// <summary>
        /// 2つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionPartyController _controller_1;

        /// <summary>
        /// 3つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionPartyController _controller_2;

        /// <summary>
        /// 4つ目の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionPartyController _controller_3;

        /// <summary>
        /// 位置とコントローラの対応辞書です。
        /// </summary>
        Dictionary<int, SelectionPartyController> _controllerDictionary = new();

        /// <summary>
        /// 位置とコントローラの対応辞書をセットアップします。
        /// </summary>
        public void SetUpControllerDictionary()
        {
            _controllerDictionary = new Dictionary<int, SelectionPartyController>
            {
                { 0, _controller_0 },
                { 1, _controller_1 },
                { 2, _controller_2 },
                { 3, _controller_3 },
            };
        }

        /// <summary>
        /// コマンドのカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _controller_0.HideCursor();
            _controller_1.HideCursor();
            _controller_2.HideCursor();
            _controller_3.HideCursor();
        }

        /// <summary>
        /// 選択中のコマンドのカーソルを表示します。
        /// </summary>
        public void ShowSelectedCursor(int selectedPosition, ElementAttribute attribute)
        {
            HideAllCursor();

            _controllerDictionary.TryGetValue(selectedPosition, out SelectionPartyController selectedController);
            if (selectedController != null)
            {
                selectedController.ShowCursor();
                selectedController.SetCursorColor(attribute);
            }
        }


        /// <summary>
        /// キャラクターの名前をセットします。
        /// </summary>
        /// <param name="characterName">敵キャラクターの名前</param>
        public void SetCharacterName(int position, string characterName)
        {
            _controllerDictionary.TryGetValue(position, out SelectionPartyController controller);
            if (controller != null)
            {
                controller.SetCharacterName(characterName);
            }
        }

        /// <summary>
        /// キャラクターの名前を空欄にします。
        /// </summary>
        public void ClearCharaterName(int position)
        {
            _controllerDictionary.TryGetValue(position, out SelectionPartyController controller);
            if (controller != null)
            {
                controller.ClearCharacterName();
            }
        }

        /// <summary>
        /// 全ての項目のテキストを初期化します。
        /// </summary>
        public void ClearAllCharacterName()
        {
            foreach (var controller in _controllerDictionary.Values)
            {
                controller.ClearCharacterName();
            }
        }

        /// <summary>
        /// パーティーリストの情報でUIを更新します。敵の数に応じて表示/非表示も行います。
        /// </summary>
        public void UpdatePartyList(List<CharacterStatus> partyMembers)
        {
            SetUpControllerDictionary();
            ClearAllCharacterName();

            for (int i = 0; i < 4; i++)
            {
                if (_controllerDictionary.TryGetValue(i, out SelectionPartyController controller))
                {
                    if (i < partyMembers.Count)
                    {
                        var characterName = CharacterDataManager.GetFirstName(partyMembers[i].characterId);
                        SetCharacterName(i, characterName);
                        controller.Show();
                    }
                    else
                    {
                        controller.Hide();
                    }
                }
            }
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