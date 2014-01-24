using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{

    public Transform weaponTransform;

    public float minElevation = -70;
    public float maxElevation = 70;

    public float elevationSpeed = 180;

    private float _elevation;
    private float _targetElevation;

    public GameObject muzzleFlash;

    public void ElevationInput(float angle)
    {
        _elevation += Mathf.Clamp(angle, -elevationSpeed, elevationSpeed);
    }

    protected void Update()
    {
        _elevation = Mathf.Clamp(_elevation, minElevation, maxElevation);
        weaponTransform.localRotation = Quaternion.Euler(_elevation, 0, 0);
    }

    public void Shoot()
    {
        if(muzzleFlash != null)
            Instantiate(muzzleFlash, weaponTransform.position, weaponTransform.rotation);
    }
}
