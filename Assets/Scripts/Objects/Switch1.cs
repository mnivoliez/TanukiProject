using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionLantern{Activate, Deactivate};

[System.Serializable]
struct SwitchObject
{
	public GameObject gameObject;
	public ActionLantern action;
}

public class Switch1 : MonoBehaviour
{
	[SerializeField] private SwitchObject[] actionOnSwitch;

    // Use this for initialization
    void Start()
    {
    }


    void OnCollisionEnter(Collision collider)
    {
		foreach(SwitchObject obj in actionOnSwitch)
		{
			if (obj.gameObject != null)
			{
				obj.gameObject.SetActive (obj.action.Equals (ActionLantern.Activate));
			}
		}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
