using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBloc : MonoBehaviour
{
	[SerializeField] private ActionLantern lanternAction;

    private Collider boxCollider;
    private MeshRenderer meshRenderer;

    // Use this for initialization
    void Start()
    {
        boxCollider = transform.parent.GetComponent<Collider>();
        meshRenderer = transform.parent.GetComponent<MeshRenderer>();
		boxCollider.enabled = lanternAction.Equals(ActionLantern.Deactivate);
		meshRenderer.enabled = lanternAction.Equals(ActionLantern.Deactivate);
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Lantern")
        {
			boxCollider.enabled = lanternAction.Equals(ActionLantern.Activate);
			meshRenderer.enabled = lanternAction.Equals(ActionLantern.Activate);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Lantern")
        {
			boxCollider.enabled = lanternAction.Equals(ActionLantern.Deactivate);
			meshRenderer.enabled = lanternAction.Equals(ActionLantern.Deactivate);
        }
    }
}
