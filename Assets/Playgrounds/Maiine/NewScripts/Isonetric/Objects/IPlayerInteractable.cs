using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInteractable
{
    public void Inter_DoPlayerAction(Player actualPlayer, bool isDemon);
    public void Inter_DoJumpAction(Player actualPlayer, bool isDemon);
}
