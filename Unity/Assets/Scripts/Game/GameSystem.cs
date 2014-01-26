using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


public class GameSystem : MonoBehaviour {

	private static GameSystem _instance;

	public enum GameState {JoinGame, ShowObjective, MainGame, Paused,
	    GameOver,
	    PreGameOver
	};

    public enum GameMode
    {
        Deathmatch,
        OneShot,
        Elimination,
        Juggernaut
    }

    [Serializable]
    public class GameModeInfo
    {
        public string name;
        public string description;
        public int defaultScore = 5;
    }

    public GameModeInfo[] gameModeInfos = new GameModeInfo[]
        {
            new GameModeInfo(){name = "Deathmatch"}, 
            new GameModeInfo(){name = "One Shot"}, 
            new GameModeInfo(){name = "Elimination"}, 
            new GameModeInfo(){name = "Juggernaut"}, 
        };

    public GameMode CurrentGameMode { get; private set; }

    public GameState state;

	public GameObject playerPrefab;
	private int numPlayersJoined = 0;
	public GameObject[] players;
	public Player[] playerScripts;

	public GUISkin scoreSkin;
	public GUISkin pauseSkin;
	public GUISkin setControlsSkin;
	public GUISkin controllerIcons;
	public GUISkin joinGameSkin;

	//spawn points
	private List<GameObject> _spawnPoints;

	//objective screen
	private int _gameCountDown = 3;

	//pause screen
	public bool adjustingControls;


	private Rect[] gameOverPlayerRects;


	//struct to hold info about who's joined the game
	struct LobbyCharacter {
		public int number;
		public bool joined;
		public Vector2 centerOfScreen;
		public bool inverted;
	}

	private LobbyCharacter[] _lobby;
    public int scoreToWin = 5;
    private int winner;

    public Material RedMat;
    public Material BlueMat;
    public Material GreenMat;
    public Material YellowMat;

    public Color ColorblindRed = Color.white;
    public Color ColorblindBlue = Color.white;
    public Color ColorblindGreen = Color.white;
    public Color ColorblindYellow = Color.white;

    private static Color _defaultRed;
    private static Color _defaultBlue;
    private static Color _defaultGreen;
    private static Color _defaultYellow;

    private readonly List<RespawnCamera> _pendingRespawns = new List<RespawnCamera>();
    private bool _colourblindMode;


    void Awake()
	{
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			_instance = this;
		}

        //TODO: REMOVE ME
        CurrentGameMode = GameMode.Elimination;

        _defaultBlue = BlueMat.color;
        _defaultRed = RedMat.color;
        _defaultGreen = GreenMat.color;
        _defaultYellow = YellowMat.color;

        Screen.showCursor = false;
	}

    protected void OnDestroy()
    {
        SetColorBlind(false);
    }

	// Use this for initialization
	void Start () {

		Time.timeScale = 1;

		_spawnPoints = new List<GameObject>();
		//get spawn points from scene
		foreach(GameObject sp in GameObject.FindGameObjectsWithTag("SpawnPoint"))
		{
			_spawnPoints.Add(sp);
		}

		_lobby = new LobbyCharacter[4];

		//init center of screen for lobby characters for customise controls
	    for (int i = 0; i < 4; i++)
	    {
            var rect = new Rect(i % 2 == 0 ? 0 : 0.5f,
                i >= 2 ? 0 : (4 > 2 ? 0.5f : 0),
                i == 2 && 4 == 3 ? 1 :  0.5f,
                4 > 2 ? 0.5f : 1f);
	        _lobby[i].centerOfScreen = rect.center;

	        if(!PlayerPrefs.HasKey("P"+(i+1)+"Inverted"))
	        	PlayerPrefs.SetInt("P"+(i+1)+"Inverted", 1);

	        if(!PlayerPrefs.HasKey("P"+(i+1)+"SensitivityScale"))
	        	PlayerPrefs.SetFloat("P"+(i+1)+"SensitivityScale", 1);
	    }

	    //initialise

		//state = GameState.ShowObjective;

		//scale font
		scoreSkin.GetStyle("Player1").fontSize = (int)(Screen.height/20f);
		scoreSkin.GetStyle("Player2").fontSize = (int)(Screen.height/20f);
		scoreSkin.GetStyle("Player3").fontSize = (int)(Screen.height/20f);
		scoreSkin.GetStyle("Player4").fontSize = (int)(Screen.height/20f);

		pauseSkin.GetStyle("Title").fontSize = (int)(Screen.height/15f);
		pauseSkin.GetStyle("Button").fontSize = (int)(Screen.height/25f);
		pauseSkin.GetStyle("Button").padding.left = (int)(Screen.height/15f);

		pauseSkin.GetStyle("Countdown").fontSize = (int)(Screen.height/15f);
		pauseSkin.GetStyle("Text").fontSize = (int)(Screen.height/25f);
		pauseSkin.GetStyle("Score").fontSize = (int)(Screen.height/25f);

		setControlsSkin.GetStyle("Title").fontSize = (int)(Screen.height/25f);
		setControlsSkin.GetStyle("Button").fontSize = (int)(Screen.height/40f);
		setControlsSkin.GetStyle("Button").padding.left = (int)(Screen.height/18f);

		joinGameSkin.GetStyle("Box").padding.left = (int)(Screen.height/15f);
		joinGameSkin.GetStyle("JoinText").fontSize = (int)(Screen.height/15f);

		pauseSkin.GetStyle("Text").normal.textColor = Color.white;
	}

    private void IncrementGameMode(bool up)
    {
        if (!up)
        {
            if (CurrentGameMode == 0)
                CurrentGameMode = (GameMode) Enum.GetValues(typeof (GameMode)).Length - 1;
            else
                CurrentGameMode--;
        }
        else
        {
            if (CurrentGameMode == (GameMode)Enum.GetValues(typeof(GameMode)).Length - 1)
                CurrentGameMode = 0;
            else
                CurrentGameMode++;
        }
    }


	// Update is called once per frame
	void Update () {

        //colourblind support
        if (Input.GetKeyDown(KeyCode.C))
        {
            _colourblindMode = !_colourblindMode;

            SetColorBlind(_colourblindMode);
        }

	    switch(state)
		{
			case GameState.JoinGame:
				JoinGame();
				break;

			case GameState.ShowObjective:
				break;

			case GameState.MainGame:
                foreach (var player in playerScripts)
                {
                    if (player.score >= scoreToWin)
                    {
                        winner = System.Array.IndexOf(playerScripts,player);
                        //print("Winner: " + winner);
                        StartCoroutine(ActivateGameOver());

                        AudioManager.Instance.inMenu = true;

                        state = GameState.PreGameOver;

                        break;
                    }
                }

                RunGameModes();

				break;

            case GameState.PreGameOver:
                break;
				
            case GameState.GameOver:
            	for(int i=0; i<4; i++)
            	{
		            if(Input.GetButtonUp("Start_"+(i+1)))
		            {
		            	//restart
                        ResetLevel();
		            	//Application.LoadLevel(Application.loadedLevel);
		            	Clicker.Instance.Click();
		            }
		        }
		 		break;

			case GameState.Paused:
				Paused();
				break;

		}
	}

    private void SetColorBlind(bool colourblindMode)
    {
        BlueMat.color = colourblindMode ? ColorblindBlue : _defaultBlue;
        RedMat.color = colourblindMode ? ColorblindRed : _defaultRed;
        GreenMat.color = colourblindMode ? ColorblindGreen : _defaultGreen;
        YellowMat.color = colourblindMode ? ColorblindYellow : _defaultYellow;
    }

    private void RunGameModes()
    {
        switch (CurrentGameMode)
        {
            case GameMode.Deathmatch:
                break;
            case GameMode.OneShot:
                bool allNeedReload = true;
                foreach (var player in playerScripts)
                {
                    if (player.ShotReady)
                        allNeedReload = false;
                }
                if (allNeedReload)
                {
                    foreach (var player in playerScripts)
                    {
                        player.GetComponent<Weapon>().StartReload();
                    }
                }
                break;
            case GameMode.Elimination:
                if (_pendingRespawns.Count >= players.Count() - 1)
                {
                    Player winner = null;
                    foreach (var player in playerScripts)
                    {
                        if (player.gameObject.activeSelf)
                        {
                            winner = player;
                            player.ScoreUp();
                        }
                    }
                    foreach (var pendingRespawn in _pendingRespawns)
                    {
                        pendingRespawn.Respawn(3);
                    }
                    StartCoroutine(EliminationRestart(winner, 3));
                    _pendingRespawns.Clear();
                }
                break;
            case GameMode.Juggernaut:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private IEnumerator EliminationRestart(Player player, int delay)
    {
        _gameCountDown = delay;
        for (int i = 0; i < delay; i++)
        {
            Clicker.Instance.Click();
            yield return new WaitForSeconds(1f);
            _gameCountDown--;
        }
        player.Respawn();

    }

    void OnGUI() 
	{
		switch(state)
		{
			case GameState.JoinGame:
				JoinGameGUI();
				break;

			case GameState.ShowObjective:
				ShowObjectiveGUI();
				break;

			case GameState.MainGame:
				MainGameGUI();
				break;

			case GameState.Paused:
				PausedGUI();
				break;

            case GameState.PreGameOver:
                float unit = Screen.width / 15f;
		        DrawBlackBars();
		        GUI.Box(new Rect(Screen.width / 2f - 2 * unit, Screen.height / 2f - 0.35f * unit, unit * 4, unit * 0.7f), "GAME OVER", pauseSkin.GetStyle("Title"));
		        break;

            case GameState.GameOver:
		        GameOverGUI();
		        break;
		}


	}

    public GameObject GetSafestSpawnpoint()
    {
        if(players == null || !players.Any())
            return _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        var activePlayers = players.Where(p => p != null && p.activeSelf).ToList();
        if (activePlayers.Count == 0)
            return _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        return _spawnPoints.OrderBy(
            spawn => activePlayers.
                             Min(p => (p.transform.position - spawn.transform.position).sqrMagnitude))
                    .Last();
    }

    public void TogglePauseGame()
	{
		if(adjustingControls || state == GameState.ShowObjective)	
			return;

		Clicker.Instance.Click();

		if(state != GameState.Paused) {
			Time.timeScale = 0;
            AudioManager.Instance.inMenu = true;
			state = GameState.Paused;
		}
		else {
			Time.timeScale = 1;
            AudioManager.Instance.inMenu = false;
			state = GameState.MainGame;
		}
	}

	void Paused()
	{
		if(adjustingControls)
		{
			for(int i=0; i<4; i++)
			{
				if(Input.GetButtonUp("X_"+(i+1)))
				{
					adjustingControls = false;
					foreach(Player p in playerScripts)
						p.setControls = false;

					state = GameState.Paused;

					Clicker.Instance.Click();
					PlayerPrefs.Save();
				}
			}

			return;
		}


		for(int i=0; i<4; i++)
		{
			if(Input.GetButtonUp("X_"+(i+1)))
			{
				adjustingControls = true;

				foreach(Player p in playerScripts)
					p.setControls = true;
				Clicker.Instance.Click();
			}
			else if(Input.GetButtonUp("A_"+(i+1)))
			{
				TogglePauseGame();
				Clicker.Instance.Click();
			}
			else if(Input.GetButtonUp("Back_"+(i+1)))
			{
				//quit
                //ResetLevel();
				Application.LoadLevel(1);
				Clicker.Instance.Click();
			}
		}
	}

    void PausedGUI()
	{
		GUI.skin = pauseSkin;

		float unit = Screen.width/20;

		if(adjustingControls)
		{
			//don't show rest of pause when adjusting controls
			if(GUI.Button(new Rect(Screen.width/2-1.5f*unit, Screen.height/2-(0.75f/2*unit), unit*3, 0.75f*unit), "DONE"))
			{
				adjustingControls = false;
				foreach(Player p in playerScripts)
					p.setControls = false;
			}

			GUI.Box(new Rect(Screen.width/2-1.4f*unit, Screen.height/2-(0.4f/2*unit), unit*0.5f, 0.5f*unit), "",controllerIcons.GetStyle("x"));

			//controls for each player

			return;
		}

		
		//background
		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2-unit*3, unit*10, unit*6), "");

		GUI.Box(new Rect(Screen.width/2-2.5f*unit, Screen.height/2-unit*4+unit*2f, unit*5, unit), "PAUSED", pauseSkin.GetStyle("Title"));

		//buttons
		if(GUI.Button(new Rect(Screen.width/2-1.5f*unit, Screen.height/2-unit*4+unit*3.2f, unit*3, 0.75f*unit), "RESUME"))
		{
			adjustingControls = true;

			foreach(Player p in playerScripts)
				p.setControls = true;
		}


		if(GUI.Button(new Rect(Screen.width/2-1.5f*unit, Screen.height/2-unit*4+unit*4.2f, unit*3, 0.75f*unit), "CONTROLS"))
		{
			adjustingControls = true;

			foreach(Player p in playerScripts)
				p.setControls = true;
		}

		if(GUI.Button(new Rect(Screen.width/2-1.5f*unit, Screen.height/2-unit*4+unit*5.2f, unit*3, 0.75f*unit), "QUIT"))
		{
			
		}

		//icons
		GUI.Box(new Rect(Screen.width/2-1.37f*unit, Screen.height/2-unit*4+unit*3.35f, 0.5f*unit, 0.5f*unit),"", controllerIcons.GetStyle("A"));
		GUI.Box(new Rect(Screen.width/2-1.37f*unit, Screen.height/2-unit*4+unit*4.35f, 0.5f*unit, 0.5f*unit),"", controllerIcons.GetStyle("x"));
		GUI.Box(new Rect(Screen.width/2-1.37f*unit, Screen.height/2-unit*4+unit*5.35f, 0.5f*unit, 0.5f*unit),"", controllerIcons.GetStyle("Back"));

	}

    public void ResetLevel()
    {
        Time.timeScale = 1;

        GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;

        _gameCountDown = 3;
        winner = 0;
        
        foreach (var player in players)
        {
            Destroy(player);
        }

        StartCoroutine(ShowObjectiveThenStartGame());
    }


	IEnumerator ActivateGameOver()
	{
		foreach (var p2 in playerScripts)
		{
		    p2.GetComponent<PlayerInput>().enabled = false;
		    p2.rigidbody.velocity = Vector3.zero;
		    p2.rigidbody.angularVelocity = Vector3.zero;
		}

		for(float t = 0; t < 2.5f; t += Time.deltaTime / (1 - ( t / 3)))
		{
			Time.timeScale = 1 - ( t / 3);
			yield return null;
		}
		state = GameState.GameOver;
		Time.timeScale = 1;
	}

    private void GameOverGUI()
    {
        GUI.skin = pauseSkin;

   		//color overlays
   		for(int i=0; i<numPlayersJoined; i++)
   		{
   			GUI.Box(gameOverPlayerRects[i], "", joinGameSkin.GetStyle(playerScripts[i].name));
   		}

   		switch(numPlayersJoined)
   		{
   			case 2:
   				//draw black bars
   				GUI.Box(new Rect(Screen.width/2-3, 0, 6, Screen.height), "", scoreSkin.GetStyle("Box"));
   				break;

   			case 3:

   				GUI.Box(new Rect(Screen.width/2-3, 0, 6, Screen.height/2), "", scoreSkin.GetStyle("Box"));
   				GUI.Box(new Rect(0, Screen.height/2-3, Screen.width, 6), "", scoreSkin.GetStyle("Box"));
   				break;


   			case 4:
   				GUI.Box(new Rect(0, Screen.height/2-3, Screen.width, 6), "", scoreSkin.GetStyle("Box"));
   				GUI.Box(new Rect(Screen.width/2-3, 0, 6, Screen.height), "", scoreSkin.GetStyle("Box"));
   				break;

   		}

        float unit = Screen.width / 20;
        //background
        GUI.Box(new Rect(Screen.width / 2 - 5 * unit, Screen.height / 2 - unit * 4, unit * 10, unit * 8), "");
        GUI.Box(new Rect(Screen.width / 2 - 2.5f * unit, Screen.height / 2 - unit * 5 + unit * 2f, unit * 5, unit), "GAME OVER", pauseSkin.GetStyle("Title"));
        pauseSkin.GetStyle("Text").normal.textColor = playerScripts[winner].col;
        GUI.Box(new Rect(Screen.width / 2 - 1.5f * unit, Screen.height / 2 - unit * 5 + unit * 3f, unit * 3, unit),  "" + playerScripts[winner].name+ " WINS", pauseSkin.GetStyle("Text"));

        //scoreboard
        //top row
        GUI.Box(new Rect(Screen.width / 1.9f - 4f * unit, Screen.height / 2 - unit * 5 + unit * 4f, unit * 3, unit), "PLAYER", pauseSkin.GetStyle("Score"));
        GUI.Box(new Rect(Screen.width / 1.9f - unit, Screen.height / 2 - unit * 5 + unit * 4f, unit , unit), "KILLS", pauseSkin.GetStyle("Score"));
        GUI.Box(new Rect(Screen.width / 1.9f + unit, Screen.height / 2 - unit * 5 + unit * 4f, unit , unit), "DEATHS", pauseSkin.GetStyle("Score"));

       

        for(int i=0; i<numPlayersJoined; i++)
        {
   			pauseSkin.GetStyle("Score").normal.textColor = playerScripts[winner].col;

        	pauseSkin.GetStyle("Score").normal.textColor = playerScripts[i].col;
        	GUI.Box(new Rect(Screen.width / 1.9f - 4f * unit, (Screen.height / 2 - unit * 5 + unit * 4.8f+i*0.5f*unit), unit * 3f, 0.5f*unit), playerScripts[i].name, pauseSkin.GetStyle("Score"));
        	pauseSkin.GetStyle("Score").normal.textColor = Color.white;
        	GUI.Box(new Rect(Screen.width / 1.9f - unit, (Screen.height / 2 - unit * 5 + unit * 4.8f+i*0.5f*unit), unit * 1f, 0.5f*unit), "" + playerScripts[i].score, pauseSkin.GetStyle("Score"));
        	GUI.Box(new Rect(Screen.width / 1.9f + 1f * unit, (Screen.height / 2 - unit * 5 + unit * 4.8f+i*0.5f*unit), unit * 1f, 0.5f*unit), ""+playerScripts[i].deaths, pauseSkin.GetStyle("Score"));
        }

        if (GUI.Button(new Rect(Screen.width / 2 - 1.5f * unit, Screen.height / 2 - unit * 5 + unit * 6.9f, unit * 3, 0.75f * unit), "RESTART"))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        //icon
        GUI.Box(new Rect(Screen.width / 2 - 1.3f * unit, Screen.height / 2 - unit * 5 + unit * 7.05f, unit * 0.5f, 0.5f * unit),"", controllerIcons.GetStyle("Start"));
    }

    void JoinGame()
	{
		for(int i=0; i<4; i++)
		{
			LobbyCharacter thisChar = _lobby[i];
			if(!thisChar.joined && Input.GetButtonDown("A_"+(i+1)))
			{
				_lobby[i].joined = true;
				numPlayersJoined++;
				Clicker.Instance.Click();
				print("joined");
			}


			if(Input.GetButtonDown("Y_"+(i+1)))
			{
				_lobby[i].inverted = !_lobby[i].inverted;
				//PlayerPrefs.SetInt("P"+(i+1)+"Inverted", -1);
				print("Invert controsl for " + (i+1));
			}

			//adjust controls
			/*if(_lobby[i].adjustingControls)
			{
				//print("I am adjusing "+ i);
				//invert

				float sensitivityScale = PlayerPrefs.GetFloat("P"+(i+1)+"SensitivityScale");
				if(Input.GetAxis("DPad_XAxis_"+(i+1)) > 0 || Input.GetAxis("DPad_YAxis_"+(i+1)) > 0) {
					sensitivityScale += 0.01f;
					PlayerPrefs.SetFloat("P"+(i+1)+"SensitivityScale", sensitivityScale);
				}
				else if(Input.GetAxis("DPad_XAxis_"+(i+1)) < 0 || Input.GetAxis("DPad_YAxis_"+(i+1)) < 0){
					sensitivityScale -= 0.01f;
					PlayerPrefs.SetFloat("P"+(i+1)+"SensitivityScale", sensitivityScale);
				}
			}*/
			

			if(numPlayersJoined>0)
			{ 
				if(Input.GetButtonDown("Start_"+(i+1)))
				{
					StartCoroutine(ShowObjectiveThenStartGame());
					PlayerPrefs.Save();
					state = GameState.ShowObjective;
					Clicker.Instance.Click();
				}
			}
		}


	}


	void JoinGameGUI()
	{
		//select number of players
		float unit = Screen.width/20;
		//float vUnit = Screen.height/10;

		//background
		GUI.Box(new Rect(Screen.width/2-unit*7.5f, Screen.height*0.05f, unit*15, Screen.height*0.9f), "");

		GUI.Box(new Rect(0,0, Screen.width/2, Screen.height/2), "", joinGameSkin.GetStyle("Red"));
		GUI.Box(new Rect(Screen.width/2,0, Screen.width/2, Screen.height/2), "", joinGameSkin.GetStyle("Blue"));
		GUI.Box(new Rect(0,Screen.height/2, Screen.width/2, Screen.height/2), "", joinGameSkin.GetStyle("Green"));
		GUI.Box(new Rect(Screen.width/2,Screen.height/2, Screen.width/2, Screen.height/2), "", joinGameSkin.GetStyle("Yellow"));

		float startingX = Screen.width/4;
		float startingY = Screen.height/6;
		joinGameSkin.GetStyle("JoinText").normal.textColor = Color.black;
		for(int i=0; i<4; i++)
		{
			LobbyCharacter thisChar = _lobby[i];

			string playerName = "";
			joinGameSkin.GetStyle("JoinText").normal.textColor = Color.black;

			string invertedText = "";
			if(!thisChar.inverted) {
				invertedText = "Y LOOK NORMAL";
			}
			else {
				invertedText = "Y LOOK INVERTED";
			}

			switch(i)
			{
				case 0:
					//red
					playerName = "RED\n";

					GUI.Box(new Rect(startingX-2.5f*unit, startingY-unit, unit*5, unit*2f), playerName, joinGameSkin.GetStyle("JoinText"));

					GUI.Box(new Rect(startingX-2.5f*unit+unit*0.5f, startingY-unit+unit*0.9f, unit*5, unit*2f), invertedText, joinGameSkin.GetStyle("JoinText"));

					GUI.Box(new Rect(startingX-3.1f*unit, startingY-unit+unit*1.35f, unit*1, unit*1f), "", controllerIcons.GetStyle("Y"));

					break;

				case 1:
					playerName = "BLUE\n";
					GUI.Box(new Rect(startingX+Screen.width/2-2.5f*unit, startingY-unit, unit*5, unit*2f), playerName, joinGameSkin.GetStyle("JoinText"));

					GUI.Box(new Rect(startingX+Screen.width/2-2.5f*unit+unit*0.5f, startingY-unit+unit*0.9f, unit*5, unit*2f), invertedText, joinGameSkin.GetStyle("JoinText"));

					GUI.Box(new Rect(startingX+Screen.width/2-3.1f*unit, startingY-unit+unit*1.35f, unit*1, unit*1f), "", controllerIcons.GetStyle("Y"));

					break;

				case 2:
					playerName = "GREEN\n";
					GUI.Box(new Rect(startingX-2.5f*unit, startingY-unit+Screen.height/2, unit*5, unit*2f), playerName, joinGameSkin.GetStyle("JoinText"));

					GUI.Box(new Rect(startingX-2.5f*unit+unit*0.5f, startingY+Screen.height/2-unit+unit*0.9f, unit*5, unit*2f), invertedText, joinGameSkin.GetStyle("JoinText"));

					GUI.Box(new Rect(startingX-3.1f*unit, startingY+Screen.height/2-unit+unit*1.35f, unit*1, unit*1f), "", controllerIcons.GetStyle("Y"));

					break;

				case 3:
					playerName = "YELLOW\n";
					GUI.Box(new Rect(startingX+Screen.width/2-2.5f*unit, startingY-unit+Screen.height/2, unit*5, unit*2f), playerName, joinGameSkin.GetStyle("JoinText"));

					GUI.Box(new Rect(startingX+Screen.width/2-2.5f*unit+unit*0.5f, startingY+Screen.height/2-unit+unit*0.9f, unit*5, unit*2f), invertedText, joinGameSkin.GetStyle("JoinText"));

					GUI.Box(new Rect(startingX+Screen.width/2-3.1f*unit, startingY-unit+Screen.height/2+unit*1.35f, unit*1, unit*1f), "", controllerIcons.GetStyle("Y"));

					break;
			}

			string joinedText = "";
			

			if(thisChar.joined) {
				joinedText += "READY";
				joinGameSkin.GetStyle("JoinText").normal.textColor = Color.white;
			}
			else {
				joinedText += "PRESS \t\t\t\t\t TO JOIN";
				joinGameSkin.GetStyle("JoinText").normal.textColor = Color.black;
			}


			switch(i)
			{
				case 0:
					//red
					playerName = "RED\n";

					GUI.Box(new Rect(startingX-2.5f*unit, startingY-unit+unit+unit, unit*5, unit*2f), joinedText, joinGameSkin.GetStyle("JoinText"));


					if(!thisChar.joined) 
						GUI.Box(new Rect(startingX-2.5f*unit+unit*1.9f, startingY-unit+unit*1.5f+unit, unit*1, unit*1f), "", controllerIcons.GetStyle("A"));

					break;

				case 1:
					playerName = "BLUE\n";
					GUI.Box(new Rect(startingX+Screen.width/2-2.5f*unit, startingY-unit+unit+unit, unit*5, unit*2f), joinedText, joinGameSkin.GetStyle("JoinText"));


					if(!thisChar.joined) 
						GUI.Box(new Rect(startingX+Screen.width/2-2.5f*unit+unit*1.9f, startingY-unit+unit*1.5f+unit, unit*1, unit*1f), "", controllerIcons.GetStyle("A"));

					break;

				case 2:
					playerName = "GREEN\n";
					GUI.Box(new Rect(startingX-2.5f*unit, startingY-unit+Screen.height/2+unit+unit, unit*5, unit*2f), joinedText, joinGameSkin.GetStyle("JoinText"));


					if(!thisChar.joined) 
						GUI.Box(new Rect(startingX-2.5f*unit+unit*1.9f, startingY-unit+Screen.height/2+unit*1.5f+unit, unit*1, unit*1f), "", controllerIcons.GetStyle("A"));

					break;

				case 3:
					playerName = "YELLOW\n";
					GUI.Box(new Rect(startingX+Screen.width/2-2.5f*unit, startingY-unit+Screen.height/2+unit+unit, unit*5, unit*2f), joinedText, joinGameSkin.GetStyle("JoinText"));


					if(!thisChar.joined) 
						GUI.Box(new Rect(startingX+Screen.width/2-2.5f*unit+unit*1.9f, startingY-unit+Screen.height/2+unit*1.5f+unit, unit*1, unit*1f), "", controllerIcons.GetStyle("A"));

					break;
			}
		}

		GUI.Box(new Rect(0, Screen.height/2-3, Screen.width, 6), "", scoreSkin.GetStyle("Box"));
		GUI.Box(new Rect(Screen.width/2-3, 0, 6, Screen.height), "", scoreSkin.GetStyle("Box"));

		string startText;
		if(numPlayersJoined>1)
			startText = "PRESS START";
		else
			startText = "WAIT FOR PLAYERS";

		GUI.Box(new Rect(Screen.width/2-unit*3f, Screen.height/2-0.05f*Screen.height, unit*6, Screen.height*0.1f), startText, pauseSkin.GetStyle("Title"));

		//each player can say they're playing, increasing numPlayersJoined by 1, then set num players

		//JoinGameControlsGUI();
	}

	//customise controls on join game
	/*void JoinGameControlsGUI()
	{	
		for(int i=0; i<4; i++)
		{
			//check if this lobby char is trying to customise
			if(_lobby[i].adjustingControls == true)
			{
				//draw GUI for me

				GUI.Box(new Rect(_lobby[i].centerOfScreen.x*Screen.width-Screen.height/5, (1-_lobby[i].centerOfScreen.y)*Screen.height-Screen.height/6, Screen.height/2.5F, Screen.height/3), "", GameSystem.Instance.setControlsSkin.GetStyle("Box"));

				GUI.Box(new Rect(_lobby[i].centerOfScreen.x*Screen.width-Screen.height/8, (1-_lobby[i].centerOfScreen.y)*Screen.height-Screen.height/7f, Screen.height/4f, Screen.height/13), "SET CONTROLS",GameSystem.Instance.setControlsSkin.GetStyle("Title"));

				string invertedText;

				//print(""+ i+ " inverted" + PlayerPrefs.GetInt("P"+(i+1)+"Inverted"));
				if(PlayerPrefs.GetInt("P"+(i+1)+"Inverted") == 1) {
					invertedText = "Y Look Normal";
				}
				else {
					invertedText = "Y Look Inverted";
				}

				GUI.Box(new Rect(_lobby[i].centerOfScreen.x*Screen.width-Screen.height/8, (1-_lobby[i].centerOfScreen.y)*Screen.height-Screen.height/20f, Screen.height/4f, Screen.height/13), invertedText, GameSystem.Instance.setControlsSkin.GetStyle("Button"));

				GUI.Box(new Rect(_lobby[i].centerOfScreen.x*Screen.width-Screen.height/8, (1-_lobby[i].centerOfScreen.y)*Screen.height-Screen.height/20f+Screen.height/11f, Screen.height/4f, Screen.height/13), "Look Sensitivity", GameSystem.Instance.setControlsSkin.GetStyle("Button"));

				GUI.skin = GameSystem.Instance.setControlsSkin;
				float sensitivityScale = PlayerPrefs.GetFloat("P"+(i+1)+"SensitivityScale");
				sensitivityScale = GUI.HorizontalSlider(new Rect(_lobby[i].centerOfScreen.x*Screen.width-Screen.height/8, (1-_lobby[i].centerOfScreen.y)*Screen.height-Screen.height/20f+Screen.height/6f, Screen.height/4f, Screen.height/13), sensitivityScale, 0.5f, 2.0f);

				//icons
				GUI.Box(new Rect(_lobby[i].centerOfScreen.x*Screen.width-Screen.height/8.3f, (1-_lobby[i].centerOfScreen.y)*Screen.height-Screen.height/20f+Screen.height/40f, Screen.width/40f, Screen.width/40f) ,"", GameSystem.Instance.controllerIcons.GetStyle("Y"));

				GUI.Box(new Rect(_lobby[i].centerOfScreen.x*Screen.width-Screen.height/8.3f, (1-_lobby[i].centerOfScreen.y)*Screen.height-Screen.height/20f+Screen.height/11f+Screen.height/40f, Screen.width/40f, Screen.width/40f) ,"", GameSystem.Instance.controllerIcons.GetStyle("Dpad"));
			}
		}
	}*/

	IEnumerator ShowObjectiveThenStartGame()
	{
		state = GameState.ShowObjective;

		for(int i=0; i<3; i++)
		{
			Clicker.Instance.Click();
			yield return new WaitForSeconds(1f);
			_gameCountDown--;
		}
		

		state = GameState.MainGame;
        AudioManager.Instance.inMenu = false;

		InitialisePlayers();
		Clicker.Instance.StartGameSound();
	}

	void ShowObjectiveGUI(bool roundStart = false)
	{
		float unit = Screen.width/20;
		//background
	    pauseSkin.GetStyle("Text").normal.textColor = Color.white;
		GUI.skin = pauseSkin;
		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2-unit*4, unit*10, unit*8), "");

		GUI.Box(new Rect(Screen.width/2-2.5f*unit, Screen.height/2-unit*4+unit*1.25f, unit*5, unit), 
            gameModeInfos[(int)CurrentGameMode].name, pauseSkin.GetStyle("Title"));
		GUI.Box(new Rect(Screen.width/2-2.5f*unit, Screen.height/2-unit*4+unit*2.5f, unit*5, unit), 
            string.Format(gameModeInfos[(int)CurrentGameMode].description, scoreToWin), pauseSkin.GetStyle("Text"));

		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2-unit*3+unit*3, unit*10, unit),
            (roundStart ? "NEXT ROUND" : "STARTING GAME") + " IN", pauseSkin.GetStyle("Text"));
		GUI.Box(new Rect(Screen.width/2-0.5f*unit, Screen.height/2-unit*3+unit*4, unit, unit), ""+_gameCountDown, pauseSkin.GetStyle("Countdown"));
	}

	void InitialisePlayers()
	{
		
		//based on number of players set, create players
		//always have player 1
		//turn off initial camera
		GameObject.Find("Main Camera").GetComponent<Camera>().enabled = false;

		int numPlayers = numPlayersJoined;
		print(numPlayers);
		players = new GameObject[numPlayers];
		playerScripts = new Player[numPlayers];

		int currentPlayer = 0;
		for(int i=0; i<4; i++)
		{
			if(!_lobby[i].joined)
				continue;

			//set inverted
			if(_lobby[i].inverted)
				PlayerPrefs.SetInt("P"+(i+1)+"Inverted", -1);
			else
				PlayerPrefs.SetInt("P"+(i+1)+"Inverted", 1);

			//choose a spawn point
			//find one that hasn't been used
			int usedCount = 0;
			GameObject sp = null;
			/*do {
				sp = _spawnPoints[Random.Range(0,_spawnPoints.Count)];
				usedCount = sp.GetComponent<SpawnPoint>().usedCount++;
				print("used" + usedCount);
			} while(usedCount > 0);*/

		    sp = GetSafestSpawnpoint();

			players[currentPlayer] = (GameObject)Instantiate(playerPrefab, sp.transform.position, sp.transform.rotation);
			//players[currentPlayer].GetComponent<Player>().playerNumber = i+1;
			players[currentPlayer].GetComponent<Player>().SetPlayerNameAndColor(i+1);
			players[currentPlayer].name = "Player"+(i+1);
			playerScripts[currentPlayer] = players[currentPlayer].GetComponent<Player>();

			currentPlayer++;
		}

		//special case
		if(numPlayers == 3)
			playerScripts[2].scoredMessageLow = true;

		SetUpPlayerViewports(numPlayers);
	}

	void SetUpPlayerViewports(int numPlayers)
	{
	    for (int i = 0; i < numPlayers; i++)
	    {
            var rect = new Rect(i % 2 == 0 ? 0 : 0.5f,
                i >= 2 ? 0 : (numPlayers > 2 ? 0.5f : 0),
                i == 2 && numPlayers == 3 ? 1 :  0.5f,
                numPlayers > 2 ? 0.5f : 1f);
            SetChildCamera(players[i], rect);

	        playerScripts[i].crosshairPos = rect.center;
	    }

	    //set up rects for game over
	    gameOverPlayerRects = new Rect[numPlayers];
	    switch(numPlayers)
	    {
	    	case 2:
	    		gameOverPlayerRects[0] = new Rect(0,0, Screen.width/2, Screen.height);
	    		gameOverPlayerRects[1] = new Rect(Screen.width/2,0, Screen.width/2, Screen.height);
	    		break;

	    	case 3: 
    			gameOverPlayerRects[0] = new Rect(0,0, Screen.width/2, Screen.height);
    			gameOverPlayerRects[1] = new Rect(Screen.width/2,0, Screen.width/2, Screen.height);
    			gameOverPlayerRects[2] = new Rect(0, Screen.height/2, Screen.width, Screen.height/2);
    			break;

	    	case 4: 
    			gameOverPlayerRects[0] = new Rect(0,0, Screen.width/2, Screen.height);
    			gameOverPlayerRects[1] = new Rect(Screen.width/2,0, Screen.width/2, Screen.height);
    			gameOverPlayerRects[2] = new Rect(0, Screen.height/2, Screen.width/2, Screen.height/2);
    			gameOverPlayerRects[3] = new Rect(Screen.width/2, Screen.height/2, Screen.width/2, Screen.height/2);
    			break;

	    }
	}

    private void SetChildCamera(GameObject player, Rect rect)
    {
        var cameras = player.GetComponentsInChildren<Camera>();
        foreach (var cam in cameras)
        {
            cam.rect = rect;            
        }
    }

    void MainGameGUI()
	{
		float unit = Screen.width/15;

        DrawBlackBars();

		//draw crosshairs
		for(int i=0;i<numPlayersJoined;i++)
		{
            if(playerScripts[i].ShotReady)
			    GUI.Box(playerScripts[i].crosshairRect, "");
		}

        bool drawScores = GameModeGUI();

        if (drawScores)
        {
            switch (numPlayersJoined)
            {
                case 2:
                    GUI.Box(new Rect(Screen.width/2 - unit, 0, unit, 0.7f*unit), "" + playerScripts[0].score,
                            scoreSkin.GetStyle("Player" + playerScripts[0].playerNumber));
                    GUI.Box(new Rect(Screen.width/2, 0, unit, 0.7f*unit), "" + playerScripts[1].score,
                            scoreSkin.GetStyle("Player" + playerScripts[1].playerNumber));
                    break;

                case 3:
                    GUI.Box(new Rect(Screen.width/2 - unit, Screen.height/2 - 0.7f*unit, unit, 0.7f*unit),
                            "" + playerScripts[0].score, scoreSkin.GetStyle("Player" + playerScripts[0].playerNumber));
                    GUI.Box(new Rect(Screen.width/2, Screen.height/2 - 0.7f*unit, unit, 0.7f*unit),
                            "" + playerScripts[1].score, scoreSkin.GetStyle("Player" + playerScripts[1].playerNumber));
                    GUI.Box(new Rect(Screen.width/2 - unit*0.5f, Screen.height/2, unit, 0.7f*unit),
                            "" + playerScripts[2].score, scoreSkin.GetStyle("Player" + playerScripts[2].playerNumber));
                    break;


                case 4:
                    GUI.Box(new Rect(Screen.width/2 - unit, Screen.height/2 - 0.7f*unit, unit, 0.7f*unit),
                            "" + playerScripts[0].score, scoreSkin.GetStyle("Player" + playerScripts[0].playerNumber));
                    GUI.Box(new Rect(Screen.width/2, Screen.height/2 - 0.7f*unit, unit, 0.7f*unit),
                            "" + playerScripts[1].score, scoreSkin.GetStyle("Player" + playerScripts[1].playerNumber));
                    GUI.Box(new Rect(Screen.width/2 - unit, Screen.height/2, unit, 0.7f*unit),
                            "" + playerScripts[2].score, scoreSkin.GetStyle("Player" + playerScripts[2].playerNumber));
                    GUI.Box(new Rect(Screen.width/2, Screen.height/2, unit, 0.7f*unit), "" + playerScripts[3].score,
                            scoreSkin.GetStyle("Player" + playerScripts[3].playerNumber));
                    break;

            }
        }
	}

    private bool GameModeGUI()
    {
        switch (CurrentGameMode)
        {
            case GameMode.Deathmatch:
                break;
            case GameMode.OneShot:
                break;
            case GameMode.Elimination:
                if (_gameCountDown > 0)
                {
                    ShowObjectiveGUI(true);
                    return false;
                }
                break;
            case GameMode.Juggernaut:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        //draw the scores
        return true;
    }

    void DrawBlackBars()
    {
        float unit = Screen.width / 15;
        switch (numPlayersJoined)
        {
            case 2:
                //draw black bars
                GUI.Box(new Rect(Screen.width / 2 - 3, 0, 6, Screen.height), "", scoreSkin.GetStyle("Box"));

                break;

            case 3:

                GUI.Box(new Rect(Screen.width / 2 - 3, 0, 6, Screen.height / 2), "", scoreSkin.GetStyle("Box"));
                GUI.Box(new Rect(0, Screen.height / 2 - 3, Screen.width, 6), "", scoreSkin.GetStyle("Box"));


                break;


            case 4:
                GUI.Box(new Rect(0, Screen.height / 2 - 3, Screen.width, 6), "", scoreSkin.GetStyle("Box"));
                GUI.Box(new Rect(Screen.width / 2 - 3, 0, 6, Screen.height), "", scoreSkin.GetStyle("Box"));

                break;

        }
    }


	public static GameSystem Instance
	{
		get
		{
			return _instance;
		}
	}

	public void OnApplicationQuit()
	{
		_instance = null;
	}

    public void AddPendingRespawn(RespawnCamera respawnCamera)
    {
        _pendingRespawns.Add(respawnCamera);
    }
}
