using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkInstance))]
public class TransformSync : MonoBehaviour {
	Rigidbody rb;
	NetworkInstance netInstance;
	[Range(1, 30)]
	public int framerate = 15;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		netInstance = GetComponent<NetworkInstance>();

		if(netInstance.isLocalPlayer)
			StartCoroutine("UpdatePosition");
		else
		{
			rb.constraints = RigidbodyConstraints.FreezeAll;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator UpdatePosition()
	{
		while(true)
		{
			if (rb.velocity != Vector3.zero || rb.angularVelocity != Vector3.zero)
			{
				ServerController.controller.UpdateLocalPositon(netInstance.id, transform);
			}
			yield return new WaitForSecondsRealtime(1.0f / framerate);
		}
	}
}
