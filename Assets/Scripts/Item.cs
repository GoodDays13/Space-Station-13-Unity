using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Clickable
{
    public string displayName;
    public Sprite sprite;
    public Sprite leftHandSprite;
    public Sprite rightHandSprite;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var inv = GetComponentInParent<Inventory>();

        if (inv == null)
        {
            spriteRenderer.sprite = sprite;
        }
        else if (transform.localPosition == Vector3.zero)
        {
            if (inv.leftHand == this)
            {
                spriteRenderer.sprite = leftHandSprite;
            }
            else if (inv.rightHand == this)
            {
                spriteRenderer.sprite = rightHandSprite;
            }

        }
    }

    private void FixedUpdate()
    {
        if (transform.parent != null && transform.localPosition != Vector3.zero)
        {
            var movement = 10 * Time.deltaTime * -transform.localPosition.normalized;
            if (movement.magnitude > transform.localPosition.magnitude)
            {
                transform.localPosition = Vector3.zero;
            }
            else
            {
                transform.localPosition += movement;
            }
        }
    }

    public override void WhenClicked(GameObject player)
    {
        var inv = player.GetComponent<Inventory>();
        if (!inv)
            return;

        inv.PutInHand(this, inv.activeHandRight);
    }
}
