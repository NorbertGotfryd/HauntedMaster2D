using UnityEngine;

public class HOTStatus : StatusBase
{
    public override void Activate()
    {
        Debug.Log($"Activating {this.GetType().Name} on {targetCharacter.name} with power {power} and duration {duration}");
        targetCharacter.HealingCalculation(targetCharacter, power);
        DecreaseDuration();
    }
}
