using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private float interactRadius = 3f;

    void OnEnable()
    {
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        // If dialogue is open, advance it
        if (DialogueManager.IsOpen)
        {
            DialogueManager.Instance.Advance();
            return;
        }

        // Otherwise find the closest NPC in range and trigger dialogue
        NPCController closest = null;
        float closestDist = Mathf.Infinity;

        foreach (NPCController npc in FindObjectsByType<NPCController>(FindObjectsSortMode.None))
        {
            float dist = Vector2.Distance(transform.position, npc.transform.position);
            if (dist <= interactRadius && dist < closestDist)
            {
                closest = npc;
                closestDist = dist;
            }
        }

        closest?.TriggerDialogue();
    }
}
