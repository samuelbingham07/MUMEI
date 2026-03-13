using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour
{

    [SerializeField] PolygonCollider2D mapBoundry;
    [SerializeField] CinemachineConfiner2D confiner; 
    [SerializeField] Direction direction;
    [SerializeField] float additivePos = 2f;
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner2D>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            confiner.BoundingShape2D = mapBoundry;
            UpdatePlayerPosition(collision.gameObject);
        }

    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;
        switch (direction)
        {
            case Direction.Up:
                newPos.y += additivePos;
                break;
            case Direction.Down:
                newPos.y -= additivePos;
                break;
            case Direction.Left:
                newPos.x -= additivePos;
                break;
            case Direction.Right:
                newPos.x += additivePos;
                break;
        }
        player.transform.position = newPos;
    }
}
