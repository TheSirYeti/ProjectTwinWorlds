using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DemonAttacks : PlayerAttacks
{
    public PentadentCollision weapon;
    public float flightDuration;
    public override void GenerateBasicAttack()
    {
        cooldownTimer = attackCooldown;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
        
        _playerObserver.NotifySubscribers("BasicAttack");
    }

    public override void AimAbility(Transform position)
    {
        transform.LookAt(new Vector3(position.position.x, transform.position.y, position.position.z));
        weapon.transform.position = transform.position;
        weapon.gameObject.SetActive(true);
        weapon.transform.forward = transform.position - position.position;
        weapon.currentDestination = position;
        StartCoroutine(weapon.ThrowPentadent());
    }

    public override void ExecuteAbility()
    {
        var collidedObject = CheckColliders();

        if (collidedObject != null && collidedObject.CheckForFirstTrigger() && LayerMask.NameToLayer(collidedObject.layerTrigger) == gameObject.layer)
        {
            collidedObject.OnObjectEnd();
        }
    }
    
    InteractableObject CheckColliders()
    {
        Collider[] colliders = Physics.OverlapSphere(weapon.transform.position, 1f, collidableLayer);

        if (colliders.Length == 1)
        {
            return colliders[0].GetComponent<InteractableObject>();
        }
        else return null;
    }

    public override void ThrowAbility(object[] parameters)
    {
        weapon.StopCoroutine(weapon.ThrowPentadent());
        weapon.transform.SetParent(null);
        weapon.transform.position = transform.position;
        weapon.gameObject.SetActive(false);
        
        EventManager.Trigger("OnPulleyStop");
        EventManager.Trigger("OnSwingStop");
        usedAbility = false;
    }

    public void AimPentadente()
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
            
            if (LayerMask.NameToLayer(intObj.layerTrigger) == gameObject.layer)
            {
                intObj.OnObjectStart();
            }
            else
            {
                EventManager.Trigger("ResetAbility");
            }
        }
    }
}
