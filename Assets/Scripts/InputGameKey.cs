using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内のキー入力を定義するクラスです。
    /// </summary>
    public static class InputGameKey
    {
        /// <summary>
        /// 決定ボタンが押されたかどうかを取得します。
        /// </summary>
        public static bool ConfirmButton()
        {
            return Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Z);
        }

        /// <summary>
        /// キャンセルボタンが押されたかどうかを取得します。
        /// </summary>
        public static bool CancelButton()
        {
            return Input.GetKeyDown(KeyCode.X);
        }
    }
}