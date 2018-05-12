using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayScript : MonoBehaviour
{
	public static DisplayScript current;
	public Camera cameraWalk;
	public Camera cameraTouch;
	public Canvas canvasMain;

	public delegate void DisplayChange(int touchCameraId);
	public event DisplayChange displayChangeEvent;

	void Awake(){
		current = this;
	}

	// Use this for initialization
	void Start()
	{
		Debug.Log("displays connected: " + Display.displays.Length);
		// Display.displays[0] is the primary, default display and is always ON.
		// Check if additional displays are available and activate each.
		if (Display.displays.Length > 1)
			Display.displays[1].Activate();
		if (Display.displays.Length > 2)
			Display.displays[2].Activate();
	}

	void Update(){
		if (Input.GetButtonDown ("Option")) {
			Debug.Log ("Option");
			var temp = cameraWalk.targetDisplay;
			cameraWalk.targetDisplay = cameraTouch.targetDisplay;
			cameraTouch.targetDisplay = temp;
			canvasMain.targetDisplay = cameraTouch.targetDisplay;

			if (displayChangeEvent != null)
				displayChangeEvent (cameraTouch.targetDisplay);
		}
	}

	public void SetupTouchScreenResolutionWidth(InputField width){
		if (Display.displays.Length <= cameraTouch.targetDisplay)
			return;
		width.text = "" + Display.displays [cameraTouch.targetDisplay].systemWidth;
	}

	public void SetupTouchScreenResolutionHeight(InputField height){
		if (Display.displays.Length <= cameraTouch.targetDisplay)
			return;
		height.text = "" + Display.displays [cameraTouch.targetDisplay].systemHeight;
	}
}