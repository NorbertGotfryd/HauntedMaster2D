using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillsButonsContainerUI : MonoBehaviour
{
    [SerializeField] private GameObject skillButtonPrefab;
    [SerializeField] private GameObject skillContainerGameObject;

    private void Start()
    {
        BattleHandler.instance.OnActiveUnitChanged += BattleHandler_OnSelectedUnitChanged;
        //CreateSkillButtons();
    }

    private void Update()
    {
        foreach (Transform skillButtonPrefab in skillContainerGameObject.transform)
            Destroy(skillButtonPrefab.gameObject);

        CharacterBase selectedCharacter = BattleHandler.instance.activeCharacter;

        foreach (SkillBase skillBase in selectedCharacter.GetSkillsBaseArray())
        {
            Transform skillButtonTransform = Instantiate(skillButtonPrefab.transform, skillContainerGameObject.transform);
            SkillButtonUI skillButtonUI = skillButtonPrefab.GetComponent<SkillButtonUI>();
            skillButtonUI.SetSkillBase(skillBase);
        }
    }

    private void BattleHandler_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        //CreateSkillButtons();
    }
}
