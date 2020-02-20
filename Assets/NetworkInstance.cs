using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class NetworkInstance : MonoBehaviour {

	public string id;
	public bool isLocalPlayer
	{
		get { return _isLocalPlayer; }
		set { }
	}
	private bool _isLocalPlayer;

	public void Start()
	{
		_isLocalPlayer = ServerController.controller.id == id;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
