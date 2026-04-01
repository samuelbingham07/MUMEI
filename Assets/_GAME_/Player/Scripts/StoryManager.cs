using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    private int storyStage = 0;
    private Dictionary<string, int> talkCounts = new Dictionary<string, int>();

    private string SavePath => Application.persistentDataPath + "/storyData.json";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public int GetStoryStage() => storyStage;

    public void SetStoryStage(int stage)
    {
        storyStage = stage;
        Save();
    }

    public int GetTalkCount(string npcID)
    {
        return talkCounts.TryGetValue(npcID, out int count) ? count : 0;
    }

    public void IncrementTalkCount(string npcID)
    {
        talkCounts[npcID] = GetTalkCount(npcID) + 1;
        Save();
    }

    void Save()
    {
        StoryData data = new StoryData
        {
            storyStage = storyStage,
            talkCounts = new List<TalkCountEntry>()
        };

        foreach (var kvp in talkCounts)
            data.talkCounts.Add(new TalkCountEntry { npcID = kvp.Key, count = kvp.Value });

        File.WriteAllText(SavePath, JsonUtility.ToJson(data));
    }

    void Load()
    {
        if (!File.Exists(SavePath)) return;
        StoryData data = JsonUtility.FromJson<StoryData>(File.ReadAllText(SavePath));
        storyStage = data.storyStage;
        foreach (var entry in data.talkCounts)
            talkCounts[entry.npcID] = entry.count;
    }

    [System.Serializable]
    class StoryData
    {
        public int storyStage;
        public List<TalkCountEntry> talkCounts;
    }

    [System.Serializable]
    class TalkCountEntry
    {
        public string npcID;
        public int count;
    }
}
