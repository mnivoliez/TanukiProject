using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Tells Unity that this script can be serialized—in other words, that we can save all the variables in this script. MUST BE IN ALL SCRIPTS THAT NEEDS TO BE SAVED !
public class Game {

    public static Game current;
    //public Zone_Level = Zone; // Need to know the current zone the player is. We need a Zone script that handles the state of the zone, the checkpoints, etc ...
    //public Character Koda; // Need to know the state Koda is in : His unlocked powers, HP remaining, current checkpoint unlocked, the amount of Yokai killed, ... Script required !

    public Game() {
        //Zone = new Zone_Level();
        //Koda = new Character();
    }

}
