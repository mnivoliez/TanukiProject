using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour {

    public GameObject koda;
    public PlayerHealth koda_health;
    public KodaController koda_power;

    // Use this for initialization
    void Start () {
        koda = GameObject.FindGameObjectWithTag("Player");
        koda_health = koda.GetComponent<PlayerHealth>();
        koda_power = koda.GetComponent<KodaController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {

        if(other.name == "Player") { 
            float hp_max = koda_health.GetHealthMax();
            float hp_current = koda_health.GetHealthCurrent();

            //playerData.caught_Yokai = koda_power.GetCaughtYokai();

            Transform check_point = koda_power.GetRespawnPointPosition();
            float check_point_x = check_point.position.x;
            float check_point_y = check_point.position.y;
            float check_point_z = check_point.position.z;

            string current_scene = SceneManager.GetActiveScene().name;

            bool power_jump = koda_power.GetPowerJump();
            bool power_ball = koda_power.GetPowerBall();
            bool power_shrink = koda_power.GetPowerShrink();

            Game this_game = new Game();
            this_game.selected_slot = 0;
            this_game.PreSave_Game(hp_max, hp_current, check_point_x, check_point_y, check_point_z, current_scene, power_jump, power_ball, power_shrink);

            Save_Load.savedGames = this_game;
            Save_Load.selected_slot = this_game.selected_slot;

            Save_Load.Save();

            //Debug.Log(Application.persistentDataPath);
        }
    }
}
