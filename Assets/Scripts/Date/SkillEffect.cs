using System;

namespace SimpleRpg
{
    /// <summary>
    /// ���@�̌��ʂɊւ���ݒ��ێ�����N���X�ł��B
    /// </summary>
    [Serializable]
    public class SkillEffect
    {
        /// <summary>
        /// ���@�̃J�e�S���ł��B
        /// </summary>
        public SkillCategory skillCategory;

        /// <summary>
        /// ���@�̌��ʔ͈͂ł��B
        /// </summary>
        public SkillTarget magicTarget;

        /// <summary>
        /// ���ʗʂł��B
        /// </summary>
        public int value;

        /// <summary>
        /// �X�e�C�^�X�w��
        /// </summary>
        public ParameterRecord parameter;
    }
}
