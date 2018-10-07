using NP.Utilities;
using System;
using System.Runtime.CompilerServices;

namespace NP.Concepts.Binding
{
    public interface IAPropValueGetter
    {
        object GetObjectAPropValue(object obj);
    }

    public delegate bool BeforePropertyChangedDelegate<ObjectType, PropertyType>
    (
        ObjectType obj,
        PropertyType oldPropertyValue,
        PropertyType newPropertyValue
    );


    public delegate void OnPropertyChangedDelegate<ObjectType, PropertyType>
    (
        ObjectType obj,
        PropertyType oldPropertyValue,
        PropertyType newPropertyValue
    );

    // class for storying the object's AProperty data 
    class APropertyValueWrapper<ObjectType, PropertyType> where ObjectType : class
    {
        // The object on which the AProperty is set
        internal WeakReference<ObjectType> ObjReference { get; private set; }

        internal ObjectType Obj
        {
            set
            {
                ObjReference = new WeakReference<ObjectType>(value);
            }
        }

        // the AProperty Value for the object Obj
        internal PropertyType APropertyValue { get; set; }

        // allows to set event handlers to be fired one the AProperty is changed on the object
        internal event OnPropertyChangedDelegate<ObjectType, PropertyType> OnPropertyChangedEvent = null;
        internal void FireOnPropertyChangedEvent(PropertyType oldPropertyValue)
        {
            if (OnPropertyChangedEvent != null)
            {
                ObjectType obj;

                if (ObjReference.TryGetTarget(out obj))
                {
                    OnPropertyChangedEvent(obj, oldPropertyValue, APropertyValue);
                }
            }
        }

        // clears the event handlers
        internal void ClearOnPropertyChangedEvent()
        {
            OnPropertyChangedEvent = null;
        }
    }

    // this is a class whose objects that uniquely defines an AProperty (non-WPF Attached Property)
    // it is used in order to set and access the property
    public class AProp<ObjectType, PropertyType> : IAPropValueGetter
        where ObjectType : class
    {
        // Default values
        PropertyType _defaultValue = default(PropertyType);

        ConditionalWeakTable<ObjectType, APropertyValueWrapper<ObjectType, PropertyType>> _objectToPropValueMap =
            new ConditionalWeakTable<ObjectType, APropertyValueWrapper<ObjectType, PropertyType>>();

        // fires before the property change, allows cancelling the propery
        // change (if returns false)
        BeforePropertyChangedDelegate<ObjectType, PropertyType> BeforePropertyChangedFn = null;

        // fires after the property change
        // two last arguments specify old and new proproperty values
        OnPropertyChangedDelegate<ObjectType, PropertyType> OnPropertyChangedFn = null;

        // Constructor - takes default value,
        // and two callbacks as arguments.
        // the first callback fires before a property is changed on an object
        // the second - after
        // Provides default values for all its arguments
        public AProp
        (
            PropertyType defaultValue = default(PropertyType),
            BeforePropertyChangedDelegate<ObjectType, PropertyType> beforePropertyChangedFn = null,
            OnPropertyChangedDelegate<ObjectType, PropertyType> onPropertyChangedFn = null
        )
        {
            _defaultValue = defaultValue;
            BeforePropertyChangedFn = beforePropertyChangedFn;
            OnPropertyChangedFn = onPropertyChangedFn;
        }


        PropertyType GetCurrentValueFromWrapper(APropertyValueWrapper<ObjectType, PropertyType> valueWrapper)
        {
            if (valueWrapper == null)
                return _defaultValue;

            return valueWrapper.APropertyValue;
        }

        // given an object returns the ValueWrapper corresponding to the AProperty value
        // of this object. If it is null and createIfDoesNotExist flag set to true,
        // it will create the ValueWrapper with default value.
        APropertyValueWrapper<ObjectType, PropertyType> GetValueWrapper(ObjectType obj, bool createIfDoesNotExist = false)
        {
            APropertyValueWrapper<ObjectType, PropertyType> valueWrapper = null;

            if (!_objectToPropValueMap.TryGetValue(obj, out valueWrapper))
            {
                if (createIfDoesNotExist)
                {
                    if (valueWrapper == null)
                    {
                        valueWrapper = new APropertyValueWrapper<ObjectType, PropertyType>
                        {
                            Obj = obj,
                            APropertyValue = _defaultValue
                        };

                        _objectToPropValueMap.Add(obj, valueWrapper);
                    }
                }
            }

            return valueWrapper;
        }

        // adds an individual property change handler
        // on the passed object. (other object won't get affected)
        public void AddOnPropertyChangedHandler
        (
            ObjectType obj,
            OnPropertyChangedDelegate<ObjectType, PropertyType> propChangedHandler
        )
        {
            if (propChangedHandler == null)
                return;

            APropertyValueWrapper<ObjectType, PropertyType> valueWrapper =
                GetValueWrapper(obj, true);

            valueWrapper.OnPropertyChangedEvent += propChangedHandler;
        }

        // removes the individual change handler
        // from a passed object.
        public void RemoveOnPropertyChangedHandler
        (
            ObjectType obj,
            OnPropertyChangedDelegate<ObjectType, PropertyType> propChangedHandler
        )
        {
            APropertyValueWrapper<ObjectType, PropertyType> valueWrapper =
                GetValueWrapper(obj, true);

            valueWrapper.OnPropertyChangedEvent -= propChangedHandler;
        }

        // if shouldRemove flag is true, newPropertyValue should be default
        void SetOrRemoveProperty(ObjectType obj, PropertyType newPropertyValue, bool shouldRemove = false)
        {
            APropertyValueWrapper<ObjectType, PropertyType> valueWrapper =
                GetValueWrapper(obj);

            PropertyType oldPropertyValue = GetCurrentValueFromWrapper(valueWrapper);

            if (oldPropertyValue.ObjEquals(newPropertyValue))
            {
                // do not do or any event firing if the property did not change
                // this is important to prevent circlular updates 
                // in case of the bindings.
                return;
            }

            // check the global BeforePropertyChanged Event
            if (BeforePropertyChangedFn != null)
            {
                // if returned false, do not proceed
                if (!BeforePropertyChangedFn(obj, oldPropertyValue, newPropertyValue))
                    return;
            }

            if (!shouldRemove)
            {
                if (valueWrapper == null)
                {
                    valueWrapper = new APropertyValueWrapper<ObjectType, PropertyType>
                    {
                        Obj = obj
                    };
                    _objectToPropValueMap.Add(obj, valueWrapper);
                }

                valueWrapper.APropertyValue = newPropertyValue;
            }
            else
            {
                _objectToPropValueMap.Remove(obj);
            }

            valueWrapper.FireOnPropertyChangedEvent(oldPropertyValue);

            if (shouldRemove)
            {
                valueWrapper.ClearOnPropertyChangedEvent();
            }

            OnPropertyChangedFn?.Invoke(obj, oldPropertyValue, newPropertyValue);
        }

        // sets the AProperty on the passed object
        public void SetProperty(ObjectType obj, PropertyType newPropertyValue)
        {
            SetOrRemoveProperty(obj, newPropertyValue);
        }

        // clears AProperty from the passed object.
        public void ClearAProperty(ObjectType obj)
        {
            APropertyValueWrapper<ObjectType, PropertyType> valueWrapper =
                GetValueWrapper(obj);

            if (valueWrapper == null)
                return;

            SetOrRemoveProperty(obj, _defaultValue, true);
            //_objectToPropValueMap.Remove(obj);
        }

        // returns the AProperty value for the passed object.
        public PropertyType GetProperty(ObjectType obj)
        {
            APropertyValueWrapper<ObjectType, PropertyType> valueWrapper =
                GetValueWrapper(obj);

            return GetCurrentValueFromWrapper(valueWrapper);
        }

        public object GetObjectAPropValue(object obj)
        {
            return GetProperty((ObjectType)obj);
        }
    }

    // non-generic version of AProperty class - fits for any object and property types
    public class AProp : AProp<object, object>
    {
        // uses null as the default value
        public AProp()
        {
        }

        public AProp(object defaultValue) : base(defaultValue) { }

        public AProp(
            object defaultValue = default(object),
            BeforePropertyChangedDelegate<object, object> beforePropertyChangedFn = null,
            OnPropertyChangedDelegate<object, object> onPropertyChangedFn = null)
            :
            base(defaultValue, beforePropertyChangedFn, onPropertyChangedFn)
        {

        }
    }
}
