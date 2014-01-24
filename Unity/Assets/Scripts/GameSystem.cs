using UnityEngine;
using System.Collections;

public class GameSystem : MonoBehaviour {


	public enum GameState {CharacterSelect, MainGame};
	public GameState state;

	public GameObject playerPrefab;

	private GameObject[] _players;

	// Use this for initialization
	void Start () {



	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() 
	{
		//select number of players
		float unit = Screen.width/20;

		GUI.Box(new Rect(Screen.width/2-unit*5, Screen.height/2-unit*5, unit*10, unit *10), "NUMBER OF PLAYERS");
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
			state = GameState.MainGame;
			InitialisePlayers();
		}
	}

	void InitialisePlayers()
	{
		//based on number of players set, create players
		//always have player 1
		int numPlayers = PlayerPrefs.GetInt("NumPlayers");
		_players = new GameObject[numPlayers];

		_players[0] = GameObject.Find("Player1");


		switch(numPlayers)
		{


			case 2:
			_players[1] = (GameObject)Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
			break;

			case 3:
			_players[1] = (GameObject)Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
			_players[2] = (GameObject)Instantiate(playerPrefab, new Vector3(0,1,0), Quaternion.identity);
			break;

			case 4:
			_players[1] = (GameObject)Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
			_players[2] = (GameObject)Instantiate(playerPrefab, new Vector3(0,1,0), Quaternion.identity);
			_players[3] = (GameObject)Instantiate(playerPrefab, new Vector3(0,2,0), Quaternion.identity);
			break;


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
