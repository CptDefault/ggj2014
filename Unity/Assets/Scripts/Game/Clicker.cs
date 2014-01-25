using UnityEngine;
using System.Collections;

public class Clicker : MonoBehaviour {

	private static Clicker _instance = null;
	public static Clicker Instance {
		get {return _instance;}
	}

	public AudioSource click;
	public AudioSource start;

	void Awake() {

		if(_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
			_instance = this;

		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Click()
	{
		click.Play();
	}


	public void StartGameSound()
	{
		//audio.clip = startGame;
		start.Play();
		//audio.clip = click;
	}
}
