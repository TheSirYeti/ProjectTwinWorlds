using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ISubscriber
{
    public float speed, swingForce;
    public Action<float,float> movementDelegate;
    [SerializeField] private Transform direction;
    public Observer playerObserver;
    public Rigidbody rb, currentSwing;
    public bool canMove;

    public Transform myParent;
    public bool canGrapple;
    public Transform grappable;
    
    private void Start()
    {
        EventManager.Subscribe("OnSwingStart", StartSwing);
        EventManager.Subscribe("OnSwingStop", ResumeMovement);
        EventManager.Subscribe("OnSwingStop", StopSwing);
        EventManager.Subscribe("OnMovableRestrict", RestrictSpeed);
        EventManager.Subscribe("OnMovableUnrestrict", UnrestrictSpeed);
        
        movementDelegate = GenerateMovement;
        playerObserver.Subscribe(this);
    }

    private void FixedUpdate()
    {
        float hMov = Input.GetAxis("Horizontal");
        float vMov = Input.GetAxis("Vertical");

        if (hMov != 0 || vMov != 0)
        {
            movementDelegate(hMov, vMov);
            playerObserver.NotifySubscribers("Walking");
        } else playerObserver.NotifySubscribers("Idle");

        if (Input.GetKeyDown(KeyCode.Space) && canGrapple)
        {
            transform.position = grappable.position;
            grappable = null;
        }
    }

    void GenerateMovement(float h, float v)
    {
        //Vector3 movement = h * direction.right + v * direction.forward;
        Vector3 movement = new Vector3(h, 0, v);
        
        if(movement.magnitude > 1)
            movement.Normalize();
        
        transform.forward = movement;

        if (canMove)
        {
            rb.velocity = new Vector3(movement.x * speed * Time.deltaTime, rb.velocity.y, movement.z * speed * Time.deltaTime);
        }
    }

    void NoMovement(float h, float v)
    {
        
    }

    void SwingMovement(float h, float v)
    {
        if (currentSwing != null)
        {
            Vector3 movement = new Vector3(h, 0, v);
            
            currentSwing.AddForce(movement * swingForce, ForceMode.Acceleration);
        }
    }
    
    
    
    public void OnNotify(string eventID)
    {
        if (eventID == "BasicAttack")
        {
            movementDelegate = NoMovement;
        }

        if (eventID == "NoAttack")
        {
            movementDelegate = GenerateMovement;
        }
    }

    void StopMovement(object[] parameters)
    {
        movementDelegate = NoMovement;
        rb.useGravity = false;
    }

    void ResumeMovement(object[] parameters)
    {
        movementDelegate = GenerateMovement;
        rb.useGravity = true;
    }

    void StartSwing(object[] parameters)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
        currentSwing = (Rigidbody) parameters[0];
        Transform newPos = (Transform) parameters[1];
        transform.position = newPos.position;
        transform.SetParent(newPos);
        movementDelegate = SwingMovement;
    }
    
    void StopSwing(object[] parameters)
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.velocity = currentSwing.velocity;
        currentSwing = null;
        transform.SetParent(myParent);
    }

    private void RestrictSpeed(object[] parameters)
    {
        canMove = false;
    }

    private void UnrestrictSpeed(object[] parameters)
    {
        canMove = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Movable Object"))
        {
            //grappable = other.gameObject.GetComponent<MovableObject>().grappablePoint;
            //canGrapple = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Movable Object"))
        {
            grappable = null;
            canGrapple = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}