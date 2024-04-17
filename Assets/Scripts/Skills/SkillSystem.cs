using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    public static SkillSystem instance { get; private set; }

    //public event EventHandler OnActiveUnitChange;
    //public event EventHandler OnSelectedSkillChange;
    //public event EventHandler OnSkillStarted;
    //public event EventHandler<bool> OnBusyChange;

    private SkillBase skillSelected;
    private bool isBusy;

    [SerializeField] LayerMask unitLayerMask;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There's more than one SkillSystem!" + transform + " - " + instance);
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
}
