using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArgsPosition {

	public int x;
	public int y;
	public int z;

	public Vector2 Position(){
		return new Vector2 (x , y);
	}
}
