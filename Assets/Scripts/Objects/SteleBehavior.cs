using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class SteleBehavior : MonoBehaviour {

    [SerializeField] private GameObject nextStele;
    [SerializeField] private bool lastStele;
    private PathCreator creator;
    private PathPlatform path;

    [SerializeField] private float pathPointsSpacing = 1f;
    [SerializeField] private float pathResolution = 1f;

    bool isActive;
    int nbTuto = 0;
    int currentTuto = 0;

    void Start() {
        nbTuto = transform.childCount;
        currentTuto = 0;

        if (nextStele) {
            creator = GetComponent<PathCreator>();
            path = creator.path;
        }
    }

    private void Update() {
        if (isActive) {
            if (Input.GetButtonDown("Jump")) {
                transform.GetChild(currentTuto).gameObject.SetActive(false);
                //================================================
                SoundController.instance.SelectHUD("TutoPictureExit");
                //================================================
                if (currentTuto < nbTuto - 1) {
                    currentTuto++;
                    transform.GetChild(currentTuto).gameObject.SetActive(true);
                }
                else {
                    isActive = false;
                    Pause.Paused = false;
                    GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(false);
                    Time.timeScale = 1;
                    nbTuto = 0;
                    //Destroy(gameObject);
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {

            if (nbTuto != 0) {
                other.gameObject.GetComponent<InputController>().SetFreezeInput(true);
                isActive = true;
                transform.GetChild(0).gameObject.SetActive(true);
                //================================================
                SoundController.instance.SelectHUD("PauseOpenClose");
                //================================================
                Pause.Paused = true;
                Time.timeScale = 0;
            }

            HitodamaController hitodama = GameObject.FindGameObjectWithTag("Hitodama").GetComponent<HitodamaController>();
            if (lastStele) {
                hitodama.SetIsGuiding(false);
            }
            else {
                hitodama.SetIsGuiding(true);
                hitodama.SetTargetStele(nextStele);
                Vector3[] points = path.CalculateEvenlySpacedPoints(pathPointsSpacing, pathResolution);
                for (int i = 0; i < points.Length; ++i) {
                    points[i] = transform.TransformDirection(points[i]) + transform.position;
                }
                hitodama.SetPath(points);
            }
        }

    }
}
