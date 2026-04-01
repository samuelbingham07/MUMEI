using UnityEngine;
using UnityEngine.InputSystem;

public class NPCController : MonoBehaviour
{
    [SerializeField] private NPCDialogueData dialogueData;
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private GameObject spacebarIcon;

    private Transform player;

    void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
            Debug.LogError("NPCController: No GameObject tagged 'Player' found.");
        else
            player = playerObj.transform;

        if (spacebarIcon != null) spacebarIcon.SetActive(false);
    }


    void Update()
    {
        if (DialogueManager.IsOpen)
        {
            if (spacebarIcon != null) spacebarIcon.SetActive(false);
            return;
        }

        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool playerInRange = dist <= interactRadius;

        if (spacebarIcon != null) spacebarIcon.SetActive(playerInRange);

    }

    public void TriggerDialogue()
    {
        if (StoryManager.Instance == null) { Debug.LogError("StoryManager not found."); return; }
        if (DialogueManager.Instance == null) { Debug.LogError("DialogueManager not found."); return; }
        if (dialogueData == null) { Debug.LogError("No DialogueData assigned on NPC."); return; }

        int talkCount = StoryManager.Instance.GetTalkCount(dialogueData.npcID);
        int storyStage = StoryManager.Instance.GetStoryStage();

        // Walk through entries and keep the last one whose conditions are met
        DialogueEntry bestEntry = null;
        foreach (DialogueEntry entry in dialogueData.entries)
        {
            if (storyStage >= entry.minStoryStage && talkCount >= entry.minTalkCount)
                bestEntry = entry;
        }

        if (bestEntry == null) return;

        DialogueManager.Instance.StartDialogue(dialogueData.npcName, bestEntry.lines, () =>
        {
            StoryManager.Instance.IncrementTalkCount(dialogueData.npcID);
        });
    }

    // Draws interact radius in the Scene view when this NPC is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
