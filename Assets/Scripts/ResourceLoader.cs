using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 定義データをロードするクラスです。
    /// </summary>
    public class ResourceLoader : MonoBehaviour
    {
        void Start()
        {
            LoadDefinitionData();
        }

        /// <summary>
        /// 定義データをロードします。
        /// </summary>
        void LoadDefinitionData()
        {
            CharacterDataManager.LoadCharacterData(); // キャラクターのデータ
            CharacterDataManager.LoadExpTables(); // 経験値テーブル
            CharacterDataManager.LoadParameterTables(); // パラメーターテーブル

            EnemyDataManager.LoadEnemyData(); // 敵キャラクターのデータ
            ItemDataManager.LoadItemData(); // アイテムのデータ
            SkillDataManager.LoadSkillData(); // スキルのデータ
        }
    }
}