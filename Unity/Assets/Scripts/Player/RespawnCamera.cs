using UnityEngine;
using System.Collections;

public class RespawnCamera : MonoBehaviour {
    string _diedMessage;
    Rect _diedMessageRect; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Activate(Player player, string verb, string sender, Rect dr)
    {
        _diedMessageRect = dr;
          _diedMessage = "YOU GOT " + verb + " BY " + sender;
        StartCoroutine(ActiveCoroutine(player));
    }

    private IEnumerator ActiveCoroutine(Player player)
    {
        camera.enabled = true;
        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(3);

        camera.enabled = false;
        _diedMessage = null;

        if (player != null)
        {
            player.gameObject.SetActive(true);
            player.Respawn();
        }
    }

    void OnGUI()
    {
        if(_diedMessage != null)
            GUI.Box(_diedMessageRect, _diedMessage, DeathMessenger.Instance.messageSkin.GetStyle("Message"));
    }
}
