using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public static bool IsOpen => Instance != null && Instance.dialogueCanvas != null && Instance.dialogueCanvas.activeSelf;

    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentLineIndex;
    private Action onFinished;
    private PlayerMovement playerMovement;

    void Awake()
    {
        Instance = this;
        dialogueCanvas.SetActive(false);
    }

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    public void StartDialogue(string npcName, string[] lines, Action onFinished = null)
    {
        currentLines = lines;
        currentLineIndex = 0;
        this.onFinished = onFinished;

        npcNameText.text = npcName;
        dialogueText.text = lines[0];
        dialogueCanvas.SetActive(true);
        playerMovement?.SetMovementLocked(true);
    }


    public void Advance()
    {
        currentLineIndex++;
        if (currentLineIndex >= currentLines.Length)
        {
            Close();
            return;
        }
        dialogueText.text = currentLines[currentLineIndex];
    }

    void Close()
    {
        dialogueCanvas.SetActive(false);
        playerMovement?.SetMovementLocked(false);
        onFinished?.Invoke();
    }
}
