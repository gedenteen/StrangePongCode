using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class StorageParameter<T>
{
    public abstract void SetInitialValue(); 
    public abstract void SetNewValue(T newValue);
    public abstract T GetCurrentValue();  
}
