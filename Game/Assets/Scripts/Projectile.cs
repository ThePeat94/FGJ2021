using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 ShootDirection { get; set; }

    private void Awake()
    {
        Destroy(this.gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.ShootDirection * Time.deltaTime * 20f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out HealthController playerHealthController)) return;
        playerHealthController.TakeDamage(1);
        
        Destroy(this.gameObject);
    }
}
