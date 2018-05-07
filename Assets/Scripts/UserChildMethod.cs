using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserChildMethod : MonoBehaviour {

	public ParticleSystem Circle;
	public ParticleSystem Glow;

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
