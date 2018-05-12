using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOption : MonoBehaviour {

	public GameObject panelToOpen;

	int clickTime = 0;

	void OnEnable()
	{
		clickTime = 0;
	}

	public void Click(){
		clickTime++;
		if (clickTime >= 10) {
			panelToOpen.SetActive (true);
			gameObject.SetActive (false);
		}
	}
}
