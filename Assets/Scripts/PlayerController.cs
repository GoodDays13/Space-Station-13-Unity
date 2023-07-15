using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite;
    public bool moveRight;
    public bool moveLeft;
    public bool walking;
    private Transform sprite;
    private PlayerInput input;
    private Vector2Int move;

    // Start is called before the first frame update
    void Start()
    {
        sprite = transform.GetChild(0);
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        var moveInputs = Vector2Int.RoundToInt(input.actions.FindAction("Move").ReadValue<Vector2>());
        if (moveRight)
            moveInputs = Vector2Int.right;
        if (moveLeft)
            moveInputs = Vector2Int.left;
        if (moveInputs == Vector2Int.zero)
            move = Vector2Int.zero;
        else if (sprite.localPosition.x == 0 || sprite.localPosition.y == 0) // currently moving horizontally
        {
            if (moveInputs.x != 0)
                move.Set(moveInputs.x, move.y);
            if (moveInputs.y != 0)
                move.Set(move.x, moveInputs.y);
        }
        else
        {
            if (moveInputs.x == 0)
                move.Set(moveInputs.x, move.y);
            if (moveInputs.y == 0)
                move.Set(moveInputs.y, 0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // Moves the sprite closer to 0
        if (sprite.localPosition != Vector3.zero)
        {
            var movement = -sprite.localPosition.normalized * speed * Time.deltaTime;
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

        if (sprite.localPosition == Vector3.zero && move != Vector2Int.zero) 
        {
            walking = input.actions.FindAction("Walk").IsPressed();
            Move(move);
        }
    }

    private void Move(Vector2Int where, bool ignorePlayers = false)
    {
        Vector3Int distance = new(where.x, where.y, 0);
        Collider2D collision = Physics2D.OverlapPoint(transform.position + distance);
        PlayerController player = null;

        if (collision && collision.CompareTag("Player") && (distance.x == 0 || distance.y == 0))
        {
            if (!ignorePlayers)
                collision.GetComponent<PlayerController>().Move(-where, true);
            collision = null;
        }

        if (collision && player == null)
        {
            if (distance.x == 0 || distance.y == 0)
            {
                UpdateSprite(distance);
                return;
            }

            Vector3Int hori = new(where.x, 0, 0);
            Vector3Int vert = new(0, where.y, 0);
            bool canMoveHori = !PointInCollider(transform.position + hori);
            bool canMoveVert = !PointInCollider(transform.position + vert);
            if (canMoveHori && !canMoveVert)
                distance = hori;
            else if (canMoveVert && !canMoveHori)
                distance = vert;
            else if (canMoveHori && canMoveVert)
                distance = vert;
            else return;
        }
        UpdateSprite(distance);
        
        transform.position += distance;
        sprite.position -= distance;
            
    }

    private void UpdateSprite(Vector3Int direction)
    {
        var renderer = sprite.GetComponent<SpriteRenderer>();
        renderer.flipX = direction.x < 0;
        if (direction.x != 0)
            renderer.sprite = sideSprite;
        else
            renderer.sprite = direction.y < 0 ? frontSprite : backSprite;
    }

    private bool PointInCollider(Vector2 point)
    {
        return Physics2D.OverlapPoint(point) != null;
    }
}
