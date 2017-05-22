// The script is in an experimental stage. We welcome your suggestions and feedback to improve the functionality.
// version 0.25 / Georg Zaim / 3DBakers

using UnityEngine;
using System.Collections;
using s3DBakers.Buttons;
using Valve.VR.InteractionSystem;

public class s3DBButton_sender : Interactable {
	
	public str3DdBbReceiver[] SendToGameObjects;
	string senderName = "";

	void Start()
	{
		if(GameManager.Instance == null)
			GameManager.Instance.Init ();
	}

	private void HandHoverUpdate( Hand hand )
	{
		if (hand.controller.GetHairTriggerDown ()) {
			SendToObjects ();
		}
	}

	public void SetName(string n)
	{
		senderName = n;
	}

	public string GetName()
	{
		return senderName;
	}

	public void SendToObjects(){
		str3DBbMessage msg;
		msg.GO = this.gameObject;

		for (int c = 0; c < SendToGameObjects.Length; c++) {

			msg.actions = SendToGameObjects [c].actions;
			msg.state = SendToGameObjects [c].Switch;

			SendToGameObjects [c].receiver.SendMessage ("button", msg);
				
			}

		GameManager.Instance.CheckMessage (this);
	}

//Just for test
	void OnMouseOver(){
		if (Input.GetMouseButtonDown (0)) {
			SendToObjects ();
		}

		if (name != null) {
			UIManager.Instance.DisplaySenderName (this);
		}
	}

	void OnDrawGizmos()
	{
		if (Application.isPlaying) {
			if (GameManager.Instance.GetCurrentTarget () == this) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere (transform.position, 0.5f);
			}
		}
	}
}
