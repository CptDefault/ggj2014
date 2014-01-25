using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class ShellRelease : MonoBehaviour
{
    public GameObject slug;
    public Transform slugPosition;

    public Vector3 offset;

    private float ejectForce = 3;

	protected void releaseSlug(AnimationEvent e)
	{
	    var newSlug = (GameObject)Instantiate(slug);

        newSlug.transform.parent = slugPosition;
        newSlug.transform.localPosition = offset;
        newSlug.transform.localRotation = Quaternion.identity;
        newSlug.transform.parent = null;
        var slugRigid = newSlug.AddComponent<Rigidbody>();

        slugRigid.AddForce(newSlug.transform.right * ejectForce, ForceMode.VelocityChange);
        slugRigid.AddTorque(Random.insideUnitSphere);

        Destroy(newSlug, 10);
	}
}
