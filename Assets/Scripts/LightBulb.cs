using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class LightBulb : Clickable
{
    public Sprite noBulb;
    public Sprite bulb;

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
        if ((player.transform.position - transform.position).magnitude > 1)
            return;
        if (state == State.NoBulb)
            state = State.HasBulb;
        else
            state = State.NoBulb;

    }
}
