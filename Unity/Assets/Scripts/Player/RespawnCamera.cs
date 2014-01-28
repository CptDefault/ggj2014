using UnityEngine;
using System.Collections;

public class RespawnCamera : MonoBehaviour {
    string _diedMessage;
    Rect _diedMessageRect;
    private Player _player;
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Activate(Player player, string message, Rect dr)
    {
        _diedMessageRect = dr;
          _diedMessage = message;
        StartCoroutine(ActiveCoroutine(player));
    }

    private IEnumerator ActiveCoroutine(Player player)
    {
        camera.enabled = true;
        _player = player;
        _player.gameObject.SetActive(false);

        if (GameSystem.Instance.CurrentGameMode == GameSystem.GameMode.Elimination)
        {
            GameSystem.Instance.AddPendingRespawn(this);
        }
        else
        {
            yield return StartCoroutine(RespawnDelayed(3));
        }
    }

    public void Respawn(float delay)
    {
        StartCoroutine(RespawnDelayed(delay));

    }
    public IEnumerator RespawnDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        camera.enabled = false;
        _diedMessage = null;

        if (_player != null)
        {
            _player.gameObject.SetActive(true);
            _player.Respawn();
        }
    }

    void OnGUI()
    {
        if(_diedMessage != null)
            GUI.Box(_diedMessageRect, _diedMessage, DeathMessenger.Instance.messageSkin.GetStyle("Message"));
    }
}
