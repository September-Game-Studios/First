using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputMaster controls;

    private void Awake()
    {
        controls.Player.Jump.performed += _ => Jump();
    }

    public void Jump()
    {
        Debug.Log("We jumped!");
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
