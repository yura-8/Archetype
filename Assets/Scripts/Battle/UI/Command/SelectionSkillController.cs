using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// 選択ウィンドウの各項目を制御するクラスです。
    /// </summary>
    public class SelectionSkillController : MonoBehaviour
    {
        /// <summary>
        /// 項目のカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObj;

        /// <summary>
        /// 選択項目の名前テキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _skillNameText;

        /// <summary>
        /// 選択項目の数値テキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _btText;

        /// <summary>
        /// 選択項目の数値テキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _numberText;

        /// <summary>
        /// 現在の使用回数を表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _currentCountText;

        /// <summary>
        /// 回数用のスラッシュテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _countSrashText;

        /// <summary>
        /// 最大使用回数を表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _maxCountText;

        /// <summary>
        /// 項目が選択可能なときの色です。
        /// </summary>
        [SerializeField]
        Color _selectableColor = Color.white;

        /// <summary>
        /// 項目が選択不可能なときの色です。
        /// </summary>
        [SerializeField]
        Color _unselectableColor = Color.gray;

        /// <summary>
        /// 選択項目のテキストをセットします。
        /// </summary>
        /// <param name="skillName">項目名</param>
        /// <param name="skillNum">項目数</param>
        /// <param name="currentCount">現在の使用回数</param>
        /// <param name="maxCount">最大使用回数 (-1なら無制限)</param>
        public void SetSkillText(string skillName, int cost, int currentCount, int maxCount)
        {
            _skillNameText.text = skillName;
            _numberText.text = cost.ToString();
            _btText.text = "BT"; // ボタンのテキストを設定
            if (maxCount < 0) // 回数無制限の場合
            {
                _currentCountText.gameObject.SetActive(false);
                _maxCountText.gameObject.SetActive(false);
                _countSrashText.gameObject.SetActive(false);
            }
            else // 回数制限がある場合
            {
                _currentCountText.gameObject.SetActive(true);
                _maxCountText.gameObject.SetActive(true);
                _countSrashText.gameObject.SetActive(true);
                _currentCountText.text = currentCount.ToString();
                _maxCountText.text = maxCount.ToString();
                _countSrashText.text = "/"; // スラッシュを表示
            }
        }

        /// <summary>
        /// 選択項目のテキストを初期化します。
        /// </summary>
        public void ClearSkillText()
        {
            _skillNameText.text = "";
            _numberText.text = "";
            _currentCountText.text = "";
            _maxCountText.text = ""; 
            _countSrashText.text = ""; // スラッシュをクリア
            _btText.text = ""; // ボタンのテキストもクリア
        }

        /// <summary>
        /// 選択項目のテキストの色をセットします。
        /// </summary>
        /// <param name="isSelectable">選択可能かどうか</param>
        public void SetSkillTextColors(bool isSelectable)
        {
            SetTextColor(_skillNameText, isSelectable);
            SetTextColor(_numberText, isSelectable);
            SetTextColor(_currentCountText, isSelectable);
            SetTextColor(_countSrashText, isSelectable);
            SetTextColor(_maxCountText, isSelectable);
            SetTextColor(_btText, isSelectable);
        }

        /// <summary>
        /// 選択項目のテキストの色をセットします。
        /// </summary>
        /// <param name="text">対象のテキスト</param>
        /// <param name="isSelectable">選択可能かどうか</param>
        void SetTextColor(TextMeshProUGUI text, bool isSelectable)
        {
            text.color = isSelectable ? _selectableColor : _unselectableColor;
        }

        /// <summary>
        /// 選択項目のカーソルを表示します。
        /// </summary>
        public void ShowCursor()
        {
            _cursorObj.SetActive(true);
        }

        /// <summary>
        /// 選択項目のカーソルを非表示にします。
        /// </summary>
        public void HideCursor()
        {
            _cursorObj.SetActive(false);
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