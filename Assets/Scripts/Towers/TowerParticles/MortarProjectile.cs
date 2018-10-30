﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MortarProjectile : MonoBehaviour
{
    private float damageRadius = 1f;
    private float damageToDeal;
    private Rigidbody rb;

    public float particleVelocity = 10f;
    public Transform enemyToAttack;
    public Vector3 enemyPos;
    public float floatTime = 5f;
    private float waitTimer;
    Tower.TargetType thisType;
    LayerMask targetableLayers;


    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    public void SetTarget(Collider enemy, Tower.TargetType towerType, LayerMask layer, float DamageToDeal)
    {
        targetableLayers = layer;
        damageToDeal = DamageToDeal;
        enemyToAttack = enemy.gameObject.GetComponent<Transform>();
        
    }




    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Enemy>())
        {
            Damage();
        }
    }

    private void Damage()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(this.transform.position, damageRadius, targetableLayers);
        foreach (Collider c in enemiesInRange)
        {
            if (c.gameObject.GetComponent<Enemy>() as Enemy)
            {
                if (c.gameObject.GetComponent<Enemy>().IsDead == false) // dont want waste attacks on dead
                {

                    c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damageToDeal, Keith.EnemyStats.StatModType.Flat));
                    c.gameObject.GetComponent<Enemy>().updateHealth();
                    print("did some damage");
                }
                else
                {
                    continue;
                }
            }
        }
        Destruct();
    }

    // Update is called once per frame
    void Update()
    {
        waitTimer += Time.deltaTime;
        if (enemyToAttack != null && waitTimer > floatTime)
        {
            enemyPos = enemyToAttack.position;
            Vector3 targetVec = enemyPos - this.transform.position;
            transform.LookAt(targetVec);
            targetVec = targetVec.normalized;
            rb.velocity = targetVec * Time.deltaTime * particleVelocity;
        }
        else
        {
            enemyPos = enemyToAttack.position;
            Vector3 targetVec = enemyPos - this.transform.position;
            transform.LookAt(targetVec);
            targetVec = targetVec.normalized;
            rb.velocity = new Vector3(targetVec.x , targetVec.y + 1, targetVec.z ) * Time.deltaTime * particleVelocity;
        }
        if(enemyToAttack == null)
        {
            Destruct();
        }

    }

    private void Destruct()
    {
        Destroy(gameObject);
    }
}