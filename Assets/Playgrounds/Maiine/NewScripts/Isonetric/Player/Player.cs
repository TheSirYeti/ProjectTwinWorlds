using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public bool isActive;
    public bool isDemon;
    public int actualRoom;

    public Projectile pentadent;
    public Projectile arrow;

    public GameObject realCharacter;
    public GameObject totemCharacter;

    CapsuleCollider myCollider;
    Rigidbody myRigidbody;

    IPlayerInteractable _playerInteractable = null;

    public LayerMask usableItems;

    ShootingController myShootingController;
    MovementController myMovementController;
    ButtonsController myButtonController;
    public CameraController cameraController;

    public LayerMask collisionMask;

    private void Start()
    {
        myCollider = GetComponent<CapsuleCollider>();
        myRigidbody = GetComponent<Rigidbody>();

        if (isDemon)
        {
            Projectile actualPentadent = Instantiate(pentadent, new Vector3(0, -50, 0), Quaternion.Euler(Vector3.zero));
            actualPentadent.SetPlayer(this);
            myShootingController = new ShootingController(actualPentadent, this, usableItems, isDemon);
        }
        else
        {
            Projectile actualArrow = Instantiate(arrow, new Vector3(0, -50, 0), Quaternion.Euler(Vector3.zero));
            actualArrow.SetPlayer(this);
            myShootingController = new ShootingController(actualArrow, this, usableItems, isDemon);
        }

        myMovementController = new MovementController(transform, myRigidbody, speed);
        myButtonController = new ButtonsController(this, myMovementController, cameraController, myShootingController, collisionMask);

        EventManager.Subscribe("ChangePlayer", ChangeCharacter);

        if (isActive)
        {
            StartCoroutine(CorrutinaTurnOn());
        }
        else
        {
            TurnOff();
        }
    }

    void Update()
    {
        myButtonController.actualButtons();
        myMovementController.actualMovement();
    }


    public void CheckInteractable()
    {
        if (_playerInteractable == null) return;

        _playerInteractable.DoPlayerAction(this, isDemon);
    }

    #region Player Change
    public void ChangeCharacter(params object[] parameter)
    {
        if (isActive)
        {
            TurnOff();
        }
        else
        {
            StartCoroutine(CorrutinaTurnOn());
        }

        isActive = !isActive;
    }

    void TurnOff()
    {
        myMovementController.ChangeToStay();
        myButtonController.ButtonsOff();
        realCharacter.SetActive(false);
        totemCharacter.SetActive(true);
        myCollider.isTrigger = true;
        myRigidbody.useGravity = false;
    }

    IEnumerator CorrutinaTurnOn()
    {
        yield return new WaitForEndOfFrame();
        myMovementController.ChangeToMove();
        myButtonController.ButtonsOn();
        realCharacter.SetActive(true);
        totemCharacter.SetActive(false);
        myCollider.isTrigger = false;
        myRigidbody.useGravity = true;
    }
    #endregion

    public void SetOffDelegates()
    {
        myMovementController.ChangeToStay();
        myButtonController.ButtonsOff();
    }

    public void SetOnDelegates()
    {
        myMovementController.ChangeToMove();
        myButtonController.ButtonsOn();
    }

    public void SetActualRoom(int nextRoom)
    {
        actualRoom = nextRoom;
    }

    public int GetActualRoom()
    {
        return actualRoom;
    }

    public void GoToTransform(Vector3 actualTransform)
    {
        transform.position = actualTransform;
    }

    private void OnTriggerEnter(Collider other)
    {
        IPlayerInteractable actualPlayerInteractable = other.gameObject.GetComponent<IPlayerInteractable>();
        if (actualPlayerInteractable != null)
        {
            _playerInteractable = actualPlayerInteractable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IPlayerInteractable actualPlayerInteractable = other.gameObject.GetComponent<IPlayerInteractable>();
        if (actualPlayerInteractable != null)
            _playerInteractable = null;
    }

}
