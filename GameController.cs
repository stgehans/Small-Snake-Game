using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Text scoreText;
	public int maxSize;
	public int currentSize;
	public int xBound;
	public int yBound;
	public int score;
	public int NESW, lastNESW;
	public GameObject snakePrefab;
	public GameObject foodPrefab;
	public GameObject currentFood;
	public Snake head; 
	public Snake tail;
	public Vector2 nextPos;
	public float deltaTime;

	void OnEnable(){
		Snake.hit += hit;
	}

	void Start () {
		InvokeRepeating ("TimerInvoke", 0, .15f);
		foodFunction ();
	}

	void OnDisable(){
		Snake.hit -= hit;
	}

	void Update () {
		ComChangeD ();
		Materialize ();
	}

    //actual update 
	void TimerInvoke(){
		movement ();
		StartCoroutine (checkVisable ());
		if (currentSize >= maxSize)
			TailFunction ();
		else
			currentSize++;
	}

    //...
	void movement() {
		GameObject temp;
		nextPos = head.transform.position;
		switch (NESW) {
		case 0:
			nextPos = new Vector2 (nextPos.x, nextPos.y + 2);
			break;
		case 1:
			nextPos = new Vector2 (nextPos.x + 2, nextPos.y);
			break;
		case 2:
			nextPos = new Vector2 (nextPos.x, nextPos.y - 2);
			break;
		case 3:
			nextPos = new Vector2 (nextPos.x - 2, nextPos.y);
			break;
		}
		temp = (GameObject)Instantiate (snakePrefab, nextPos, transform.rotation);
		head.SetNext (temp.GetComponent<Snake> ());
		head.setDir(NESW);
		head.setInputDir (lastNESW);
		lastNESW = NESW;
		head = temp.GetComponent<Snake> ();

		if (NESW == 0) {
			head.transform.GetChild (2).gameObject.SetActive (true);
		}
		if (NESW == 1) {
			head.transform.GetChild (1).gameObject.SetActive (true);
		}
		if (NESW == 2) {
			head.transform.GetChild (3).gameObject.SetActive (true);
		}
		if (NESW == 3) {
			head.transform.GetChild (0).gameObject.SetActive (true);
		}
	}

    //change direction on computer
	void ComChangeD(){
		if (NESW != 2 && Input.GetKeyDown (KeyCode.W)) {
			lastNESW = NESW;
			NESW = 0;
		}
		if (NESW != 3 && Input.GetKeyDown (KeyCode.D)) {
			lastNESW = NESW;
			NESW = 1;
		}
		if (NESW != 0 && Input.GetKeyDown (KeyCode.S)) {
			lastNESW = NESW;
			NESW = 2;
		}
		if (NESW != 1 && Input.GetKeyDown (KeyCode.A)) {
			lastNESW = NESW;
			NESW = 3;
		}
	}

    // change direction on Mobile
	public void MobChangeD(int direction){
		if (NESW != 2 && direction == 0) {
			lastNESW = NESW;
			NESW = direction;
		}
		if (NESW != 3 && direction == 1) {
			lastNESW = NESW;
			NESW = direction;
		}
		if (NESW != 0 && direction == 2) {
			lastNESW = NESW;
			NESW = direction;
		}
		if (NESW != 1 && direction == 3) {
			lastNESW = NESW;
			NESW = direction;
		}
	}

    //processes movement on tail after head is moved
	void TailFunction(){
		Snake tempSnake = tail;
		tail = tail.GetNext ();
		tempSnake.RemoveTail ();
	}

    //spawns food
	void foodFunction (){
		int xPos = Random.Range (-(xBound * 2), xBound * 2);
		int yPos = Random.Range (-(yBound * 2), yBound * 2);
		currentFood = (GameObject)Instantiate (foodPrefab, new Vector2(xPos, yPos), transform.rotation);
		int randChild = Random.Range (0, 10);
		currentFood.transform.GetChild (randChild).gameObject.SetActive (true);
		StartCoroutine (CheckRender (currentFood));
	}

    //checks if instantiated food is inside rendered area, destroys food and spawns new if not
	IEnumerator CheckRender(GameObject IN){
		yield return new WaitForEndOfFrame();
		if (IN.GetComponent<Renderer>().isVisible == false) {
			if (IN.tag == "Food") {
				Destroy (IN);
				foodFunction ();
			}
		}
	}

    //process 'snail hits something' event
	void hit(string WhatWasSent){
		if(WhatWasSent == "Food"){
			GameObject[] foods = GameObject.FindGameObjectsWithTag ("Food");
			if(foods.Length == 1)
				foodFunction();
			maxSize++;
			score++;
			scoreText.text = score.ToString ();
			int temp = PlayerPrefs.GetInt ("HighScore");
			if (score > temp) {
				PlayerPrefs.SetInt ("HighScore", score);
			}
		}
		if (WhatWasSent == "Snake") {
			CancelInvoke ("TimerInvoke");
			Exit ();
		}
	}

    //end game
	public void Exit(){
		SceneManager.LoadScene (0);
	}


    //reposition snake if head goes out of rendered area
	void wrap(){
		if (NESW == 0) {
			head.transform.position = 
				new Vector2 (head.transform.position.x,
				- (head.transform.position.y - 1));
		}
		else if (NESW == 1) {
			head.transform.position =
				new Vector2 ( - (head.transform.position.x - 1),
					head.transform.position.y);
		}
		else if (NESW == 2) {
			head.transform.position = 
				new Vector2 (head.transform.position.x,
					- (head.transform.position.y + 1));
		}
		else if (NESW == 3) {
			head.transform.position =
				new Vector2 ( -(head.transform.position.x + 1),
					head.transform.position.y);
		}
	}

    //checks if head is in rendered area
	IEnumerator checkVisable(){
		yield return new WaitForEndOfFrame ();
		if (!head.GetComponent<Renderer>().isVisible) {
			wrap ();
		}
	}

    //"render" the snake right
	public void Materialize(){
		GameObject[] snakesAsGO = GameObject.FindGameObjectsWithTag ("Snake");
		Snake[] snakesAsSnakes = new Snake[snakesAsGO.Length];
		for (int i = 0; i < snakesAsGO.Length - 1; i++) {
			snakesAsSnakes [i] = snakesAsGO [i].GetComponent<Snake> ();
		}
		for (int i = 1; i < snakesAsSnakes.Length - 1; i++) {

			snakesAsSnakes [i].transform.GetChild (0).gameObject.SetActive (false);
			snakesAsSnakes [i].transform.GetChild (1).gameObject.SetActive (false);
			snakesAsSnakes [i].transform.GetChild (2).gameObject.SetActive (false);
			snakesAsSnakes [i].transform.GetChild (3).gameObject.SetActive (false);

			if (snakesAsSnakes [i].IsEdge ()) {
				if ((snakesAsSnakes [i].getInputDir () == 1 && snakesAsSnakes [i].getDir () == 0) || (snakesAsSnakes [i].getInputDir () == 2 && snakesAsSnakes [i].getDir () == 3)) {
					snakesAsSnakes [i].transform.GetChild (10).gameObject.SetActive (true);
					continue;
				}
				if ((snakesAsSnakes [i].getInputDir () == 1 && snakesAsSnakes [i].getDir () == 2) || (snakesAsSnakes [i].getInputDir () == 0 && snakesAsSnakes [i].getDir () == 3)) {
					snakesAsSnakes [i].transform.GetChild (11).gameObject.SetActive (true);
					continue;
				}
				if ((snakesAsSnakes [i].getInputDir () == 3 && snakesAsSnakes [i].getDir () == 2) || (snakesAsSnakes [i].getInputDir () == 0 && snakesAsSnakes [i].getDir () == 1)) {
					snakesAsSnakes [i].transform.GetChild (12).gameObject.SetActive (true);
					continue;
				}
				if ((snakesAsSnakes [i].getInputDir () == 3 && snakesAsSnakes [i].getDir () == 0) || (snakesAsSnakes [i].getInputDir () == 2 && snakesAsSnakes [i].getDir () == 1)) { 
						snakesAsSnakes [i].transform.GetChild (13).gameObject.SetActive (true);
						continue;
				}
			}

			if (snakesAsSnakes [i].getDir () == 1 || snakesAsSnakes [i].getDir () == 3) {
				snakesAsSnakes [i].transform.GetChild (4).gameObject.SetActive (true);
			}
			if (snakesAsSnakes [i].getDir () == 0 || snakesAsSnakes [i].getDir () == 2) {
				snakesAsSnakes [i].transform.GetChild (5).gameObject.SetActive (true);
			}
		}
		foreach (Transform child in tail.transform)
			child.gameObject.SetActive (false);

		if (tail.getDir () == 0) {
			tail.transform.GetChild (8).gameObject.SetActive (true);
		}
		if (tail.getDir () == 2) {
			tail.transform.GetChild (9).gameObject.SetActive (true);
		}
		if (tail.getDir () == 3) {
			tail.transform.GetChild (6).gameObject.SetActive (true);
		}
		if (tail.getDir () == 1) {
			tail.transform.GetChild (7).gameObject.SetActive (true);
		}
	}
}
