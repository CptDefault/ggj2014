using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSystem : MonoBehaviour {

	private static GameSystem _instance;

	public enum GameState {JoinGame, ShowObjective, MainGame, Paused,
	    GameOver
	};
	public GameState state;

	public GameObject playerPrefab;
	private int numPlayersJoined = 0;
	public GameObject[] players;
	public Player[] playerScripts;

	public GUISkin scoreSkin;
	public GUISkin pauseSkin;
	public GUISkin setControlsSkin;
	public GUISkin controllerIcons;

	//spawn points
	private List<GameObject> _spawnPoints;

	//objective screen
	private int _gameCountDown = 3;

	//pause screen
	public bool adjustingControls;


	//struct to hold info about who's joined the game
	struct LobbyCharacter {
		public int number;
		public bool joined;
	}

	private LobbyCharacter[] _lobby;
    public int scoreToWin = 5;
    private int winner;

    void Awake()
	{
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			_instance = this;
		}

        Screen.showCursor = false;
	}

	// Use this for initialization
	void Start () {
		_spawnPoints = new List<GameObject>();
		//get spawn points from scene
		foreach(GameObject sp in GameObject.FindGameObjectsWithTag("SpawnPoint"))
		{
			_spawnPoints.Add(sp);
		}

		_lobby = new LobbyCharacter[4];

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

		 pauseSkin.GetStyle("Text").normal.textColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {

	    
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
                        winner = player.playerNumber;
                        state = GameState.GameOver;
                        foreach (var p2 in playerScripts)
                        {
                            p2.GetComponent<PlayerInput>().enabled = false;
                            p2.GetComponent<Moveable>().enabled = false;
                            p2.rigidbody.angularVelocity = Vector3.zero;
                        }
                        break;
                    }
                }
				break;
				
            case GameState.GameOver:
		        {
		            for (int i = 0; i < 4; i++)
		            {
		                if (Input.GetButtonUp("Start_" + (i + 1)))
		                {
                            Application.LoadLevel(Application.loadedLevel);
		                }
		            }
		        }
		 break;

			case GameState.Paused:
				Paused();
				break;

		}
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

            case GameState.GameOver:
		        GameOverGUI();
		        break;
		}


	}

    public void TogglePauseGame()
	{
		if(adjustingControls || state == GameState.ShowObjective)	
			return;

		Clicker.Instance.Click();

		if(state != GameState.Paused) {
			Time.timeScale = 0;
			state = GameState.Paused;
		}
		else {
			Time.timeScale = 1;
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

    private void GameOverGUI()
    {
        GUI.skin = pauseSkin;

        float unit = Screen.width / 20;
        //background
        GUI.Box(new Rect(Screen.width / 2 - 5 * unit, Screen.height / 2 - unit * 4, unit * 10, unit * 8), "");
        GUI.Box(new Rect(Screen.width / 2 - 2.5f * unit, Screen.height / 2 - unit * 5 + unit * 2f, unit * 5, unit), "GAME OVER", pauseSkin.GetStyle("Title"));
        pauseSkin.GetStyle("Text").normal.textColor = playerScripts[winner-1].col;
        GUI.Box(new Rect(Screen.width / 2 - 1.5f * unit, Screen.height / 2 - unit * 5 + unit * 3f, unit * 3, unit),  "" + playerScripts[winner-1].name+ " WINS", pauseSkin.GetStyle("Text"));

        //scoreboard
        //top row
        GUI.Box(new Rect(Screen.width / 1.9f - 4f * unit, Screen.height / 2 - unit * 5 + unit * 4f, unit * 3, unit), "PLAYER", pauseSkin.GetStyle("Score"));
        GUI.Box(new Rect(Screen.width / 1.9f - unit, Screen.height / 2 - unit * 5 + unit * 4f, unit , unit), "KILLS", pauseSkin.GetStyle("Score"));
        GUI.Box(new Rect(Screen.width / 1.9f + unit, Screen.height / 2 - unit * 5 + unit * 4f, unit , unit), "DEATHS", pauseSkin.GetStyle("Score"));

       

        for(int i=0; i<numPlayersJoined; i++)
        {
   			pauseSkin.GetStyle("Score").normal.textColor = playerScripts[winner-1].col;

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
			}

			if(numPlayersJoined>0)
			{
				if(Input.GetButtonDown("Start_"+(i+1)))
				{
					StartCoroutine(ShowObjectiveThenStartGame());
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

		//heading
		GUI.Box(new Rect(Screen.width/2-unit*2.5f, Screen.height*0.07f, unit*5, Screen.height*0.1f), "VERSUS_JOIN GAME");

		float startingX = Screen.width/2-unit*7.5f + unit*0.75f;
		for(int i=0; i<4; i++)
		{
			LobbyCharacter thisChar = _lobby[i];
			GUI.Box(new Rect(startingX+i*unit*3.5f, Screen.height*0.19f, unit*3, Screen.height*0.6f), "");

			string joinedText;
			if(thisChar.joined)
				joinedText = "READY";
			else
				joinedText = "PRESS A TO JOIN";

			GUI.Box(new Rect(startingX+i*unit*3.5f, Screen.height*0.64f, unit*3, Screen.height*0.15f), joinedText);

		}

		string startText;
		if(numPlayersJoined>1)
			startText = "PRESS START";
		else
			startText = "WAITING FOR PLAYERS TO JOIN";

		GUI.Box(new Rect(Screen.width/2-unit*2.5f, Screen.height*0.82f, unit*5, Screen.height*0.1f), startText);

		//each player can say they're playing, increasing numPlayersJoined by 1, then set num players
	}

	IEnumerator ShowObjectiveThenStartGame()
	{
		state = GameState.ShowObjective;

		for(int i=0; i<3; i++)
		{
			Clicker.Instance.Click();
			yield return new WaitForSeconds(0.1f);
			_gameCountDown--;
		}
		

		state = GameState.MainGame;
		InitialisePlayers();
		Clicker.Instance.StartGameSound();
	}

	void ShowObjectiveGUI()
	{
		float unit = Screen.width/20;
		//background
		GUI.skin = pauseSkin;
		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2-unit*4, unit*10, unit*8), "");

		GUI.Box(new Rect(Screen.width/2-2.5f*unit, Screen.height/2-unit*4+unit*1.25f, unit*5, unit), "DEATHMATCH", pauseSkin.GetStyle("Title"));
		GUI.Box(new Rect(Screen.width/2-2.5f*unit, Screen.height/2-unit*4+unit*2.5f, unit*5, unit), "SCORE 5 KILLS TO WIN", pauseSkin.GetStyle("Text"));

		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2-unit*3+unit*3, unit*10, unit), "STARTING GAME IN", pauseSkin.GetStyle("Text"));
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

			//choose a spawn point
			//find one that hasn't been used
			int usedCount = 0;
			GameObject sp = null;
			do {
				sp = _spawnPoints[Random.Range(0,_spawnPoints.Count)];
				usedCount = sp.GetComponent<SpawnPoint>().usedCount++;
				print("used" + usedCount);
			} while(usedCount > 0);

			players[currentPlayer] = (GameObject)Instantiate(playerPrefab, sp.transform.position, Quaternion.identity);
			//players[currentPlayer].GetComponent<Player>().playerNumber = i+1;
			players[currentPlayer].GetComponent<Player>().SetPlayerNameAndColor(i+1);
			players[currentPlayer].name = "Player"+(i+1);
			playerScripts[currentPlayer] = players[currentPlayer].GetComponent<Player>();

			currentPlayer++;
		}

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
		float unit = Screen.width/20;
		switch(numPlayersJoined)
		{
			case 2:
				GUI.Box(new Rect(Screen.width/2-unit, 0, unit, 0.7f*unit), ""+playerScripts[0].score, scoreSkin.GetStyle("Player"+playerScripts[0].playerNumber));
				GUI.Box(new Rect(Screen.width/2, 0, unit, 0.7f*unit), ""+playerScripts[1].score, scoreSkin.GetStyle("Player"+playerScripts[1].playerNumber));

				//crosshairs
				

				break;

			case 3:
				GUI.Box(new Rect(Screen.width/2-unit, Screen.height/2-0.7f*unit, unit, 0.7f*unit), ""+playerScripts[0].score, scoreSkin.GetStyle("Player"+playerScripts[0].playerNumber));
				GUI.Box(new Rect(Screen.width/2, Screen.height/2-0.7f*unit, unit, 0.7f*unit), ""+playerScripts[1].score, scoreSkin.GetStyle("Player"+playerScripts[1].playerNumber));
				GUI.Box(new Rect(Screen.width/2-unit*0.5f, Screen.height/2, unit, 0.7f*unit), ""+playerScripts[2].score, scoreSkin.GetStyle("Player"+playerScripts[2].playerNumber));

				break;


			case 4:
				GUI.Box(new Rect(Screen.width/2-unit, Screen.height/2-0.7f*unit, unit, 0.7f*unit), ""+playerScripts[0].score, scoreSkin.GetStyle("Player"+playerScripts[0].playerNumber));
				GUI.Box(new Rect(Screen.width/2, Screen.height/2-0.7f*unit, unit, 0.7f*unit), ""+playerScripts[1].score, scoreSkin.GetStyle("Player"+playerScripts[1].playerNumber));
				GUI.Box(new Rect(Screen.width/2-unit, Screen.height/2, unit, 0.7f*unit), ""+playerScripts[2].score, scoreSkin.GetStyle("Player"+playerScripts[2].playerNumber));
				GUI.Box(new Rect(Screen.width/2, Screen.height/2, unit, 0.7f*unit), ""+playerScripts[3].score, scoreSkin.GetStyle("Player"+playerScripts[3].playerNumber));
				break;

		}

		//draw crosshairs
		for(int i=0;i<numPlayersJoined;i++)
		{
            if(playerScripts[i].ShotReady)
			    GUI.Box(playerScripts[i].crosshairRect, "");
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
}
