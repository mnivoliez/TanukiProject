using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternStandController : MonoBehaviour
{

    private Vector3 origin_pos;
    private Quaternion origin_rot;
    [SerializeField]
    private LanternController lantern;

    void Awake()
    {
        origin_pos = lantern.transform.position;
        origin_rot = lantern.transform.rotation;
    }

    public void RecallLantern()
    {
        lantern.transform.position = origin_pos;
        lantern.transform.rotation = origin_rot;
    }
}
