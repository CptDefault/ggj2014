using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public int playerNumber;
	public int score = 0;
	public int deaths = 0;
    public Rect crosshairRect {
        get { return new Rect(crosshairPos.x * Screen.width-Screen.width/30f, (1-crosshairPos.y) * Screen.height-Screen.width/30f, Screen.width/15f, Screen.width/15f);}
    }

    public bool ShotReady
    {
        get { return gameObject.activeSelf && _weapon.ShotReady; }
    }

    //score GUI indicator
	public GUIStyle scorePopUpStyle;
	private float _scorePopUpTime=10;
	private float _scorePopUpY;

	private Camera _myCamera;

    private RespawnCamera _respawnCamera;
	public bool setControls;
    public Vector2 crosshairPos;
    private Weapon _weapon;

    public string name;
    public Color col;

    //game mode
    public bool isJuggernaut;
    

	//sound
	public AudioSource deathSound;
    public GameObject ragdoll;

    // Use this for initialization
    protected void Awake()
    {
        _weapon = GetComponent<Weapon>();

        _myCamera = GetComponentInChildren<Camera>();        
    }

    // Use this for initialization
	void Start () {

		scorePopUpStyle.fontSize = (int)(Screen.height/20f);

	    foreach (Transform child in transform)
	    {
	        SetChildLayerRecursive(child, 8 + playerNumber);
	    }
	    foreach (var cam in GetComponentsInChildren<Camera>())
	    {
	        for (int i = 1; i <= 4; i++)
	        {
	            if (i != playerNumber)
                    cam.cullingMask = cam.cullingMask & ~(1 << 8 + i);
	        }

	    }

	    //SET INVERTED
	    GetComponent<PlayerInput>().vertLookInvert = PlayerPrefs.GetInt("P"+playerNumber+"Inverted");

	    //SetPlayerAsJuggernaut();
	}

    private RespawnCamera GetRespawnCamera()
    {
        if (_respawnCamera == null)
        {
            var camGO = new GameObject("Player" + playerNumber + "RespawnCam");
            _respawnCamera = camGO.AddComponent<RespawnCamera>();
            var cam = camGO.AddComponent<Camera>();
            cam.rect = _myCamera.rect;
            cam.cullingMask = _myCamera.cullingMask;
        }

        _respawnCamera.transform.position = _myCamera.transform.position;
        _respawnCamera.transform.rotation = _myCamera.transform.rotation;

        iTween.MoveTo(_respawnCamera.gameObject,
            iTween.Hash(
            "position", _respawnCamera.transform.position + transform.up * 1 + transform.forward * -5,
            "time", 4,
            "looktarget", transform)
            );

        return _respawnCamera;
    }

    private void SetChildLayerRecursive(Transform t, int layer)
    {
        if(!t.GetComponent<ParticleSystem>())
	        t.gameObject.layer = layer;
        foreach (Transform child in t)
        {
            SetChildLayerRecursive(child, layer);
        }
    }

    public void SetPlayerAsJuggernaut()
    {

    	//set weapon
    	_weapon.SetJuggernaut();

    	isJuggernaut = true;
    }

    // Update is called once per frame
	void Update () {
		

		if(setControls)
		{
			//invert Y
			if(Input.GetButtonDown("Y_"+playerNumber)) {
			    GetComponent<PlayerInput>().vertLookInvert *= -1;
			    Clicker.Instance.Click();
			    PlayerPrefs.SetInt("P"+playerNumber+"Inverted",  GetComponent<PlayerInput>().vertLookInvert);
			}

			if(Input.GetAxis("DPad_XAxis_"+playerNumber) > 0 || Input.GetAxis("DPad_YAxis_"+playerNumber) > 0) 
				GetComponent<PlayerInput>().sensitivityScale += 0.01f;
			else if(Input.GetAxis("DPad_XAxis_"+playerNumber) < 0 || Input.GetAxis("DPad_YAxis_"+playerNumber) < 0)
				GetComponent<PlayerInput>().sensitivityScale -= 0.01f;

			 PlayerPrefs.SetFloat("P"+playerNumber+"sensitivityScale",  GetComponent<PlayerInput>().sensitivityScale);
		}

        if (_messageOffset > 0)
    	    _messageOffset -= Time.deltaTime*80;
	    if (_messageOffset < 0)
	        _messageOffset = 0;
	}

    public void Respawn()
    {
        var safestSpawnpoint = GameSystem.Instance.GetSafestSpawnpoint();
        transform.position = safestSpawnpoint.transform.position; //Random.insideUnitCircle.XZ()*25 + Vector3.up*15;
        transform.rotation = safestSpawnpoint.transform.rotation;
        GetComponent<Moveable>().GetRotationFromFacing();

        _weapon.ResetFromJuggernaut();
        isJuggernaut = false;
    }

    public void GotHit(Weapon shooter)
    {
        if(isJuggernaut)
        {
        	shooter.GetComponent<Player>().SetPlayerAsJuggernaut();
        }

        //AudioSource.PlayClipAtPoint(deathSound.clip, transform.position, deathSound.volume);
        _scoredMessages.Clear();
        deaths++;
        Player sp=null;
        //string verb = DeathMessenger.Instance.GetRandomVerb();
        var killMessage = DeathMessenger.Instance.GetRandomMessage();
        if(shooter != null) {
        	//set death message for me, and score for scorer
        	sp = shooter.GetComponent<Player>();

            sp.SetScoredMessage(string.Format(killMessage.killerMessage, name, sp.name));
            if(GameSystem.Instance.CurrentGameMode != GameSystem.GameMode.Elimination) {
            	var plShoot = shooter.GetComponent<Player>();

            	if(GameSystem.Instance.CurrentGameMode != GameSystem.GameMode.Juggernaut || (isJuggernaut || plShoot.isJuggernaut))
                	plShoot.ScoreUp();
            }

        }
        
        var rag = (GameObject)Instantiate(ragdoll, transform.position, transform.rotation);

        rag.GetComponentInChildren<Renderer>().material.color = col;

        _weapon.InstantReload();

        foreach (var rigid in rag.GetComponentsInChildren<Rigidbody>())
        {
            rigid.AddForce(rigidbody.velocity, ForceMode.VelocityChange);
            var characterJoint = rigid.GetComponent<CharacterJoint>();
            if(characterJoint)
                Destroy(characterJoint, 14.95f);
            Destroy(rigid, 15);
        }

        GetRespawnCamera().Activate(this, string.Format(killMessage.victimMessage, name, sp.name), new Rect(crosshairRect.center.x - Screen.width / 4.4f, crosshairRect.center.y, Screen.width / 2.2f, Screen.height / 15f));
    }

    public void ScoreUp()
    {
    	_scorePopUpY = crosshairRect.y-Screen.height/7;
    	_scorePopUpTime = 0;

    	score++;

    	AudioManager.Instance.PlayKillConfirmed();
    }

    public void SetPlayerNameAndColor(int num)
    {
    	playerNumber = num;
    	
    	switch(playerNumber)
    	{
    		case 1: 
    			name = "RED";
    			col = GameSystem.Instance.RedMat.color;
    			break;

    		case 2:
    			name = "BLUE";
    			col = GameSystem.Instance.BlueMat.color;
    			break;

    		case 3: 
    			name = "GREEN";
    			col = GameSystem.Instance.GreenMat.color;
    			break;

    		case 4:
    			name = "YELLOW";
    			col = GameSystem.Instance.YellowMat.color;
    			break;
    	}
    }

    private Queue<string> _scoredMessages = new Queue<string>();
    public bool scoredMessageLow = false;
    private float _messageOffset;
    private int _killStreak;
    void SetScoredMessage(string verb, string receiever)
    {
        SetScoredMessage("YOU " + verb + " " + receiever);
    }
    void SetScoredMessage(string message)
    {
    	_scoredMessages.Enqueue(message);

        _killStreak++;

        StartCoroutine(ShowScoredMessage());

        switch (_killStreak)
        {
            case 2:
                _scoredMessages.Enqueue("DOUBLE KILL");
    	        StartCoroutine(ShowScoredMessage());
                break;
            case 3:
                _scoredMessages.Enqueue("TRIPLE KILL");
    	        StartCoroutine(ShowScoredMessage());
                break;
            case 4:
                _scoredMessages.Enqueue("LUCKY STREAK");
    	        StartCoroutine(ShowScoredMessage());
                break;
            case 5:
                _scoredMessages.Enqueue("DREAM CRUSHER");
    	        StartCoroutine(ShowScoredMessage());
                break;
        }
    }
    IEnumerator ShowScoredMessage()
    {
    	yield return new WaitForSeconds(2.0f + 0.6f * _scoredMessages.Count);

        

        _scoredMessages.Dequeue();

        if (_scoredMessages.Count > 0)
        {
            _messageOffset += Screen.height/15f*1.2f;
        }
        else
        {
            _killStreak = 0;
        }
        //_scoredMessages = null;
    }

    void OnGUI()
    {
    	//died

		if(_scoredMessages != null)
		{
		    float top;
			if(!scoredMessageLow)
			{
			    top = crosshairRect.y - Screen.height/5.5f;
			}
			else
			{
			    top = crosshairRect.y + Screen.height/5.5f;
			}
		    top += _messageOffset * (scoredMessageLow ? -1 : 1);

		    foreach (var scoredMessage in _scoredMessages)
		    {
                GUI.Box(new Rect(crosshairRect.center.x - Screen.width / 4.4f, top, Screen.width / 2.2f, Screen.height / 15f), scoredMessage, DeathMessenger.Instance.messageSkin.GetStyle("Message"));
                top += Screen.height / 15f * 1.2f * (scoredMessageLow ? -1 : 1);
		    }

		}

    	if(_scorePopUpTime < 1.3f)
    	{
    		GUI.Box(new Rect(crosshairRect.center.x-Screen.height/32, _scorePopUpY, Screen.height/15, Screen.height/15), "+1", scorePopUpStyle);

    		_scorePopUpY -= 30*Time.deltaTime;

    	}

    	_scorePopUpTime += Time.deltaTime;

    	if(setControls)
    	{
    		GUI.Box(new Rect(crosshairRect.center.x-Screen.height/5, crosshairRect.center.y-Screen.height/6, Screen.height/2.5F, Screen.height/3), "", GameSystem.Instance.setControlsSkin.GetStyle("Box"));

    		GUI.Box(new Rect(crosshairRect.center.x-Screen.height/8, crosshairRect.center.y-Screen.height/7f, Screen.height/4f, Screen.height/13), "SET CONTROLS",GameSystem.Instance.setControlsSkin.GetStyle("Title"));

    		string invertedText;

    		if(GetComponent<PlayerInput>().vertLookInvert == 1)
    			invertedText = "Y Look Normal";
    		else
    			invertedText = "Y Look Inverted";

    		GUI.Box(new Rect(crosshairRect.center.x-Screen.height/8, crosshairRect.center.y-Screen.height/20f, Screen.height/4f, Screen.height/13), invertedText, GameSystem.Instance.setControlsSkin.GetStyle("Button"));

    		GUI.Box(new Rect(crosshairRect.center.x-Screen.height/8, crosshairRect.center.y-Screen.height/20f+Screen.height/11f, Screen.height/4f, Screen.height/13), "Look Sensitivity", GameSystem.Instance.setControlsSkin.GetStyle("Button"));

    		GUI.skin = GameSystem.Instance.setControlsSkin;
    		GetComponent<PlayerInput>().sensitivityScale = GUI.HorizontalSlider(new Rect(crosshairRect.center.x-Screen.height/8, crosshairRect.center.y-Screen.height/20f+Screen.height/6f, Screen.height/4f, Screen.height/13), GetComponent<PlayerInput>().sensitivityScale, 0.5f, 2.0f);


    		//icons
    		GUI.Box(new Rect(crosshairRect.center.x-Screen.height/8.3f, crosshairRect.center.y-Screen.height/20f+Screen.height/40f, Screen.width/40f, Screen.width/40f) ,"", GameSystem.Instance.controllerIcons.GetStyle("Y"));

    		GUI.Box(new Rect(crosshairRect.center.x-Screen.height/8.3f, crosshairRect.center.y-Screen.height/20f+Screen.height/11f+Screen.height/40f, Screen.width/40f, Screen.width/40f) ,"", GameSystem.Instance.controllerIcons.GetStyle("Dpad"));

    	}

    }

}
