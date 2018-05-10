using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCredits : MonoBehaviour {

	[SerializeField] private float scrollSpeed = 1f;

    private Canvas CanvasCredits;

    private GameObject CreditPanel;
    private GameObject CreditText;
    private GameObject ExitPanel;

    // Use this for initialization
    void Start () {
        CanvasCredits = GetComponent<Canvas>();

        CreditPanel = CanvasCredits.transform.GetChild(0).gameObject;
        CreditPanel.SetActive(true);

        CreditText = CreditPanel.transform.GetChild(0).gameObject;

        ExitPanel = CanvasCredits.transform.GetChild(1).gameObject;
        ExitPanel.SetActive(true);	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        CreditText.transform.position += Vector3.up * Time.fixedDeltaTime * scrollSpeed;
	}
}
