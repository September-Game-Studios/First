using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAreaController : MonoBehaviour
{

    public bool canGrab
    {
        get
        {
            return items.Length > 0;
        }
    }

    public Collider[] items
    {
        get
        {
            return Physics.OverlapSphere(transform.position, 0.5f, 1 << LayerMask.NameToLayer("Grabbable"));
        }
    }

    public GameObject closest
    {
        get
        {
            float minDist = Mathf.Infinity;
            Collider closest = new Collider();

            foreach (Collider c in items)
            {
                float dist = Vector3.Distance(transform.position, c.transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    closest = c;
                }
            }
            return closest.gameObject;
        }
    }
}
