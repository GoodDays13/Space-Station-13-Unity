using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.InputSystem;

public abstract class Clickable : MonoBehaviour
{
    void OnMouseDown()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PlayerInput input = player.GetComponent<PlayerInput>();
            if (input && input.playerIndex == 0 && ((Vector2)player.transform.position - (Vector2)transform.position).magnitude <= 1.01)
                WhenClicked(player);
        }
    }

    public abstract void WhenClicked(GameObject player);
}
