using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	public Text hS;


	void Start () {
		HSFunction ();
	}

    void Update () {
		
	}

	public void Play(){
		SceneManager.LoadScene (1);
	}

	void HSFunction(){
		hS.text = PlayerPrefs.GetInt ("HighScore").ToString();
	}
}
