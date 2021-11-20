using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ModifiedEvent();

[Serializable]
public class ModifiableInt
{
    [SerializeField] private int _baseValue;
    public int BaseValue
    {
        get { return _baseValue; }
        set { _baseValue = value; }
    }

    [SerializeField] private int _modifiedValue;
    public int ModifiedValue
    {
        get { return _modifiedValue; }
        private set { _modifiedValue = value; }
    }

    public List<IModifier> modifiers = new List<IModifier>();
    public event ModifiedEvent ValueModified;

    public ModifiableInt(ModifiedEvent method = null)
    {
        _modifiedValue = BaseValue;
        if (method != null)
            ValueModified += method;
    }

    public void RegisterModEvent(ModifiedEvent method)
    {
        ValueModified += method;
    }
    public void UnregisterModEvent(ModifiedEvent method)
    {
        ValueModified -= method;
    }

    public void UpdateModifiedValue()
    {
        var valueToAdd = 0;
        for (int i = 0; i < modifiers.Count; i++)
        {
            modifiers[i].AddValue(ref valueToAdd);
        }
        ModifiedValue = _baseValue + valueToAdd;
        
        ValueModified?.Invoke();
    }

    public void AddModifier(IModifier _modifier)
    {
        modifiers.Add(_modifier);
        UpdateModifiedValue();
    }

    public void RemoveModifier(IModifier _modifier)
    {
        modifiers.Remove(_modifier);
        UpdateModifiedValue();
    }
}
