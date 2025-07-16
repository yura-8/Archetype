using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// デバッグ用のログを出力するクラスです。
    /// </summary>
    public class SimpleLogger : MonoBehaviour
    {
        /// <summary>
        /// デバッグログを出力するかどうかを示すフラグです。
        /// </summary>
        [SerializeField]
        bool _outputDebugLog = true;

        /// <summary>
        /// ログ出力時のプレフィックスです。
        /// </summary>
        readonly string LogPrefix = "[Debug] ";

        /// <summary>
        /// このクラスのインスタンスです。
        /// </summary>
        private static SimpleLogger _instance;

        /// <summary>
        /// このクラスのインスタンスです。
        /// </summary>
        public static SimpleLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<SimpleLogger>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new();
                        _instance = singletonObject.AddComponent<SimpleLogger>();
                        singletonObject.name = typeof(SimpleLogger).ToString() + " (Singleton)";
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="message">出力するメッセージ</param>
        public void Log(string message)
        {
            if (_outputDebugLog)
            {
                Debug.Log(LogPrefix + message);
            }
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="message">出力するメッセージ</param>
        public void LogWarning(string message)
        {
            if (_outputDebugLog)
            {
                Debug.LogWarning(LogPrefix + message);
            }
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="message">出力するメッセージ</param>
        public void LogError(string message)
        {
            if (_outputDebugLog)
            {
                Debug.LogError(LogPrefix + message);
            }
        }
    }
}