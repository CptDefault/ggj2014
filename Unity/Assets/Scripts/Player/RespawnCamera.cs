using UnityEngine;
using System.Collections;

public class RespawnCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Activate(Player player)
    {
        StartCoroutine(ActiveCoroutine(player));
    }

    private IEnumerator ActiveCoroutine(Player player)
    {
        camera.enabled = true;
        player.gameObject.SetActive(false);


        yield return new WaitForSeconds(3);

        camera.enabled = false;

        player.gameObject.SetActive(true);
        player.Respawn();
    }
}
