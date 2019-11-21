using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IButtonSignalHandler : IEventSystemHandler
{
    void CloseButton();
    void OpenButton();
}
