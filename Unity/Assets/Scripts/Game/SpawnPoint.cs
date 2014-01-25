using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	
	public int usedCount = 0;

	// Use this for initialization
	void Start ()
	{
	    renderer.enabled = false;
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
	

}
