using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// �G�L�����N�^�[�̏����`����N���X�ł��B
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/SimpleRpg/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        /// <summary>
        /// �G�L�����N�^�[��ID�ł��B
        /// </summary>
        public int enemyId;

        /// <summary>
        /// �G�L�����N�^�[�̖��O�ł��B
        /// </summary>
        public string enemyName;

        /// <summary>
        /// �G�L�����N�^�[�̉摜�ł��B
        /// </summary>
        public Sprite sprite;

        /// <summary>
        /// HP�̒l�ł��B
        /// </summary>
        public int hp;

        /// <summary>
        /// MP�̒l�ł��B
        /// </summary>
        public int mp;

        /// <summary>
        /// �͂̒l�ł��B
        /// </summary>
        public int strength;

        /// <summary>
        /// �g�̂܂���̒l�ł��B
        /// </summary>
        public int guard;

        /// <summary>
        /// �f�����̒l�ł��B
        /// </summary>
        public int speed;

        /// <summary>
        /// �o���l�̒l�ł��B
        /// </summary>
        public int exp;

        /// <summary>
        /// �S�[���h�̒l�ł��B
        /// </summary>
        public int gold;
    }
}