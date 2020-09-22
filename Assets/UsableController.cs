using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableController : MonoBehaviour
{
    public void Use()
    {
        Debug.Log("USED ME");
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
