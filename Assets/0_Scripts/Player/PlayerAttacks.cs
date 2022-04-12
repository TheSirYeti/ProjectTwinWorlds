using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerAttacks : MonoBehaviour
{
    public float attackCooldown;
    public float cooldownTimer;
    
    public Observer _playerObserver;
    public Action attackDelegate;

    public float distance, radius;
    public Camera cam;
    public LayerMask wallLayer;
    public bool usedAbility;
    public LayerMask collidableLayer;
    
    
    private void Start()
    {
        attackDelegate = GenerateBasicAttack;
        EventManager.Subscribe("ResetAbility", ThrowAbility);
    }

    private void Update()
    {
        cooldownTimer -= Time.fixedDeltaTime;
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && cooldownTimer <= 0)
        {
            //attackDelegate();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!usedAbility)
            {
                Debug.Log("Tiro");
                CheckAbility();
            }
            else
            {
                //ThrowAbility(null);
                Debug.Log("Saco");
                EventManager.Trigger("ResetAbility");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            ExecuteAbility();
        }
    }

    public abstract void GenerateBasicAttack();

    void StopAttacking()
    {
        _playerObserver.NotifySubscribers("NoAttack");
    }

    public void CheckAbility()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, collidableLayer))
        {
            CheckForCollisions(hit.point);
        }
    }

    public void CheckForCollisions(Vector3 objectPosition)
    {
        Collider[] collisions = Physics.OverlapSphere(objectPosition, radius, collidableLayer);

        if (collisions.Length == 1)
        {
            InteractableObject intObj = collisions[0].GetComponent<InteractableObject>();

            var dirToTarget = objectPosition - transform.position;

            if (LayerMask.NameToLayer(intObj.layerTrigger) == gameObject.layer
                && intObj.CheckForFirstTrigger())
            {
                AimAbility(intObj.insertionPoints[intObj.GetClosestInsertionPoint(transform.position)]);
                intObj.OnObjectStart();
                usedAbility = true;
            }
            else
            {
                if (LayerMask.NameToLayer(intObj.firstTrigger) == gameObject.layer)
                {
                    AimAbility(intObj.insertionPoints[intObj.GetClosestInsertionPoint(transform.position)]);
                    intObj.isFirstTriggered = true;
                    usedAbility = true;
                }
            }
        }
        else usedAbility = false;
    }
    
    public abstract void ThrowAbility(object[] parameters);
    
    public abstract void AimAbility(Transform destination);

    public abstract void ExecuteAbility();
}
