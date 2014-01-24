using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSystem : MonoBehaviour {

	private static GameSystem _instance;

	public enum GameState {JoinGame, ShowObjective, MainGame};
	public GameState state;

	public GameObject playerPrefab;
	private int numPlayersJoined = 0;
	private GameObject[] _players;

	//spawn points
	private List<GameObject> _spawnPoints;

	//objective screen
	private int _gameCountDown = 5;


	//struct to hold info about who's joined the game
	struct LobbyCharacter {
		public int number;
		public bool joined;
	}

	private LobbyCharacter[] _lobby;

	void Awake()
	{
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			_instance = this;
		}
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
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("A_1"))
		{
			print("joined");
		}

		switch(state)
		{
			case GameState.JoinGame:
				JoinGame();
				break;

			case GameState.ShowObjective:
				break;

			case GameState.MainGame:
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
				ShowObjective();
				break;

			case GameState.MainGame:
				break;
		}
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
			}

			if(numPlayersJoined>1)
			{
				if(Input.GetButtonDown("Start_"+(i+1)))
				{
					StartCoroutine(ShowObjectiveThenStartGame());
					state = GameState.ShowObjective;
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

		for(int i=0; i<5; i++)
		{
			yield return new WaitForSeconds(0.5f);
			_gameCountDown--;
		}
		

		state = GameState.MainGame;
		InitialisePlayers();
	}

	void ShowObjective()
	{
		float unit = Screen.width/20;
		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2-unit*3, unit*10, unit*6), "SCORE 5 POINTS TO WIN");

		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2-unit*3+unit*2, unit*10, unit), "STARTING GAME IN");
		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2-unit*3+unit*3, unit*10, unit), ""+_gameCountDown);
	}

	void InitialisePlayers()
	{
		
		//based on number of players set, create players
		//always have player 1
		//turn off initial camera
		GameObject.Find("Main Camera").GetComponent<Camera>().enabled = false;

		int numPlayers = numPlayersJoined;
		print(numPlayers);
		_players = new GameObject[numPlayers];

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

			_players[currentPlayer] = (GameObject)Instantiate(playerPrefab, sp.transform.position, Quaternion.identity);
			_players[currentPlayer].GetComponent<Player>().playerNumber = i+1;
			_players[currentPlayer].name = "Player"+(i+1);

			currentPlayer++;
		}

		SetUpPlayerViewports(numPlayers);
	}

	void SetUpPlayerViewports(int numPlayers)
	{
		switch(numPlayers)
		{
			case 2:
				_players[0].GetComponentInChildren<Camera>().rect = new Rect(-0.5f,0,1,1);
				_players[1].GetComponentInChildren<Camera>().rect = new Rect(0.5f,0,1,1);
				break;

			case 3:
				_players[0].GetComponentInChildren<Camera>().rect = new Rect(-0.5f,0.5f,1,1);
				_players[1].GetComponentInChildren<Camera>().rect = new Rect(0.5f,0.5f,1,1);
				_players[2].GetComponentInChildren<Camera>().rect = new Rect(0f,-0.5f,1,1);
				break;

			case 4:
				_players[0].GetComponentInChildren<Camera>().rect = new Rect(-0.5f,0.5f,1,1);
				_players[1].GetComponentInChildren<Camera>().rect = new Rect(0.5f,0.5f,1,1);
				_players[2].GetComponentInChildren<Camera>().rect = new Rect(-0.5f,-0.5f,1,1);
				_players[3].GetComponentInChildren<Camera>().rect = new Rect(0.5f,-0.5f,1,1);
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
}
