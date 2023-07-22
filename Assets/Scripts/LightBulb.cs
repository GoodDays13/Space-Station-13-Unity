using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class LightBulb : Clickable
{
    public Sprite noBulb;
    public Sprite bulb;
    public GameObject bulbItem;

    public enum State
    {
        NoBulb,
        HasBulb
    }

    public State state;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.NoBulb:
                spriteRenderer.sprite = noBulb;
                GetComponentInChildren<Light2D>().enabled = false;
                break;
            case State.HasBulb:
                spriteRenderer.sprite = bulb;
                GetComponentInChildren<Light2D>().enabled = true;
                break;
        };
    }

    public override void WhenClicked(GameObject player)
    {
        var inv = player.GetComponent<Inventory>();
        if (!inv)
            return;
        //playerHasBulb = (inv.activeHandRight && inv.rightHand != null && inv.rightHand.displayName == "Light Bulb") ||
        //    (!inv.activeHandRight && inv.leftHand != null && inv.leftHand.displayName == "Light Bulb");
        Item activeHand = inv.activeHandRight ? inv.rightHand : inv.leftHand;

        if (state == State.NoBulb && activeHand != null && activeHand.displayName == "Light Bulb")
        {
            state = State.HasBulb;
            Destroy(inv.RemoveInHand(inv.activeHandRight).gameObject);
        }
        else if (state == State.HasBulb && activeHand == null)
        {
            state = State.NoBulb;
            inv.PutInHand(Instantiate(bulbItem, player.transform.position, Quaternion.identity), inv.activeHandRight);
        }

    }
}
