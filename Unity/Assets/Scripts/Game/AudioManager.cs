using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	private static AudioManager _instance;

	public bool _mutedAll;
	public bool _mutedMusic;

	//music
	public AudioClip intro;
	public AudioClip loop;
	public AudioClip loopQuiet;

	public AudioSource introSource;
	public AudioSource loopSource;
	public AudioSource loopQuietSource;

    public bool useQuietMusic;
    public bool skipIntro;

    private float _quietLoudBalance = 0;

	//sound effects
	public AudioSource killConfirmedSource;

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

		StartCoroutine(IntroThenLoopMusic());
	}

    public void Update()
    {
        float targetLevel = useQuietMusic ? 0 : 1;
        if (targetLevel > _quietLoudBalance)
            _quietLoudBalance += Time.deltaTime;
        else if (targetLevel < _quietLoudBalance)
            _quietLoudBalance -= Time.deltaTime;
        _quietLoudBalance = Mathf.Clamp01(_quietLoudBalance);

        //set previous settings
        if (_mutedMusic)
        {
            introSource.volume = 0;
            loopSource.volume = 0;
        }
        else
        {
            introSource.volume = 1;
            loopSource.volume = 1 * _quietLoudBalance;
            loopQuietSource.volume = 1 * (1 - _quietLoudBalance);
        }

        if (_mutedAll)
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1;
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

	IEnumerator IntroThenLoopMusic()
	{
		yield return new WaitForSeconds(0.3f);
		//need two audio sources to avoid gap
	    if (!skipIntro)
	    {
	        introSource.Play();
	        yield return new WaitForSeconds(intro.length /*-0.08f*/);
	    }
	    loopSource.Play();
		loopQuietSource.Play();
	}

	public void PlayKillConfirmed()
	{
		killConfirmedSource.Play();
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
