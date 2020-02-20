using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkInstance))]
public class MoveBehaviour : MonoBehaviour {
	Rigidbody rb;
	NetworkInstance netInstance;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		netInstance = GetComponent<NetworkInstance>();
	}
	
	// Update is called once per frame
	void Update () {
		if (netInstance.isLocalPlayer)
			rb.velocity = Vector3.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("Vertical");
	}
}
