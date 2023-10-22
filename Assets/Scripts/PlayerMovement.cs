using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //set up the cooldown and speed of movement in the editor
    [SerializeField]
    private float startCooldown;
    [SerializeField]
    private float speed;

    void Start()
    {

    }

    void Update()
    {
        //handles movement of player using the variable "speed" in both vertical and horizontal
        transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0.0f, Input.GetAxis("Vertical") * speed * Time.deltaTime);

        //setting up raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
