using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public bool activeHandRight;
    public Item leftHand;
    public Item rightHand;
    public Canvas UI;
    private Image selectorImage;
    private Image leftItemImage;
    private Image rightItemImage;
    private PlayerInput input;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        Image[] images = UI.GetComponentsInChildren<Image>();
        foreach (var item in images)
        {
            switch (item.name)
            {
                case "Selected Hand":
                    selectorImage = item; break;
                case "Left Item":
                    leftItemImage = item; break;
                case "Right Item":
                    rightItemImage = item; break;
            }
        }

        leftItemImage.enabled = false;
        rightItemImage.enabled = false;
        selectorImage.transform.position = activeHandRight ? rightItemImage.transform.position : leftItemImage.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.actions.FindAction("Switch Hands").WasPressedThisFrame())
            SwitchActiveHand();
        if (input.actions.FindAction("Drop").WasPressedThisFrame())
            RemoveInHand(activeHandRight);
    }

    /// <summary>
    /// Tries to put an item in the specified hand.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="inRightHand"></param>
    /// <returns>If it successfully put the item in.</returns>
    public bool PutInHand(Item item, bool inRightHand)
    {
        ref Item hand = ref (inRightHand ? ref rightHand : ref leftHand);
        if (hand != null)
            return false;

        hand = item;
        item.transform.parent = transform;

        Image handImage = inRightHand ? rightItemImage : leftItemImage;
        handImage.enabled = true;
        handImage.sprite = hand.sprite;
        handImage.SetNativeSize();

        return true;
    }

    public bool PutInHand(GameObject item, bool inRightHand)
    {
        return PutInHand(item.GetComponent<Item>(), inRightHand);
    }

    public Item RemoveInHand(bool inRightHand)
    {
        ref Item hand = ref (inRightHand ? ref rightHand : ref leftHand);
        Item r = hand;
        hand = null;
        r.transform.parent = null;
        (inRightHand ? rightItemImage : leftItemImage).enabled = false;
        return r;
    }

    public void SwitchActiveHand()
    {
        if (activeHandRight)
        {
            activeHandRight = false;
            selectorImage.transform.position = leftItemImage.transform.position;
        }
        else
        {
            activeHandRight= true;
            selectorImage.transform.position = rightItemImage.transform.position;
        }
    }
}
