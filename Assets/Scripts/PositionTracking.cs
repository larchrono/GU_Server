using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracking {

	public int x;
	public int y;
	public bool hasArgsPick;
	public float fitDistance;
	public int fitID;

	public PositionTracking(ArgsPosition arg){
		x = arg.x;
		y = arg.y;
		fitDistance = float.MaxValue;
	}
}
