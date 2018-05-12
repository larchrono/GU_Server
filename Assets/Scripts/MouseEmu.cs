using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class MouseEmu : MonoBehaviour {

	public GameObject baseUser;
	public GameObject baseBullet;
	GameObject myUser = null;

	public bool useEmuNetwork;
	public int emuUserNumber;

	List<ArgsPosition> emuPersonPos = new List<ArgsPosition> ();

	TcpClient emuSocket = null;
	NetworkStream socketStream;
	Thread emuConnectThread;

	Vector3 userPoint;

	int emuScreenPositon_baseX;
	int emuScreenPositon_baseY;
	float EmuScreen_Width;
	float EmuScreen_Height;

	const int Depth_Width = 512;
	const int Depth_Height = 424;
	const int emu_Height = 160;

	// Use this for initialization
	IEnumerator Start () {
		
		emuScreenPositon_baseX = Mathf.FloorToInt(LogicSystem.current.SceneInScreenBase.x);
		emuScreenPositon_baseY = Mathf.FloorToInt(LogicSystem.current.SceneInScreenBase.y);
		EmuScreen_Width = LogicSystem.current.SceneInScreenWidth;
		EmuScreen_Height = LogicSystem.current.SceneInScreenHeight;

		if (!useEmuNetwork) {
			myUser = Instantiate (baseUser, new Vector3 (0, 0.5f, 0), Quaternion.identity);

		} else {
			yield return new WaitForSeconds (1.0f);
			emuConnectThread = new Thread (ConnectThread);
			emuConnectThread.Start ();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (emuPersonPos.Count != emuUserNumber) {
			if (emuUserNumber > emuPersonPos.Count) {
				for (int i = 0; i < emuUserNumber - emuPersonPos.Count; i++) {
					emuPersonPos.Add (new ArgsPosition { x = Random.Range (0, 512), y = Random.Range (0, 424), z = emu_Height });
				}
			}
			if (emuUserNumber < emuPersonPos.Count) {
				for (int i = emuPersonPos.Count ; i > emuUserNumber ; i--) {
					try {
						emuPersonPos.RemoveAt (i - 1);
					} catch(System.ArgumentOutOfRangeException){
					}
				}
			}
		}

		//Debug.Log (Input.mousePosition);
		RaycastHit hit; 
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
		if (Physics.Raycast (ray,　 out hit,　 100.0f)) {
			if (hit.collider.gameObject.tag == "Ground") {
				userPoint = new Vector3 (hit.point.x, 0.5f, hit.point.z);
				SmoothMove (myUser, userPoint);
				int screen_x = Mathf.FloorToInt((Input.mousePosition.x - emuScreenPositon_baseX) / EmuScreen_Width * Depth_Width);
				int screen_y = Mathf.FloorToInt((Input.mousePosition.y - emuScreenPositon_baseY) / EmuScreen_Height * Depth_Height);

				int num = emuPersonPos.Count + 1;
				string mes = "" + num + "," + screen_x + "," + screen_y + "," + emu_Height;

				for (int i = 0; i < emuPersonPos.Count; i++) {
					mes += "," + emuPersonPos [i].x + "," + emuPersonPos [i].y + "," + emu_Height;
				}

				WriteEmuPosition (mes);
			}
		}

//		if (Input.GetButtonDown ("Fire1")) {
//			Instantiate (baseBullet , userPoint , Quaternion.identity);
//		}

	}


	/*
	var pushDir = Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);
	// If you know how fast your character is trying to move,
	// then you can also multiply the push velocity by that.
	// Apply the push
	body.velocity = pushDir * pushPower;
	*/


	void SmoothMove(GameObject obj,Vector3 vect){
		if (obj == null)
			return;
		obj.transform.position = Vector3.Lerp(obj.transform.position, vect, 0.25f);
		if(obj.GetComponent<Rigidbody> () != null)
			obj.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	}

	void WriteEmuPosition(string message){
		if (emuSocket == null)
			return;
		if (emuSocket.Connected == false)
			return;

		message += "[/TCP]";
		byte[] data = System.Text.Encoding.ASCII.GetBytes(message);        
		socketStream.Write(data, 0, data.Length);
	}

	void ConnectThread(){
		try {
			emuSocket = new TcpClient ("127.0.0.1", 25566);
			socketStream = emuSocket.GetStream ();
		} catch (SocketException e) {
			Debug.LogWarning ("Port: 25566 connect fault , " + e.Message.ToString());
		}
	}
}
