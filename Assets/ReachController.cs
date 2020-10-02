using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachController : MonoBehaviour
{
	public float radius = 0.5f;

	public bool CanReach(int layer)
	{
		return GetItems(layer).Length > 0;
	}

	public Collider[] GetItems(int layer)
	{
		return Physics.OverlapSphere(transform.position, radius, 1 << layer);
	}

	public GameObject GetClosest(int layer)
	{
		float minDist = Mathf.Infinity;
		Collider closest = new Collider();

		foreach (Collider collider in GetItems(layer))
		{
			float dist = Vector3.Distance(transform.position, collider.transform.position);

			if (dist < minDist)
			{
				minDist = dist;
				closest = collider;
			}
		}
		return closest.gameObject;
	}
}
