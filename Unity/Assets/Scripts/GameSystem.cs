using UnityEngine;
using System.Collections;

public class GameSystem : MonoBehaviour {


	public enum GameState {CharacterSelect, ShowObjective, MainGame};
	public GameState state;

	public GameObject playerPrefab;

	private int numPlayersJoined = 0;

	private GameObject[] _players;


	// Use this for initialization
	void Start () {



	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() 
	{
		switch(state)
		{
			case GameState.CharacterSelect:
				CharacterSelect();
				break;

			case GameState.ShowObjective:
				ShowObjective();
				break;

			case GameState.MainGame:
				break;
		}
	}

	void CharacterSelect()
	{
		//select number of players
		float unit = Screen.width/20;

		GUI.Box(new Rect(Screen.width/2-unit*5, Screen.height/2-unit*5, unit*10, unit *10), "WHO'S PLAYING?");

		//each player can say they're playing, increasing numPlayersJoined by 1, then set num players

		if(GUI.Button(new Rect(Screen.width/2-unit*4, Screen.height/2-unit*5+unit*1, 2*unit, unit), "2"))
		{
			PlayerPrefs.SetInt("NumPlayers", 2);
		}

		if(GUI.Button(new Rect(Screen.width/2-unit*2, Screen.height/2-unit*5+unit*1, 2*unit, unit), "3"))
		{
			PlayerPrefs.SetInt("NumPlayers", 3);
		}

		if(GUI.Button(new Rect(Screen.width/2, Screen.height/2-unit*5+unit*1, 2*unit, unit), "4"))
		{
			PlayerPrefs.SetInt("NumPlayers", 4);
		}

		if(GUI.Button(new Rect(Screen.width/2-unit*3, Screen.height/2, 6*unit, 2*unit), "START GAME"))
		{
			StartCoroutine(ShowObjectiveThenStartGame());
		}
	}

	IEnumerator ShowObjectiveThenStartGame()
	{
		state = GameState.ShowObjective;

		yield return new WaitForSeconds(1.0f);

		state = GameState.MainGame;
		InitialisePlayers();
	}

	void ShowObjective()
	{
		float unit = Screen.width/20;
		GUI.Box(new Rect(Screen.width/2-5*unit, Screen.height/2, unit*10, unit*3), "SCORE 5 POINTS AND YOU ARE GOOD");
	}

	void InitialisePlayers()
	{
		
		//based on number of players set, create players
		//always have player 1
		//turn off initial camera
		GameObject.Find("Main Camera").GetComponent<Camera>().enabled = false;

		int numPlayers = PlayerPrefs.GetInt("NumPlayers");
		print(numPlayers);
		_players = new GameObject[numPlayers];

		for(int i=0; i<numPlayers; i++)
		{
			_players[i] = (GameObject)Instantiate(playerPrefab, new Vector3(5*i,5,0), Quaternion.identity);
		    var player = _players[i].GetComponent<Player>();
		    player.playerNumber = i+1;
		    player.Respawn();
			_players[i].name = "Player"+(i+1);
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
}
