using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static SkillBase;

public class SkillButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private Button skillButton;

    private SkillBase skillBase;

    public void SetSkillBase(SkillBase skillBase)
    {
        this.skillBase = skillBase;
        skillText.text = skillBase.GetSkillName();

        skillButton.onClick.AddListener(() => {
            BattleHandler.instance.skillSelected = skillBase;
        });
    }

    public void UseSkill()
    {
        if (BattleHandler.instance.skillSelected != null)
        {
            BattleHandler.instance.skillSelected.UseSkill(() => {
                Debug.Log("Skill action complete");
            });
        }
    }
}
