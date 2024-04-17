using UnityEngine;

public class MouseController : MonoBehaviour
{
    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
        }
    }
}
