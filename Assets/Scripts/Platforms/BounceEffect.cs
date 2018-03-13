using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : MonoBehaviour {

    [SerializeField] private int coeffRebond = 200;
    [SerializeField] private AudioClip bumperSound;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Rigidbody Koda = collision.gameObject.GetComponent<Rigidbody>();
            //Vector3 Koda_old_velocity = Koda.velocity;
            ContactPoint contact = collision.contacts[0];
            Vector3 temp_orientation = -collision.contacts[0].normal.normalized;
            temp_orientation.x = 0;
            temp_orientation.y = temp_orientation.y / Mathf.Abs(temp_orientation.y);
            temp_orientation.z = 0;
            try {
                SoundController.instance.PlaySingle(bumperSound);
            }
            catch {
            }
            Koda.AddForce(temp_orientation * coeffRebond, ForceMode.Impulse); // 200 = coeffRebond
        }
        else if (collision.gameObject.CompareTag("Lure")) {
            Rigidbody Lure = collision.gameObject.GetComponent<Rigidbody>();
            Lure.velocity = Vector3.zero;
            ContactPoint contact = collision.contacts[0];

            Lure.AddForce(Vector3.up * 5 * 999 + Vector3.forward * 5 * 999 * Random.Range(-1.0f, 1.0f) + Vector3.right * 5 * 999 * Random.Range(-1.0f, 1.0f), ForceMode.Impulse); // 200 = coeffRebond
        }
        else {
            Rigidbody other = collision.gameObject.GetComponent<Rigidbody>();
            other.velocity = Vector3.zero;
            ContactPoint contact = collision.contacts[0];

            other.AddForce(Vector3.up * 5 + Vector3.forward * 5 * Random.Range(-1.0f, 1.0f) + Vector3.right * 5 * Random.Range(-1.0f, 1.0f), ForceMode.Impulse); // 200 = coeffRebond
        }

        //Debug.Log("BOING");
    }

    /*private void OnCollisionExit(Collision collision) {
        Rigidbody Koda = collision.gameObject.GetComponent<Rigidbody>();
        Koda.velocity += Vector3.up * coeffRebond;
    }*/
}