using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	private static AudioManager _instance;

	public bool _mutedAll;
	public bool _mutedMusic;

	//music
	public AudioClip intro;
	public AudioClip loop;

	public AudioSource introSource;
	public AudioSource loopSource;

	void Awake()
	{
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			_instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		print("music " + PlayerPrefs.GetInt("MutedMusic"));
		if(PlayerPrefs.GetInt("MutedMusic")==1)
			_mutedMusic = true;
		else
			_mutedMusic = false;

		if(PlayerPrefs.GetInt("MutedAll")==1)
			_mutedAll = true;
		else
			_mutedAll = false;

		//play music
		introSource.clip = intro;
		loopSource.clip = loop;

		//set previous settings
		if(_mutedMusic)
		{
			introSource.volume = 0;
			loopSource.volume = 0;
		}
		else
		{
			introSource.volume = 1;
			loopSource.volume = 1;
		}

		if(_mutedAll)
			AudioListener.volume = 0;
		else
			AudioListener.volume = 1;


		StartCoroutine(IntroThenLoopMusic());
	}

	public void ToggleMuteAll()
	{
		if(_mutedAll) {
			AudioListener.volume = 1;
			_mutedAll = false;
		}
		else {
			AudioListener.volume = 0;
			_mutedAll = true;
		}

		SaveMuteSettings();
	}

	public void ToggleMuteMusic()
	{
		if(_mutedMusic)
		{
			introSource.volume = 1;
			loopSource.volume = 1;
			_mutedMusic = false;
		}
		else
		{
			introSource.volume = 0;
			loopSource.volume = 0;
			_mutedMusic = true;
		}

		SaveMuteSettings();
	}

	public void SaveMuteSettings()
	{
		if(_mutedMusic)
			PlayerPrefs.SetInt("MutedMusic", 1);
		else
			PlayerPrefs.SetInt("MutedMusic", 0);

		if(_mutedAll)
			PlayerPrefs.SetInt("MutedAll", 1);
		else
			PlayerPrefs.SetInt("MutedAll", 0);

		PlayerPrefs.Save();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator IntroThenLoopMusic()
	{
		yield return new WaitForSeconds(0.3f);
		//need two audio sources to avoid gap
		introSource.Play();
		yield return new WaitForSeconds(intro.length/*-0.08f*/);
		loopSource.Play();
	}

	public static AudioManager Instance
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
