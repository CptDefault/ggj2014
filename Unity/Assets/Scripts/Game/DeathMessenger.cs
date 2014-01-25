using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathMessenger : MonoBehaviour {

	private static DeathMessenger _instance;

	private string[] _verbs;

	public Player receiver;
	public Player sender;

	public GUISkin messageSkin;

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
		messageSkin.GetStyle("Message").fontSize = (int)(Screen.height/23f);

		_verbs = new []{"ATE", "WHACKED", "SHANKED", "SLAPPED", "JUDGED", "ELBOWED", "ELIMINATED", "DESTROYED", "REMOVED", "RENOVATED",
						"BLANKED", "HUNG", "SPANKED", "TENDERLY LOVED", "TOUCHED", "CONVERTED", "BEFRIENDED", "BLOCKED", "KILLED", "UNFRIENDED",
						"UNFOLLOWED", "#DEAD", "\"#YOLO'D\"", "JUST BIEBER'D", "REVENGEANCED", "CALIBRATED", "??????", "MASTICATED", "0x183D6DF", "FLUCTUATED", "SWAG",
						"SWAGGED", "SNAPPED", "PWNED", "CONSTRICTED", "FRATERNISED", "FRIENDZONED", "360MLGNOSCOPED" ,"SIGNED OUT", "ACQUIRED", "FIRED", "HIT", "SACKED",
						"SPLONKED", "WHOMPED", "SHUT DOWN", "BORKED", "DISCOVERED", "CONQUERED", "CRUNKED", "CARJACKED", "ROBBED", "THROTTLED", "RAGDOLLED",
						"MUNCHED", "OBLITERATED", "SANDWICHED", "SERVED", "BAKED", "FRIED", "DEEP FRIED", "JAVASCRIPT'D", "C-SHARPED", "BOO-ED", "BOO-URNS-ED",
						"CQC'D", "OVERHEATED", "SHOT", "SLAMMED", "JAMMED", "KANYE WEST", "SPACE JAMMED", "TAKEN", "ANNIHILATED", "ARMAGEDDONED", "CREAMED",
						"ENDED", "BROKEN", "SHATTERED", "DEMOLISHED", "ENVELOPED", "JUSTIFIED", "ZANDATSU'D", "GANGNAM STYLED", "DOGE-D", "AFK-D", "PUT ON THE INTERNET",
						"VIRUS SCANNED", "WINDOWS VISTA'D", "UNINSTALLED", "FRAGMENTED", "DELETED", "CORRUPTED", "FORMATTED", "LOL'D", "ROLLED", "SOLD",
						"BOUGHT", "TRADED", "CHEESECAKED", "SANTORUMED", "STUFFED", "BLASTED", "TONY ABBOTTED", "ELECTED", "POKED", "SCREENCHEATED"};
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void OnGUI() {


	}

	public string GetRandomVerb()
	{
		return _verbs[Random.Range(0,_verbs.Length)];
	}


	public static DeathMessenger Instance
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
