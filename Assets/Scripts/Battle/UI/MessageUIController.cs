﻿using UnityEngine;
using TMPro;
using System.Text;

namespace SimpleRpg
{
    /// <summary>
    /// メッセージウィンドウのUIを制御するクラスです。
    /// </summary>
    public class MessageUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// メッセージを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _messageText;

        /// <summary>
        /// ページ送りのカーソルです。
        /// </summary>
        [SerializeField]
        GameObject _pagerCursor;

        /// <summary>
        /// 現在表示中のメッセージです。
        /// </summary>
        string _currentMessage;

        // ファイル上部の変数宣言に以下を追加
        private int _lineCount = 0;

        /// <summary>
        /// メッセージをクリアします。
        /// </summary>
        public void ClearMessage()
        {
            _messageText.text = string.Empty;
            _currentMessage = string.Empty;
            _lineCount = 0;
        }

        /// <summary>
        /// メッセージを追加します。
        /// </summary>
        /// <param name="message">追加するメッセージ</param>
        public void AppendMessage(string message)
        {
            StringBuilder sb = new();
            sb.Append(_currentMessage);
            if (sb.Length > 0)
            {
                sb.Append("\n");
            }
            sb.Append(message);
            _currentMessage = sb.ToString();
            _messageText.text = _currentMessage;
            _lineCount++;
        }

        /// <summary>
        /// 現在の表示行数を取得します。
        /// </summary>
        public int GetLineCount()
        {
            return _lineCount;
        }

        /// <summary>
        /// ページ送りのカーソルを表示します。
        /// </summary>
        public void ShowCursor()
        {
            _pagerCursor.SetActive(true);
        }

        /// <summary>
        /// ページ送りのカーソルを非表示にします。
        /// </summary>
        public void HideCursor()
        {
            _pagerCursor.SetActive(false);
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