﻿using System.Linq;
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
    public GameObject particles;
    private GameSystem _gameSystem;

    public float gunRange = 20;
    public float gunCone = 20;

    private bool _isLoaded = true;

    [System.Serializable]
    public class ConePoints
    {
        public float distance;
        public float diameter;

        public ConePoints(float distance = 0, float diameter = 2)
        {
            this.distance = distance;
            this.diameter = diameter;
        }
    }

    public ConePoints[] cone = new ConePoints[]{new ConePoints(0,0), new ConePoints(10, 2), new ConePoints(100,2), };
    public int reloadTime = 2;

    protected void Awake()
    {
        _gameSystem = FindObjectOfType<GameSystem>();
    }

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
        if (!_isLoaded)
            return;

        if (muzzleFlash != null)
        {
            var flash = (GameObject)Instantiate(muzzleFlash, weaponTransform.position + weaponTransform.forward - weaponTransform.up * 0.2f + weaponTransform.right  *0.2f, weaponTransform.rotation * Quaternion.Euler(0, 180, 0));
            flash.transform.parent = weaponTransform;
            Destroy(flash, 0.2f);
        }
        if (particles != null)
        {
            var part = Instantiate(particles, weaponTransform.position + weaponTransform.forward, weaponTransform.rotation);
            Destroy(part, 1f);
        }

        var hitPlayers = _gameSystem.players.Where(TestPlayerHit);

        foreach (var hitPlayer in hitPlayers)
        {
            if(hitPlayer == gameObject)
                continue;
            
            hitPlayer.SendMessage("GotHit", this);
        }

        _isLoaded = false;
        Invoke("Reloaded", reloadTime);
    }

    protected void Reloaded()
    {
        _isLoaded = true;
    }

    private bool TestPlayerHit(GameObject player)
    {
        var origin = weaponTransform.position;
        var direction = weaponTransform.forward;

        float distance = Vector3.Dot(direction, player.transform.position - origin);
        var closestPoint = player.collider.ClosestPointOnBounds(origin + distance*direction);

        var dispFromCenter = closestPoint - origin;
        dispFromCenter -= direction*Vector3.Dot(dispFromCenter, direction);

        int coneIndex = 0;
        while (coneIndex < cone.Length && cone[coneIndex].distance < distance)
        {
            coneIndex++;
        }
        print(player.name + " : " + distance);

        if (coneIndex >= cone.Length)
            return false; //target is out of range

        if (coneIndex <= 0)
            return false; //target is less than minumum range (probably behind shooter)

        if((closestPoint - origin).magnitude > 0.1f
            && Physics.Raycast(origin, closestPoint - origin, (closestPoint - origin).magnitude  - 0.1f, ~(1 << 8))
            && (player.transform.position - origin).magnitude > 0.1f
            && Physics.Raycast(origin, player.transform.position - origin, (player.transform.position - origin).magnitude - 0.1f, ~(1 << 8)))
                    return false;

        float segProg = (distance - cone[coneIndex-1].distance) / (cone[coneIndex].distance - cone[coneIndex-1].distance);

        print(player.name + " : " + dispFromCenter.magnitude + " < " + Mathf.Lerp(cone[coneIndex - 1].diameter, cone[coneIndex].diameter, segProg) / 2);

        return dispFromCenter.magnitude < Mathf.Lerp(cone[coneIndex-1].diameter, cone[coneIndex].diameter, segProg) / 2;
    }
}
