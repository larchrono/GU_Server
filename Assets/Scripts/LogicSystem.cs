﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicSystem : MonoBehaviour {
	public static LogicSystem current;
	public GameObject pointLeftBottom;
	public GameObject pointRightup;
	public GameObject iconBase;
	public GameObject userBase;
	public List<GameObject> usersSet = new List<GameObject>();

	public Light LightSpot;

	public const int touchScreenWidth = 640;
	public const int touchScreenHeight = 480;

	public Vector3 SceneInScreenBase;
	public Vector3 SceneInScreenEnd;
	public float SceneInScreenWidth;
	public float SceneInScreenHeight;

	const float sceneWidth = 20f;
	const float sceneHeight = 20f;
	const float iconHeight = 1f;
	const float userHeight = 0.5f;
	const float iconInitHeight = 10f;

	const int Depth_Width = 512;
	const int Depth_Height = 424;

	const float max_x = sceneWidth / 2f;
	const float min_x = -sceneWidth / 2f;
	const float max_z = sceneHeight / 2f;
	const float min_z = -sceneHeight / 2f;

	const float SpotLightheight = 10;


	public Vector3 View_MapCenter;


	//To run Method in Unity main Thread
	delegate void NetworkThreadWork();
	NetworkThreadWork functionCallback = null;

	void Awake(){
		current = this;

		SceneInScreenBase = Camera.main.WorldToScreenPoint (pointLeftBottom.transform.position);
		SceneInScreenEnd = Camera.main.WorldToScreenPoint (pointRightup.transform.position);
		SceneInScreenWidth = SceneInScreenEnd.x - SceneInScreenBase.x;
		SceneInScreenHeight = SceneInScreenEnd.y - SceneInScreenBase.y;
	}

	// Use this for initialization
	void Start () {
		if (iconBase == null)
			Debug.LogWarning ("Not Set iconBase !!");

		ServerPosition.recievePositionEvent += OnRecieveUserPosition;
	}

	// Update is called once per frame
	void Update () {
		if (functionCallback != null) {
			functionCallback.Invoke ();
			functionCallback = null;
		}

		for (int i = 0; i < usersSet.Count; i++) {
			UserData loc = usersSet [i].GetComponent<UserData> ();
			SmoothMove(usersSet[i], new Vector3(DepthToSceneX(loc.depthX) , userHeight , DepthToSceneZ(loc.depthY)));
		}

		LightSpot.transform.position = View_MapCenter = new Vector3 (GetUsersCenter ().x, SpotLightheight, GetUsersCenter ().z);
	}

	public void CreateIcon(int x, int y){
		StartCoroutine (GroundShower (x, y));
	}

	IEnumerator GroundShower(int x,int y){
		yield return new WaitForSeconds (0.1f);
		Instantiate (iconBase, new Vector3 (TouchToSceneX (x), iconInitHeight, TouchToSceneZ (y)), Quaternion.identity);
	}

	//In Unity , (0,0) is at screen left bottom
	float TouchToSceneX(int src){
		return ((src * 1.0f) / touchScreenWidth) * sceneWidth - (sceneWidth / 2);
	}
	float TouchToSceneZ(int src){
		return ((src * 1.0f) / touchScreenHeight) * sceneHeight - (sceneHeight / 2);
	}
	float DepthToSceneX(int src){
		return ((src * 1.0f) / Depth_Width) * sceneWidth - (sceneWidth / 2);
	}
	float DepthToSceneZ(int src){
		return ((src * 1.0f) / Depth_Height) * sceneHeight - (sceneHeight / 2);
	}


	Vector3 GetUsersCenter(){
		if (usersSet.Count == 0)
			return Vector3.zero;
		
		float center_x = 0;
		float center_y = 0;
		float center_z = 0;

		for (int i = 0; i < usersSet.Count; i++) {
			center_x += usersSet [i].transform.position.x;
			center_y += usersSet [i].transform.position.y;
			center_z += usersSet [i].transform.position.z;
		}
		center_x /= usersSet.Count;
		center_y /= usersSet.Count;
		center_z /= usersSet.Count;

		return new Vector3 (center_x, center_y, center_z);
	}

	void OnRecieveUserPosition(ArgsPosition[] args){
		functionCallback += delegate {
			int numToCreate = args.Length - usersSet.Count;
			if (numToCreate > 0) {
				for (int i = 0; i < numToCreate; i++) {
					GameObject temp = Instantiate (userBase, new Vector3 (0, -100, 0), Quaternion.identity);
					usersSet.Add (temp);

					//Change Color when it isn't first object
					if(temp != usersSet[0]){
						temp.GetComponent<UserChildMethod>().SetColorRandom();
					}
				}
			}
			if(numToCreate < 0){
				for(int i = usersSet.Count; i > args.Length ; i--){
					Destroy(usersSet[i - 1]);
					usersSet.RemoveAt(i - 1);
				}
			}
				
//			PositionTracking[] pts = new PositionTracking[args.Length];
//			for(int i = 0 ; i < pts.Length ; i++)
//				pts[i] = new PositionTracking(args[i]);
//			List<GameObject> remainUser;
//			List<ArgsPosition> remainArgs;


			//Step1. Get Every Object the min distance to args
			bool[] hasArgsPick = new bool[args.Length];
			for(int a=0 ; a < usersSet.Count ; a++){
				UserData data = usersSet[a].GetComponent<UserData>();
				float fitMin = float.MaxValue;
				int fitIndex = 0;

				//Search the minimun Distance and it's Args index
				for(int b=0 ; b < args.Length ; b++){
					if(hasArgsPick[b])
						continue;
					float dis = Vector2.Distance(data.Position(),args[b].Position());
					if(dis < fitMin){
						fitMin = dis;
						fitIndex = b;
					}
				}


//				if(pts[fitIndex].fitDistance == float.MaxValue){
//					pts[fitIndex].fitID = a;
//					pts[fitIndex].fitDistance = fitMin;
//					pts[fitIndex].hasArgsPick = true;
//				} else if(fitMin < pts[fitIndex].fitDistance) {
//					remainUser.Add(usersSet[pts[fitIndex].fitID]);
//					pts[fitIndex].fitID = a;
//					pts[fitIndex].fitDistance = fitMin;
//					pts[fitIndex].hasArgsPick = true;
//				}

				hasArgsPick[fitIndex] = true;
				usersSet[a].GetComponent<UserData>().Setup(args[fitIndex].x,args[fitIndex].y);
			}


//			for(int b = 0; b < pts.Length ; b++){
//				if(pts[b].hasArgsPick == true){
//
//				} else {
//					remainArgs.Add(args[b]);
//				}
//			}

		};
	}

	void SmoothMove(GameObject obj,Vector3 vect){
		if (obj == null)
			return;
		obj.transform.position = Vector3.Lerp(obj.transform.position, vect, 0.25f);
		if(obj.GetComponent<Rigidbody> () != null)
			obj.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	}
}
