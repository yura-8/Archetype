using UnityEngine;

public class CeilingController : MonoBehaviour
{
    [SerializeField] private GameObject ceilingObject; 
    private bool isInClassroom = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInClassroom = true;
            UpdateCeilingVisibility();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInClassroom = false;
            UpdateCeilingVisibility();
        }
    }

    void UpdateCeilingVisibility()
    {
        if (ceilingObject != null)
        {
            ceilingObject.GetComponent<SpriteRenderer>().enabled = !isInClassroom; 
        }
    }
}