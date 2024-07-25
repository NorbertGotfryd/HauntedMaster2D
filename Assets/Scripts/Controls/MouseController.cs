using UnityEngine;

public class MouseController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                CharacterBase clickedCharacter = hit.collider.GetComponent<CharacterBase>();
                if (clickedCharacter != null && BattleHandler.instance.characterEnemyList.Contains(clickedCharacter))
                {
                    BattleHandler.instance.targetCharacter = clickedCharacter;
                    BattleHandler.instance.PlayerTeamAction();
                }
            }
        }
    }
}