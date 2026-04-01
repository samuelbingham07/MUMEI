using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialogue", menuName = "Dialogue/NPC Dialogue Data")]
public class NPCDialogueData : ScriptableObject
{
    public string npcID;    // unique ID used for save data (no spaces)
    public string npcName;  // displayed in the dialogue box
    public DialogueEntry[] entries;
}

[System.Serializable]
public class DialogueEntry
{
    [TextArea(2, 6)]
    public string[] lines;
    public int minStoryStage = 0;  // only show if story stage is >= this
    public int minTalkCount = 0;   // only show if player has talked to this NPC >= this many times
}
