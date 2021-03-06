﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour {

	public Camera touchCamera;
	public GameObject particleTouch;
	public Text textTouchCount;

	const float touchToWorldDepth = 10;

	void EmuSendTouch(){
		string emuMessage = "";
		int touchNum = Random.Range (1, 11);
		emuMessage += touchNum;
		int _x;
		int _y;
		for (int i = 0; i < touchNum; i++) {
			_x = Random.Range (0, LogicSystem.touchScreenWidth);
			_y = Random.Range (0, LogicSystem.touchScreenHeight);
			emuMessage += "," + _x + "," + _y;

			//LogicSystem.current.CreateIcon (_x,_y);
			CreateParticleTouch (_x, _y);
		}
		if(ServerGlass.current != null)
			ServerGlass.current.SocketSend (emuMessage);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.T)) {
			EmuSendTouch ();
		}

		if (Input.GetMouseButtonDown (0)) {
			if (LogicSystem.current.UseDebug) {
				int _x = Mathf.FloorToInt (Input.mousePosition.x);
				int _y = Mathf.FloorToInt (Input.mousePosition.y);
				//LogicSystem.current.CreateIcon (_x,_y);
				CreateParticleTouch (_x, _y);
				string message = "1," + _x + "," + _y;
				if (ServerGlass.current != null)
					ServerGlass.current.SocketSend (message);
			}
		}

		if (Input.touchSupported) {
			Touch[] myTouches = Input.touches;
			textTouchCount.text = "Touch Count : " + Input.touchCount;
			string message = "";
			int touchNum = 0;
			for (int i = 0; i < Input.touchCount; i++) {
				if (myTouches [i].phase == TouchPhase.Began) {
					touchNum++;
					int _x = Mathf.FloorToInt (myTouches [i].position.x);
					int _y = Mathf.FloorToInt (myTouches [i].position.y);
					message += "," + _x + "," + _y;
					//LogicSystem.current.CreateIcon (_x,_y);
					CreateParticleTouch (_x, _y);
				}
			}
			if (touchNum > 0) {
				message = "" + touchNum + message;
				if (ServerGlass.current != null)
					ServerGlass.current.SocketSend (message);
			}
		}
	}

	void CreateParticleTouch(float x , float y){
		Vector3 pos = touchCamera.ScreenToWorldPoint(new Vector3(x, y, touchToWorldDepth));
		Destroy (Instantiate (particleTouch, pos, Quaternion.identity), 2f);
	}
}
