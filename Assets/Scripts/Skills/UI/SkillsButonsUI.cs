using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillsButonsUI : MonoBehaviour
{
    [SerializeField] private GameObject skillButtonPrefab;
    [SerializeField] private GameObject skillContainerGameObject;

    private void Start()
    {
        CreateSkillButtons();
    }

    private void CreateSkillButtons()
    {
        CharacterBase selectedCharacter = BattleHandler.instance.activeCharacter;

        foreach (SkillBase skillBase in selectedCharacter.GetSkillsBaseArray())
        {
            Instantiate(skillButtonPrefab.transform, skillContainerGameObject.transform);
        }
    }
}
