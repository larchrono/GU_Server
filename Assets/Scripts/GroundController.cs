using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour {

	//To run Method in Unity main Thread
	delegate void NetworkThreadWork();
	NetworkThreadWork functionCallback = null;

	// Use this for initialization
	void Start () {
		ServerGlass.recieveGlassEvent += OnRecieveBallCollision;
	}

	// Update is called once per frame
	void Update () {
		if (functionCallback != null) {
			functionCallback.Invoke ();
			functionCallback = null;
		}
	}

	// coordinate from args is Touch screen Size
	void OnRecieveBallCollision(ArgsPosition[] args){
		functionCallback += delegate {
			for (int i = 0; i < args.Length; i++) {
				LogicSystem.current.CreateIcon (args [i].x, args [i].y);
			}
		};
	}
}
