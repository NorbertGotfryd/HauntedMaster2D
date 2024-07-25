using System;
using UnityEngine;

public abstract class StatusBase : MonoBehaviour
{
    protected int power;
    protected int duration;
    protected CharacterBase targetCharacter;

    public virtual void Initialize(int power, int duration, CharacterBase targetCharacter)
    {
        this.power = power;
        this.duration = duration;
        this.targetCharacter = targetCharacter;
    }

    public abstract void Activate();

    public void DecreaseDuration()
    {
        duration--;
        if (duration <= 0)
            RemoveStatus();
    }

    protected void RemoveStatus()
    {
        Debug.Log($"{this.GetType().Name} removed from {targetCharacter.name}");
        targetCharacter.RemoveStatus(this);
        Destroy(this);
    }

    public int GetPower() => power;
    public int GetDuration() => duration;
    public void AddPower(int additionalPower) => power += additionalPower;
}
