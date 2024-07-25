using UnityEngine;

public class DOTStatus : StatusBase
{
    public override void Activate()
    {
        Debug.Log($"Activating {this.GetType().Name} on {targetCharacter.name} with power {power} and duration {duration}");
        targetCharacter.DamageCalculation(targetCharacter, power);
        DecreaseDuration();
    }
}
