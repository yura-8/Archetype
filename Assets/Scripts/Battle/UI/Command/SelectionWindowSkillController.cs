using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 選択ウィンドウにてスキルに関する処理を制御するクラスです。
    /// </summary>
    public class SelectionWindowSkillController : MonoBehaviour
    {
        /// <summary>
        /// 項目オブジェクトとスキルIDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _skillIdDictionary = new();

        /// <summary>
        /// キャラクターが覚えているスキルの一覧です。
        /// </summary>
        List<SkillData> _characterSkillList = new();

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        public bool IsValidIndex(int index)
        {
            bool isValid = index >= 0 && index < _characterSkillList.Count;
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// スキルの場合は消費MPを確認、アイテムの場合は所持数を確認します。
        /// </summary>
        /// <param name="selectedIndex">選択中のインデックス</param>
        public bool IsValidSelection(int selectedIndex)
        {
            bool isValid = false;
            int indexInPage = selectedIndex % 4;

            // インデックスが辞書に存在しない場合は有効でないと判断します。
            if (!_skillIdDictionary.ContainsKey(indexInPage))
            {
                return isValid;
            }

            var skillId = _skillIdDictionary[indexInPage];
            var skillData = SkillDataManager.GetSkillDataById(skillId);
            isValid = CanSelectSkill(skillData);
            return isValid;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        public int GetMaxPageNum()
        {
            int maxPage = Mathf.CeilToInt(_characterSkillList.Count * 1.0f / 4.0f);
            return maxPage;
        }

        /// <summary>
        /// キャラクターが覚えているスキルをリストにセットします。
        /// </summary>
        public void SetCharacterSkill()
        {
            _characterSkillList.Clear();

            // 指定したキャラクターのステータスを取得します。
            var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
            foreach (var skillId in characterStatus.SkillList)
            {
                var skillData = SkillDataManager.GetSkillDataById(skillId);
                _characterSkillList.Add(skillData);
            }

            Debug.Log($"[SkillController] SetCharacterSkill completed. Skill count: {_characterSkillList.Count}");
            foreach (var skill in _characterSkillList)
            {
                // skill が null でないかも確認
                string skillName = skill != null ? skill.skillName : "NULL SkillData";
                Debug.Log($"[SkillController] Loaded skill: {skillName}");
            }
        }

        /// <summary>
        /// ページ内のスキルの項目をセットします。
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetPageSkill(int page, SelectionSkillUIController uiController)
        {
            _skillIdDictionary.Clear();
            int startIndex = page * 4;
            for (int i = 0; i < 4; i++) // UIスロットは常に4つなので、ループ回数を4に固定すると安全
            {
                int positionIndex = i; // UI上の位置は常に 0-3
                int listIndex = startIndex + i; // スキルリスト内のインデックス

                if (listIndex < _characterSkillList.Count)
                {
                    var skillData = _characterSkillList[listIndex];

                    bool canSelect = CanSelectSkill(skillData);
                    uiController.SetSkillText(
                        positionIndex,
                        skillData.skillName,
                        skillData.cost, // BTコスト
                        skillData.currentCount,
                        skillData.maxCount,
                        canSelect);

                    // uiController.SetDescriptionText(skillData.skillDesc); // 説明はカーソル選択時に更新するのが一般的
                    _skillIdDictionary.Add(positionIndex, skillData.skillId);
                }
                else
                {
                    uiController.ClearSkillText(positionIndex);
                }

                if (_skillIdDictionary.Count == 0)
                {
                    string noSkillText = "* 選択できるスキルがありません！ *";
                    uiController.SetDescriptionText(noSkillText);
                }
            }
        }

        /// <summary>
        /// スキルを使えるか確認します。
        /// </summary>
        /// <param name="skilllData">スキルデータ</param>
        bool CanSelectSkill(SkillData skillData)
        {
            if (skillData == null)
            {
                return false;
            }

            var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
            // BTが足りているか？
            bool hasEnoughBt = characterStatus.currentBt >= skillData.cost;

            // 使用回数が残っているか？ (maxCount < 0 は無制限)
            bool hasEnoughCount = skillData.maxCount < 0 || skillData.currentCount > 0;

            // 両方の条件を満たしている場合にのみ選択可能
            return hasEnoughBt && hasEnoughCount;
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public SkillData GetSkillData(int selectedIndex)
        {
            SkillData skillData = null;
            if (selectedIndex >= 0 && selectedIndex < _characterSkillList.Count)
            {
                skillData = _characterSkillList[selectedIndex];
            }

            return skillData;
        }
    }
}