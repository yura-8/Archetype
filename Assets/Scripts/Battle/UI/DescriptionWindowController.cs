using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// 説明文のウィンドウを制御するクラスです。
    /// </summary>
    public class DescriptionWindowController : MonoBehaviour, IBattleWindowController
    {
        [SerializeField]
        private TextMeshProUGUI _descriptionText; // インスペクターでテキストを設定

        /// <summary>
        /// ウィンドウに表示するテキストを更新します。
        /// </summary>
        public void UpdateText(string text)
        {
            Debug.Log($"UpdateText called with: {text}, _descriptionText={(_descriptionText != null ? _descriptionText.name : "null")}");
            if (_descriptionText != null)
            {
                _descriptionText.text = text;
                Debug.Log($"Updated _descriptionText.text to: {_descriptionText.text}");
            }
            else
            {
                Debug.LogError("UpdateText: _descriptionText is null. Check Inspector settings.");
            }

            _descriptionText.ForceMeshUpdate();
        }

        /// <summary>
        /// バトルマネージャーへの参照をセットアップします（IBattleWindowControllerの実装）。
        /// </summary>
        /// <param name="battleManager">戦闘管理クラス</param>
        public void SetUpController(BattleManager battleManager)
        {
            // DescriptionWindowでは特にbattleManagerへの参照は不要なため、中身は空でOK
        }

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            gameObject.SetActive(false);
        }
    }
}