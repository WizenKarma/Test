﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AoETower : InGameTower
{
    public enum EffectType
    {
        Damage = 200,
        // other AoE effects?
    };

    public EffectType thisEffect;
    private float timerVar;
    SphereCollider rangeSphere;
    public bool enemyIsInRange;
    private Transform barrelEnd;

    public ParticleSystem directParticleSystem;
    public GameObject bulletPref;

    // Use this for initialization
    void Start ()
    {
        barrelEnd = transform.Find("BarrelEnd");
        rangeSphere = GetComponent<SphereCollider>();
        rangeSphere.isTrigger = true;
        rangeSphere.radius = this.range.Value;
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (enemyIsInRange)
        {
            RotateToTarget();
            timerVar += Time.deltaTime;
            if (timerVar > fireRate.Value)
            {
                ApplyAoE();
                timerVar = 0f;
            }
        }
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyIsInRange = false;
        }
    }

    

    

    // could this be under InGameTowers as an AuxFn for say, a DOT AOE tower or a SLOW AOE tower?
    public void ApplyAoE()
    {
        Collider[] inRange = Physics.OverlapSphere(this.transform.position, range.Value, targetableLayers);
        Collider[] targets = this.Targets(this.transform, this.targettype, inRange, 1);

        // error checking here
        Transform targetTransform = TransformOfTarget(targets);
        

        // might need a separate AoE range around the target?

        //Collider[] targetsToAoE = Physics.OverlapSphere(targetTransform.position, range.Value, targetableLayers);

        if (thisEffect == EffectType.Damage)
        {
            foreach (Collider c in inRange)
            {
                if (c.gameObject.GetComponent<Enemy>() as Enemy)
                {
                    ParticleSystem muzzleFlash = Instantiate(directParticleSystem, barrelEnd.position, Quaternion.identity) as ParticleSystem;
                    GameObject bullet = Instantiate(bulletPref, barrelEnd.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<ProjectileParticle>().setParms(c, 100f);

                    c.gameObject.GetComponent<Enemy>().Health.AddModifier(new Keith.EnemyStats.StatModifier(-damage.Value, Keith.EnemyStats.StatModType.Flat));
                    c.gameObject.GetComponent<Enemy>().updateHealth();
                    print("did AoE damage");
                }
            }
        }
    }
}
