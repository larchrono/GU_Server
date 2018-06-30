using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCollision : MonoBehaviour {

	public GameObject ExploreObject;

	public Sprite[] RandomSprites;

	// Use this for initialization
	void Start () {
		
	}

	void OnCollisionEnter(Collision c){
		if (c.gameObject.tag == "Ball") {
			ParticleSystem temp = Instantiate (ExploreObject, c.transform.position, Quaternion.Euler (-90, 0, 0)).GetComponent<ParticleSystem> ();
			if (RandomSprites.Length > 0)
				temp.textureSheetAnimation.SetSprite (0, RandomSprites [Random.Range (0, RandomSprites.Length)]);
			Destroy (temp.gameObject,8);
			Destroy (c.gameObject);

			//2018.06.29 Score++
			var score = GetComponent<UserData>().score++;
			GetComponent<UserChildMethod> ().textScore.text = "" + score;
		}
	}
	
	void OnCollisionStay_X(Collision c)
	{
		// force is how forcefully we will push the player away from the enemy.
		float force = 300;

		// If the object we hit is the enemy
		if (c.gameObject.tag == "Ball")
		{
			//Debug.Log (c.contacts[0].point);
			// Calculate Angle Between the collision point and the player
			Vector3 dir = c.contacts[0].point - transform.position;
			// We then get the opposite (-Vector3) and normalize it
			dir = dir.normalized;
			// And finally we add force in the direction of dir and multiply it by force. 
			// This will push back the player
			c.gameObject.GetComponent<Rigidbody>().AddForce(dir*force);
		}
	}

}
