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

	public bool setControls;

	// Use this for initialization
	void Start () {
		_myCamera = GetComponentInChildren<Camera>();

		scorePopUpStyle.fontSize = (int)(Screen.height/20f);
	}
	
	// Update is called once per frame
	void Update () {
		

		if(setControls)
		{
			//invert Y
			if(Input.GetButtonDown("Y_"+playerNumber))
			    GetComponent<PlayerInput>().vertLookInvert *= -1;

			if(Input.GetAxis("DPad_XAxis_"+playerNumber) > 0 || Input.GetAxis("DPad_YAxis_"+playerNumber) > 0)
				GetComponent<PlayerInput>().sensitivityScale += 0.01f;
			else if(Input.GetAxis("DPad_XAxis_"+playerNumber) < 0 || Input.GetAxis("DPad_YAxis_"+playerNumber) < 0)
				GetComponent<PlayerInput>().sensitivityScale -= 0.01f;
		}
	}

    public void Respawn()
    {

        transform.position = Random.insideUnitCircle.XZ()*25 + Vector3.up*15;
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

    	if(setControls)
    	{
    		GUI.Box(new Rect(crosshairRect.x-Screen.height/5, crosshairRect.y-Screen.height/6, Screen.height/2.5F, Screen.height/3), "", GameSystem.Instance.setControlsSkin.GetStyle("Box"));

    		GUI.Box(new Rect(crosshairRect.x-Screen.height/8, crosshairRect.y-Screen.height/7f, Screen.height/4f, Screen.height/13), "SET CONTROLS",GameSystem.Instance.setControlsSkin.GetStyle("Title"));

    		string invertedText;

    		if(GetComponent<PlayerInput>().vertLookInvert == 1)
    			invertedText = "Y Look Normal";
    		else
    			invertedText = "Y Look Inverted";

    		GUI.Box(new Rect(crosshairRect.x-Screen.height/8, crosshairRect.y-Screen.height/20f, Screen.height/4f, Screen.height/13), invertedText, GameSystem.Instance.setControlsSkin.GetStyle("Button"));

    		GUI.Box(new Rect(crosshairRect.x-Screen.height/8, crosshairRect.y-Screen.height/20f+Screen.height/11f, Screen.height/4f, Screen.height/13), "Look Sensitivity", GameSystem.Instance.setControlsSkin.GetStyle("Button"));

    		GUI.skin = GameSystem.Instance.setControlsSkin;
    		GetComponent<PlayerInput>().sensitivityScale = GUI.HorizontalSlider(new Rect(crosshairRect.x-Screen.height/8, crosshairRect.y-Screen.height/20f+Screen.height/6f, Screen.height/4f, Screen.height/13), GetComponent<PlayerInput>().sensitivityScale, 0.5f, 2.0f);


    		//icons
    		GUI.Box(new Rect(crosshairRect.x-Screen.height/8.3f, crosshairRect.y-Screen.height/20f+Screen.height/40f, Screen.width/40f, Screen.width/40f) ,"", GameSystem.Instance.controllerIcons.GetStyle("Y"));

    		GUI.Box(new Rect(crosshairRect.x-Screen.height/8.3f, crosshairRect.y-Screen.height/20f+Screen.height/11f+Screen.height/40f, Screen.width/40f, Screen.width/40f) ,"", GameSystem.Instance.controllerIcons.GetStyle("Dpad"));

    	}

    }

}
