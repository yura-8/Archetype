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

        private static bool _confirmKeyReleased = true;

        public static bool ConfirmButton()
        {
            if (Keyboard.current == null)
                return false;

            var space = Keyboard.current.spaceKey;
            var z = Keyboard.current.zKey;

            bool isPressed = space.isPressed || z.isPressed;

            if (!isPressed)
            {
                _confirmKeyReleased = true;
                return false;
            }

            if (_confirmKeyReleased && isPressed)
            {
                _confirmKeyReleased = false;
                return true;
            }

            return false;
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