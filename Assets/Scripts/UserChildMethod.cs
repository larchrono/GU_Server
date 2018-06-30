using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserChildMethod : MonoBehaviour {

	public ParticleSystem Circle;
	public ParticleSystem Glow;

	public Text textScorePrefab;

	public Text textScore;

	void Awake(){
		Transform canv = GameObject.Find ("CanvasWalk").transform;
		if (canv != null) {
			textScore = Instantiate (textScorePrefab, canv, false);
		}
	}

	public void SetColorRandom(){
		float h , s , v , target_h;
		target_h = Random.Range (0f, 1f);

		var main = Circle.main;
		Color.RGBToHSV(main.startColor.color, out h , out s , out v);
		main.startColor = Color.HSVToRGB (target_h, s, v);

		main = Glow.main;
		Color.RGBToHSV(main.startColor.color, out h , out s , out v);
		main.startColor = Color.HSVToRGB (target_h, s, v);

	}
}
