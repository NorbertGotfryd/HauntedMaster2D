using System;
using UnityEngine;

public class SkillsButonsContainerUI : MonoBehaviour
{
    [SerializeField] private Transform skillButtonPrefab;
    [SerializeField] private Transform skillContainerTransform;

    private void Start()
    {
        BattleHandler.instance.OnActiveUnitChanged += BattleHandler_OnActiveUnitChanged;
        CreateSkillButtons();
    }

    private void CreateSkillButtons()
    {
        foreach (Transform skillButtonPrefab in skillContainerTransform)
            Destroy(skillButtonPrefab.gameObject);

        CharacterBase selectedCharacter = BattleHandler.instance.GetActiveCharacter();

        foreach (SkillBase skillBase in selectedCharacter.GetSkillsBaseArray())
        {
            Transform skillButtonTransform = Instantiate(skillButtonPrefab, skillContainerTransform);
            SkillButtonUI skillButtonUI = skillButtonTransform.GetComponent<SkillButtonUI>();
            skillButtonUI.SetSkillBase(skillBase);
        }
    }

    private void BattleHandler_OnActiveUnitChanged(object sender, EventArgs e)
    {
        CreateSkillButtons();
    }
}
