using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour {

	public int depthX = 0;
	public int depthY = 0;

	public int score = 0;

	public Vector2 Position(){
		return new Vector2 (depthX, depthY);
	}

	public void Setup(int x , int y){
		depthX = x;
		depthY = y;
	}
}
