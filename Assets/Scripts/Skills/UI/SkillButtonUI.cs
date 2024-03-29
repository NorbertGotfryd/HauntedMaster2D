using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private Button skillButton;

    public void SetSkillBase(SkillBase skillBase)
    {
        skillText.text = skillBase.GetSkillName().ToUpper();
    }
}
