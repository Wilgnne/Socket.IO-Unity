using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using Newtonsoft.Json;

struct Client {
	public string id;
	public Vector3 position;
	public Vector3 rotation;
}

struct SyncTransform
{
	public string id;
	public Vector3 position;
	public Vector3 rotation;

	public SyncTransform(string id, Vector3 position, Vector3 rotation)
	{
		this.id = id;
		this.position = position;
		this.rotation = rotation;
	}
}

struct RegisterState {
	public Client[] clients;
	public string myID;
}

[RequireComponent(typeof(SocketIOComponent))]
public class ServerController : MonoBehaviour {
	public static ServerController controller;

	private SocketIOComponent socket;
	public string id;

	public GameObject PlayerPrefab;

	public StringGameObjectDictionary clients;
	

	// Use this for initialization
	void Start () {
		if (!controller)
			controller = this;
		else
			Destroy(gameObject);

		socket = GetComponent<SocketIOComponent>();
		ConfigureSocket();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void ConfigureSocket()
	{
		socket.On("register", (e) =>
		{
			Debug.Log("[Register] Registrando PlayerLocal");

			RegisterState state = JsonConvert.DeserializeObject<RegisterState>(e.data.ToString());

			foreach (Client c in state.clients)
			{
				InstantiateClient(c);
			}
			id = state.myID;
		});

		socket.On("ddd", (e) =>
		{
			Debug.Log("[DisconectClient] Disconectando cliente: " + e.data);
		});

		socket.On("newClient", (e) =>
		{
			Client newClient = JsonConvert.DeserializeObject<Client>(e.data.ToString());

			if (clients.ContainsKey(newClient.id) == false)
			{
				Debug.Log("[NewClient] Registrando Cliente: " + newClient.id);
				InstantiateClient(newClient);
			}
		});

		socket.On("updateRemotePosition", (e) =>
		{
			SyncTransform sync = JsonConvert.DeserializeObject<SyncTransform>(e.data.ToString());
			if (sync.id != id)
			{
				GameObject syncClient = clients[sync.id];
				syncClient.transform.position = sync.position;
				syncClient.transform.eulerAngles = sync.rotation;
				//Debug.Log("[UpdateRemotePosition] Update Position from " + sync.id);
			}
		});
	}

	public void UpdateLocalPositon(string id, Transform syncTransform)
	{
		SyncTransform sync = new SyncTransform(id, syncTransform.position, syncTransform.eulerAngles);
		socket.Emit("updatePosition", new JSONObject(JsonConvert.SerializeObject(sync)));
	}

	void InstantiateClient(Client client)
	{
		Debug.Log("[InstantiateClient] Criando Client "+ client.id +" em: " + client.position.ToString());
		GameObject clientObject = Instantiate(PlayerPrefab, client.position, Quaternion.identity);
		clientObject.transform.eulerAngles = client.rotation;
		NetworkInstance clientNet = clientObject.GetComponent<NetworkInstance>();
		clientNet.id = client.id;

		clients.Add(client.id, clientObject);
	}
}
