using System;

namespace SimpleRpg
{
    /// <summary>
    /// ���x���ɑΉ�����p�����[�^��ێ�����N���X�ł��B
    /// </summary>
    [Serializable]
    public class ParameterRecord
    {
        /// <summary>
        /// ���x���̒l�ł��B
        /// </summary>
        public int level;

        /// <summary>
        /// HP�̒l�ł��B
        /// </summary>
        public int hp;

        /// <summary>
        /// �o�b�e���[�̒l�ł��B
        /// </summary>
        public int bt;

        /// <summary>
        /// �U���͂̒l�ł��B
        /// </summary>
        public int atk;

        /// <summary>
        /// �h��̒l�ł��B
        /// </summary>
        public int def;

        /// <summary>
        /// ���x�̒l�ł��B
        /// </summary>
        public int dex;
    }
}