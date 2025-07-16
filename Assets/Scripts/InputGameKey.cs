using UnityEngine;
using UnityEngine.InputSystem;

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
            if (Keyboard.current == null)
            {
                return false;
            }

            return Keyboard.current.spaceKey.wasPressedThisFrame
                || Keyboard.current.zKey.wasPressedThisFrame;
        }

        /// <summary>
        /// キャンセルボタンが押されたかどうかを取得します。
        /// </summary>
        public static bool CancelButton()
        {
            if (Keyboard.current == null)
            {
                return false;
            }

            return Keyboard.current.xKey.wasPressedThisFrame;
        }
    }
}