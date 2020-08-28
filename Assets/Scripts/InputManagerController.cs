using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerController : MonoBehaviour
{
    private InputMaster controls;

    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new InputMaster();
        }
        controls.Player.Enable();
    }
    public void OnDisable()
    {
        controls.Player.Disable();
    }
}
