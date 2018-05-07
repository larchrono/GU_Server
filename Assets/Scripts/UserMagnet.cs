using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMagnet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public float pullRadius;
	public float pullForce;

	void FixedUpdate() {
		
		foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius))
		{
			if (collider.tag == "Ball") {

				//Debug.Log (collider.name);

				// calculate direction from target to me
				Vector3 forceDirection = transform.position - collider.transform.position;

				//Debug.Log (forceDirection);

				//Debug.Log ("Direction : " + forceDirection);

				if (forceDirection.magnitude == 0)
					continue;

				Vector3 useForce = (forceDirection.normalized * pullForce * Time.fixedDeltaTime) / (forceDirection.magnitude);

				//Debug.Log (useForce);

				// apply force on target towards me
				collider.GetComponent<Rigidbody> ().AddForce (useForce);

				//Debug.Log ("force use:" + useForce);
			}
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		//Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
		Gizmos.DrawWireSphere (transform.position , pullRadius);
	}
}
