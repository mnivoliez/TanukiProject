using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternStandController : MonoBehaviour
{
    [SerializeField]
    private LanternController lantern;

    public void RecallLantern()
    {
        lantern.Respawn();
    }
}
