using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public int playerNumber;
	public int score = 0;
	public Rect crosshairRect;

	//score GUI indicator
	public GUIStyle scorePopUpStyle;
	private float _scorePopUpTime=10;
	private float _scorePopUpY;

	private Camera _myCamera;

	// Use this for initialization
	void Start () {
		_myCamera = GetComponentInChildren<Camera>();

		scorePopUpStyle.fontSize = (int)(Screen.height/20f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Respawn()
    {

        transform.position = Random.insideUnitCircle.XZ()*25 + Vector3.up*15;
    }

    public void GotHit(MonoBehaviour shooter)
    {
        Respawn();
        shooter.GetComponent<Player>().ScoreUp();
    }

    public void ScoreUp()
    {
    	_scorePopUpY = crosshairRect.y-Screen.height/7;
    	_scorePopUpTime = 0;

    	score++;
    }

    void OnGUI()
    {
    	if(_scorePopUpTime < 1.3f)
    	{
    		GUI.Box(new Rect(crosshairRect.x-Screen.height/32, _scorePopUpY, Screen.height/15, Screen.height/15), "+1", scorePopUpStyle);

    		_scorePopUpY -= 30*Time.deltaTime;

    	}
    	_scorePopUpTime += Time.deltaTime;

    }

}
