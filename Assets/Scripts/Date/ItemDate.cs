using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// �A�C�e���̏����`����N���X�ł��B
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/SimpleRpg/ItemData")]
    public class ItemData : ScriptableObject
    {
        /// <summary>
        /// �A�C�e����ID�ł��B
        /// </summary>
        public int itemId;

        /// <summary>
        /// �A�C�e���̖��O�ł��B
        /// </summary>
        public string itemName;

        /// <summary>
        /// �A�C�e���̐����ł��B
        /// </summary>
        public string itemDesc;

        /// <summary>
        /// �A�C�e���̃J�e�S���ł��B
        /// </summary>
        public ItemCategory itemCategory;

        /// <summary>
        /// HP�񕜗ʁB
        /// </summary>
        public int hp;

        /// <summary>
        /// �o�b�e���[�񕜗ʁB
        /// </summary>
        public int bt;

        /// <summary>
        /// �g�p�\�񐔂ł��B
        /// </summary>
        public int numberOfUse;

        /// <summary>
        /// �U���͂̕␳�l�ł��B
        /// </summary>
        public int atk;

        /// <summary>
        /// �h��͂̕␳�l�ł��B
        /// </summary>
        public int def;

        /// <summary>
        /// �f�����̕␳�l�ł��B
        /// </summary>
        public int dex;

        /// <summary>
        /// �A�C�e���̉��i�ł��B
        /// </summary>
        public int price;
    }
}