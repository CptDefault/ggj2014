using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    private Moveable _moveable;
    private Weapon _weapon;
    private Player _player;

    public float vertLookSensitivity = 90;
    public float horizontalLookSensitivity = 200;

    public int vertLookInvert = 1;
    public float sensitivityScale = 1;

    private int pNo {
        get { return _player.playerNumber;}
    }

    protected void Awake()
    {
        _moveable = GetComponent<Moveable>();
        _weapon = GetComponent<Weapon>();
        _player = GetComponent<Player>();

        //pNo = PlayerPrefs.GetInt("NumPlayers")-_player.playerNumber;

        //set up defaults set at the join screen
        //vertLookInvert = PlayerPrefs.GetInt("P"+_player.playerNumber+"Inverted");
        //print("set inerted " + pNo + " " + vertLookInvert);
        sensitivityScale = 1;//PlayerPrefs.GetFloat("P"+_player.playerNumber+"sensitivityScale");
    }

    protected void Update()
    {
        _moveable.SetDesiredInput(new Vector2(Input.GetAxis("L_XAxis_" + pNo), Input.GetAxis("L_YAxis_" + pNo)));
        var hInput = sensitivityScale*Input.GetAxis("R_XAxis_" + pNo);
        var vInput = vertLookInvert*sensitivityScale*Input.GetAxis("R_YAxis_" + pNo);

        _weapon.ElevationInput(vInput * Mathf.Abs(vInput) * vertLookSensitivity * Time.deltaTime);
        _moveable.LookRelative(hInput * horizontalLookSensitivity * Time.deltaTime);

        if(Input.GetButtonDown("A_"+pNo))
            _moveable.Jump();

        if(Input.GetAxis("Triggers_"+pNo) < -0.3f)
            _weapon.Shoot();

        if(!GameSystem.Instance.adjustingControls && Input.GetButtonDown("Start_"+pNo))
            GameSystem.Instance.TogglePauseGame();
    }
}
