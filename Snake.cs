using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Snake : MonoBehaviour {

	private Snake next;
	static public Action<String> hit;
	public int dir, inputDir;
	public bool isEdge = false;


	public void SetNext (Snake IN){
		next = IN;
	}

	public Snake GetNext (){
		return next;
	}

	public void RemoveTail(){
		Destroy (this.gameObject);
	}

    //handle collision
	void OnTriggerEnter(Collider other){
		if (hit != null)
			hit (other.tag);
		if (other.tag == "Food") {
			Destroy (other.gameObject);
		}
	}

	public void setDir(int value){
		dir = value;
	}

	public void setInputDir(int value){
		inputDir = value;
		if (dir != inputDir)
			isEdge = true;
	}

	public bool IsEdge(){
		return isEdge;
	}

	public int getDir(){
		return dir;
	}

	public int getInputDir(){
		return inputDir;
	}

}
