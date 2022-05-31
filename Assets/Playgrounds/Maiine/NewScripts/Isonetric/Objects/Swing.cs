using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour, IWeaponInteractable
{
    Player _player;
    GameObject _hangItem;
    BulletSystem _bullet;

    public Vector3 backRigth;
    public Vector3 backLeft;
    Vector3 dir;

    bool isBackRigth = false;
    bool isBackLeft = false;

    public Transform rotationPoint;
    public LineRenderer lineRenderer;

    public bool _usableByDemon;
    bool _isConnect = false;

    delegate void SwingDelegate();
    SwingDelegate actualMove = delegate { };

    void Start()
    {

    }

    void Update()
    {
        actualMove();
    }

    void MoveSwing()
    {
        //if (rotationPoint.localRotation.eulerAngles.x > 5 && rotationPoint.localRotation.eulerAngles.x < 90)
        //{
        //    Debug.Log("rigth");
        //    isBackRigth = true;
        //}
        //else if (rotationPoint.localRotation.eulerAngles.x > 340 && rotationPoint.localRotation.eulerAngles.z < 100)
        //{
        //    Debug.Log("left");
        //    isBackLeft = true;
        //}

        if (isBackRigth)
        {
            Debug.Log("is going rigth");
            rotationPoint.Rotate(backRigth);
        }
        else if (isBackLeft)
        {
            Debug.Log("is going left");
            rotationPoint.Rotate(backLeft);
        }
        else
        {
            rotationPoint.Rotate(dir);
        }

        //Debug.Log(rotationPoint.localRotation.eulerAngles.x + " X");
        //Debug.Log(rotationPoint.localRotation.eulerAngles.z + " Z");
        //if (rotationPoint.localRotation.eulerAngles.x < 280 && rotationPoint.localRotation.eulerAngles.x > 200 && rotationPoint.localRotation.eulerAngles.z > 260 && isBackRigth)
        //{
        //    Debug.Log("cancel rigth");
        //    isBackRigth = false;
        //}
        //else if (rotationPoint.localRotation.eulerAngles.x > 280 && rotationPoint.localRotation.eulerAngles.z > 260 && isBackLeft)
        //{
        //    Debug.Log("cancel left");
        //    isBackLeft = false;
        //}
    }

    public void SetBackState(bool isRigth, bool state)
    {
        if (isRigth)
            isBackRigth = state;
        else
            isBackLeft = state;
    }

    public void CancelMovement()
    {
        isBackRigth = false;
        isBackLeft = false;
    }

    public void SetDir(float horizontal)
    {
        dir = transform.forward * horizontal * -1;
    }

    public void Inter_DoWeaponAction(BulletSystem bullet)
    {
        _isConnect = true;
        _bullet = bullet;
        _player.transform.parent = rotationPoint;
        _player.myMovementController.ChangeToSwing(this);
        _player.myButtonController.ChangeAxies(true);
        actualMove = MoveSwing;
    }

    public void Inter_DoConnectAction(IWeaponInteractable otherObject)
    {
        throw new System.NotImplementedException();
    }

    public void Inter_ResetObject()
    {
        _isConnect = false;
        lineRenderer.enabled = false;
        _player.myMovementController.ChangeToMove();
        _player.myButtonController.ChangeAxies(false);
        _player.transform.parent = null;
        actualMove = delegate { };
    }

    public bool Inter_CheckCanUse(Player actualPlayer, bool isDemon)
    {
        if (_usableByDemon == isDemon)
        {
            _player = actualPlayer;
            return true;
        }
        else
            return false;
    }

    public bool Inter_OnUse()
    {
        return _isConnect;
    }

    public void Inter_SetParent(Transform weapon)
    {
        weapon.parent = rotationPoint;
        weapon.localScale = new Vector3(1, 1, 1);
        weapon.localPosition = Vector3.zero;
    }
}
