using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    private Moveable _moveable;
    private Weapon _weapon;

    public float vertLookSensitivity = 90;
    public float horizontalLookSensitivity = 200;

    private int pNo {
        get { return 1; }
    }

    protected void Awake()
    {
        _moveable = GetComponent<Moveable>();
        _weapon = GetComponent<Weapon>();
    }

    protected void Update()
    {
        _moveable.SetDesiredInput(new Vector2(Input.GetAxis("L_XAxis_" + pNo), Input.GetAxis("L_YAxis_" + pNo)));
        var hInput = Input.GetAxis("R_XAxis_" + pNo);
        var vInput = Input.GetAxis("R_YAxis_" + pNo);

        _weapon.ElevationInput(vInput * Mathf.Abs(vInput) * vertLookSensitivity * Time.deltaTime);
        _moveable.LookRelative(hInput * horizontalLookSensitivity * Time.deltaTime);
    }
}
