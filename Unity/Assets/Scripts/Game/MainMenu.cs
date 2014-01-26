using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public GUISkin mainSkin;

	public GUISkin controllerIcons;

	// Use this for initialization
	void Start () {
		mainSkin.GetStyle("Box").fontSize = (int)(Screen.height/20f);

		mainSkin.GetStyle("Button").fontSize = (int)(Screen.height/13f);
		mainSkin.GetStyle("Button").padding.left = (int)(Screen.height/7f);

        AudioManager.Instance.inMenu = true;

	}
	
		// Update is called once per frame
	void Update () {


		for(int i=0; i<4; i++)
		{
			if(Input.GetButtonUp("X_"+(i+1)))
			{

				Clicker.Instance.Click();
			}
			else if(Input.GetButtonUp("A_"+(i+1)))
			{
				AudioManager.Instance.inMenu = false;
				Application.LoadLevel(1);
				Clicker.Instance.Click();
			}
			else if(Input.GetButtonUp("Back_"+(i+1)))
			{
				//quit
				Application.Quit();
				Clicker.Instance.Click();
			}
		}
	
	}

	void OnGUI()
	{	
		GUI.skin = mainSkin;
		GUI.Box(new Rect(Screen.width-Screen.width*0.42f, Screen.height*0.36f, Screen.width*0.25f, Screen.height*0.1f), "", mainSkin.GetStyle("Samurai"));
		GUI.Box(new Rect(Screen.width-Screen.width*0.27f, Screen.height*0.375f, Screen.width*0.25f, Screen.height*0.1f), " GAME");

		GUI.Box(new Rect(Screen.width/2-Screen.width*0.4f, Screen.height*0.1f, Screen.width*0.8f, Screen.width*0.153f),"", mainSkin.GetStyle("Title"));

		GUI.Box(new Rect(Screen.width-Screen.width*0.2f, Screen.height-Screen.width*0.2f, Screen.width*0.2f, Screen.width*0.2f),"", mainSkin.GetStyle("Corner"));


		//buttons
		if(GUI.Button(new Rect(Screen.width/2-Screen.width*0.4f, Screen.height*0.5f, Screen.width*0.4f, Screen.height*0.1f), "DEATHMATCH"))
		{

		}

		if(GUI.Button(new Rect(Screen.width/2-Screen.width*0.4f, Screen.height*0.65f, Screen.width*0.4f, Screen.height*0.1f), "CREDITS"))
		{

		}

		if(GUI.Button(new Rect(Screen.width/2-Screen.width*0.4f, Screen.height*0.8f, Screen.width*0.4f, Screen.height*0.1f), "QUIT"))
		{

		}

		//icons
		GUI.Box(new Rect(Screen.width/2-Screen.width*0.4f+Screen.width*0.02f, Screen.height*0.505f, Screen.width*0.05f, Screen.width*0.05f),"", controllerIcons.GetStyle("A"));

		GUI.Box(new Rect(Screen.width/2-Screen.width*0.4f+Screen.width*0.02f, Screen.height*0.655f, Screen.width*0.05f, Screen.width*0.05f),"", controllerIcons.GetStyle("X"));

		GUI.Box(new Rect(Screen.width/2-Screen.width*0.4f+Screen.width*0.02f, Screen.height*0.805f, Screen.width*0.05f, Screen.width*0.05f),"", controllerIcons.GetStyle("Back"));


	}
}
