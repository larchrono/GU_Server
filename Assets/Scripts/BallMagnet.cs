using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMagnet : MonoBehaviour {

	public GameObject target;

	float force = 100f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(target != null)
			GetComponent<Rigidbody> ().AddForce ((target.transform.position - transform.position) * force * Time.smoothDeltaTime);
	}
}
