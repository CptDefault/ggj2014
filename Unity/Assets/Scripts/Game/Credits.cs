using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {
	public MovieTexture Video;

	float timer =0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.anyKeyDown)
		{
			Application.LoadLevel(0);
		}

		timer+=Time.deltaTime;

		if(timer >= 25)
		{
			Application.LoadLevel(0);
		}
	}

	// Update is called once per frame
	void OnGUI () {
	   Video.Play();
	   GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Video,ScaleMode.StretchToFill, false, 0.0f);
	   if (Video.isPlaying)
	   {
	     audio.Stop();
	   }


	}
}
