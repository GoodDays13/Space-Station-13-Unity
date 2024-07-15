using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LOSWalls : MonoBehaviour
{
    public Tilemap walls;
    public Tilemap blocker;
    public LayerMask wallLayerMask;
    private Vector2[] cornerOffsets;

    // Start is called before the first frame update
    void Start()
    {
        cornerOffsets = new Vector2[6];
        cornerOffsets[0] = new Vector2(.28125f, .5f);
        cornerOffsets[1] = new Vector2(.5625f, 0);
        cornerOffsets[2] = new Vector2(.28125f, -.5f);
        cornerOffsets[3] = new Vector2(-.28125f, -.5f);
        cornerOffsets[4] = new Vector2(-.5625f, 0);
        cornerOffsets[5] = new Vector2(-.28125f, .5f);
            
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLOS();
    }

    private void UpdateLOS()
    {
        BoundsInt bounds = walls.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new(x, y);
                TileBase tile = walls.GetTile(cellPosition); 
                if (walls.GetTile(cellPosition) is null)
                    continue;

                bool blocked = true;
                foreach (Vector2 offset in cornerOffsets)
                {
                    Vector2 cellWorldPosition = walls.CellToWorld(cellPosition);
                    Vector2 playerPosition = transform.GetChild(0).position;
                    Vector2 difference = cellWorldPosition + offset * .95f - playerPosition;

                    RaycastHit2D ray = Physics2D.Raycast(playerPosition, difference, difference.magnitude - 0.1f, wallLayerMask);

                    if (ray.collider is null)
                    {
                        blocked = false; break;
                    }
                }
                blocker.SetTile(cellPosition, blocked ? tile : null);
            }
        }
    }
}
