using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite;
    private bool walking;
    private Transform sprite;
    private PlayerInput input;
    private Vector2 move;
    private int lastUsedInput = -1;

    // Start is called before the first frame update
    void Start()
    {
        sprite = transform.GetChild(0);
        input = GetComponent<PlayerInput>();
    }

    private void UpdateInputs()
    {
        var actions = input.actions;
        bool[] movement = new bool[]
        {
            actions.FindAction("UpLeft").IsPressed(),
            actions.FindAction("Up").IsPressed(),
            actions.FindAction("UpRight").IsPressed(),
            actions.FindAction("DownLeft").IsPressed(),
            actions.FindAction("Down").IsPressed(),
            actions.FindAction("DownRight").IsPressed()
        };

        int pressedCount = 0;
        int lastPressedIndex = -1;

        // Find the last pressed movement direction
        for (int i = 0; i < movement.Length; i++)
        {
            if (movement[i])
            {
                pressedCount++;
                lastPressedIndex = i;
            }
        }

        if (pressedCount == 2)
        {
            if (lastPressedIndex == lastUsedInput)
            {
                movement[lastUsedInput] = false;
                while (!movement[lastPressedIndex])
                    lastPressedIndex--;

            }
            pressedCount--;
        }

        if (pressedCount == 1)
        {
            lastUsedInput = lastPressedIndex;
            // Set move vector based on the last pressed direction
            switch (lastPressedIndex)
            {
                case 0:
                    move = new Vector2(-Mathf.Sqrt(3) / 2f, 0.5f);
                    break;
                case 1:
                    move = Vector2.up;
                    break;
                case 2:
                    move = new Vector2(Mathf.Sqrt(3) / 2f, 0.5f);
                    break;
                case 3:
                    move = new Vector2(-Mathf.Sqrt(3) / 2f, -0.5f);
                    break;
                case 4:
                    move = Vector2.down;
                    break;
                case 5:
                    move = new Vector2(Mathf.Sqrt(3) / 2f, -0.5f);
                    break;
            }
        }
        else move = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Moves the sprite closer to 0
        if (sprite.localPosition != Vector3.zero)
        {
            var movement = speed * Time.deltaTime * -sprite.localPosition.normalized;
            movement *= walking ? 0.5f : 1f;
            if (movement.magnitude > sprite.localPosition.magnitude)
            {
                sprite.localPosition = Vector3.zero;
            }
            else
            {
                sprite.localPosition += movement;
            }
        }


        if (sprite.localPosition == Vector3.zero) 
        {
            UpdateInputs();
            if (move != Vector2Int.zero)
            {
                walking = input.actions.FindAction("Walk").IsPressed();
                Move(move);
            }
        }
    }

    private void Move(Vector2 where, bool ignorePlayers = false)
    {
        Vector3 distance = new(where.x, where.y, 0);
        Collider2D collision = Physics2D.OverlapPoint(transform.position + distance);
        PlayerController player = null;

        if (collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (!ignorePlayers)
                    collision.GetComponent<PlayerController>().Move(-where, true);
                collision = null;
            }
            else if (collision.CompareTag("Item"))
                collision = null;
        }

        if (collision && player == null)
        {
            UpdateSprite(distance);
            return;
        }
        UpdateSprite(distance);

        Tilemap tilemap = GameObject.FindGameObjectWithTag("Floor").GetComponent<Tilemap>();
        Vector3Int cell = tilemap.WorldToCell(transform.position + distance);
        Vector3 target = tilemap.CellToWorld(cell);
        target.z = 0;
        var temp = transform.position;
        transform.position = target;
        sprite.position = temp;    
    }

    private void UpdateSprite(Vector3 direction)
    {
        var renderer = sprite.GetComponent<SpriteRenderer>();
        renderer.flipX = direction.x < 0;
        if (direction.x != 0)
            renderer.sprite = sideSprite;
        else
            renderer.sprite = direction.y < 0 ? frontSprite : backSprite;
    }
}
