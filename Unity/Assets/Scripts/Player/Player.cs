using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public int playerNumber;
	public int score = 0;

	private Camera _myCamera;

	// Use this for initialization
	void Start () {
		_myCamera = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
