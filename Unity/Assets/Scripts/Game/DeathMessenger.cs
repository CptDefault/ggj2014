using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathMessenger : MonoBehaviour {

	private static DeathMessenger _instance;

    [System.Serializable]
    public class KillMessage
    {
        public string victimMessage;
        public string killerMessage;

        public KillMessage(string victimMessage, string killerMessage)
        {
            this.victimMessage = victimMessage;
            this.killerMessage = killerMessage;
        }
    }

	private string[] _verbs = new []{"ATE", "WHACKED", "SHANKED", "SLAPPED", "JUDGED", "ELBOWED", "ELIMINATED", "DESTROYED", "REMOVED", "RENOVATED",
						"BLANKED", "HUNG", "SPANKED", "TENDERLY LOVED", "TOUCHED", "CONVERTED", "BEFRIENDED", "BLOCKED", "KILLED", "UNFRIENDED",
						"UNFOLLOWED", "#DEAD", "\"#YOLO'D\"", "JUST BIEBER'D", "REVENGEANCED", "CALIBRATED", "??????", "MASTICATED", "0x183D6DF", "FLUCTUATED", "SWAG",
						"SWAGGED", "SNAPPED", "PWNED", "CONSTRICTED", "FRATERNISED", "FRIENDZONED", "360MLGNOSCOPED" ,"SIGNED OUT", "ACQUIRED", "FIRED", "HIT", "SACKED",
						"SPLONKED", "WHOMPED", "SHUT DOWN", "BORKED", "DISCOVERED", "CONQUERED", "CRUNKED", "CARJACKED", "ROBBED", "THROTTLED", "RAGDOLLED",
						"MUNCHED", "OBLITERATED", "SANDWICHED", "SERVED", "BAKED", "FRIED", "DEEP FRIED", "JAVASCRIPT'D", "C-SHARPED", "BOO-ED", "BOO-URNS-ED",
						"CQC'D", "OVERHEATED", "SHOT", "SLAMMED", "JAMMED", "KANYE WEST", "SPACE JAMMED", "TAKEN", "ANNIHILATED", "ARMAGEDDONED", "CREAMED",
						"ENDED", "BROKEN", "SHATTERED", "DEMOLISHED", "ENVELOPED", "JUSTIFIED", "ZANDATSU'D", "GANGNAM STYLED", "DOGE-D", "AFK-D",
						"VIRUS SCANNED", "WINDOWS VISTA'D", "UNINSTALLED", "FRAGMENTED", "DELETED", "CORRUPTED", "FORMATTED", "LOL'D", "ROLLED", "SOLD",
						"BOUGHT", "TRADED", "CHEESECAKED", "SANTORUMED", "STUFFED", "BLASTED", "TONY ABBOTTED", "ELECTED", "POKED", "SCREENCHEATED", 
                        "ADDITIONAL PYLONS", "FRIDGED", "OBAMA'D", "IGDAM'D", "GGJ'D", "LOKI'D", "SMASHED", "KILLED", "DUELLED", "BESMIRCHED", "FONDLED",
                        "RICKROLLED", "NaN", "STORMED OUT", "SCOOPED", "TRADED IN", "TAKEN TO THE POOL", "DAMPENED", "TWEETED", "SNIFFED", "FACEPLANTED", "ECSCAVATED", "TWERKED",
                        "PLANKED", "TANKED", "BONKED", "SMOKED", "VENTILATED", "FETISHED", "NIDHOGGED"

        };


    private KillMessage[] _killMessages = new []
        {
            new KillMessage("{1} ENDED YOU", "YOU ENDED {0}"),
            new KillMessage("YOU GOT PUT IN YOUR PLACE BY {1}", "YOU PUT {0} IN THEIR PLACE"), 
            new KillMessage("@{1}: @{0} #dead #screencheat", "@{0} #dead #screencheat"), 
            new KillMessage("{1} RIGGED THE ELECTION", "YOU RIGGED {0}'S ELECTION"), 
            new KillMessage("{1} KILLED YOU. THANKS OBAMA.", "YOU KILLED {0}"), 
            new KillMessage("{1} STOLE THE SPOTLIGHT", "YOU STOLE {0}'S SPOTLIGHT"),
            new KillMessage("KILLED BY {1}, YOU WERE", "KILLED {0}, YOU DID"), 
            new KillMessage("{1} PUT YOU ON THE INTERNET", "YOU PUT {0} ON THE INTERNET"), 
            
            
        };

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
	}
	
    public KillMessage GetRandomMessage()
    {
        if (Random.value < _killMessages.Length/(float) (_killMessages.Length + _verbs.Length))
        {
            return _killMessages[Random.Range(0, _killMessages.Length)];
        }
        else
        {
            string verb = GetRandomVerb();
            return new KillMessage("YOU GOT "+ verb + " BY {1}", "YOU " + verb + " {0}");
        }
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
