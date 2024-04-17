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

    //test
    [SerializeField] private SkillBase buttonHandler;

    private SkillBase skillBase;

    /*
    public void SetSkillBase(SkillBase skillBase)
    {
        this.skillBase = skillBase;
        skillText.text = skillBase.GetSkillName();

        skillButton.onClick.AddListener(() => {
            Debug.Log("test test test");
        });
    }
    */

    public void SetSkillBase(SkillBase skillBase)
    {
        this.skillBase = skillBase;
        skillText.text = skillBase.GetSkillName();

        skillButton.onClick.AddListener(() => {
            skillBase.UseSkill(BattleHandler.instance.GetTargetCharacter(), () => {
                Debug.Log("Action in progress");
            }, () => {
                Debug.Log("Action complete");
            });
        });
    }
}
