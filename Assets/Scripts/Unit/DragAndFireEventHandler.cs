using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IDragAndFireEventHandler : IEventSystemHandler
{
    void TurnOnDrag();
    void TurnOffDrag();
}
