using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkBehindTileFader : MonoBehaviour
{
    [SerializeField] private float fadedAlpha = 0.5f;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private int radius = 1;

    private Tilemap tilemap;
    private Transform player;

    // Tracks current alpha for each tile we've touched
    private Dictionary<Vector3Int, float> tileAlphas = new Dictionary<Vector3Int, float>();
    private HashSet<Vector3Int> tilesToFade = new HashSet<Vector3Int>();

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        tilesToFade.Clear();

        // Find all tiles within radius of the player
        Vector3Int playerCell = tilemap.WorldToCell(player.position);
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int pos = new Vector3Int(playerCell.x + x, playerCell.y + y, 0);
                if (tilemap.HasTile(pos))
                    tilesToFade.Add(pos);
            }
        }

        // Register any newly found tiles so we can start tracking their alpha
        foreach (Vector3Int pos in tilesToFade)
        {
            if (!tileAlphas.ContainsKey(pos))
            {
                tilemap.SetTileFlags(pos, TileFlags.None);
                tileAlphas[pos] = 1f;
            }
        }

        // Lerp all tracked tiles toward their target alpha
        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach (Vector3Int pos in new List<Vector3Int>(tileAlphas.Keys))
        {
            float target = tilesToFade.Contains(pos) ? fadedAlpha : 1f;
            float newAlpha = Mathf.MoveTowards(tileAlphas[pos], target, fadeSpeed * Time.deltaTime);
            tileAlphas[pos] = newAlpha;

            Color c = tilemap.GetColor(pos);
            c.a = newAlpha;
            tilemap.SetColor(pos, c);

            // Once fully restored, stop tracking this tile
            if (Mathf.Approximately(newAlpha, 1f) && !tilesToFade.Contains(pos))
                toRemove.Add(pos);
        }

        foreach (Vector3Int pos in toRemove)
            tileAlphas.Remove(pos);
    }
}
