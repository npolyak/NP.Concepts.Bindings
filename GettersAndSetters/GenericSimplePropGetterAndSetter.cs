// (c) Nick Polyak 2013 - http://awebpros.com/
// License: Code Project Open License (CPOL) 1.92(http://www.codeproject.com/info/cpol10.aspx)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also as a courtesy, please, mention this software in any documentation for the 
// products that use it.

using System;

namespace NP.Concepts.Binding.GettersAndSetters
{
    public abstract class GenericSimplePropWithDefaultGetter<PropertyType> : 
        IObjWithPropGetter<PropertyType>
    {
        public PropertyType DefaultValue { get; private set; }

        protected abstract void OnObjUnset();

        protected abstract void OnObjSet();

        object _obj;
        public object TheObj
        {
            get
            {
                return _obj;
            }

            set
            {
                if (_obj == value)
                    return;

                if (HasObj) // clear the old object
                {
                    OnObjUnset();
                }

                _obj = value;

                if (HasObj)
                {
                    OnObjSet();
                }

                TriggerPropertyChanged();
            }
        }

        public bool HasObj
        {
            get
            {
                return _obj != null;
            }
        }

        public GenericSimplePropWithDefaultGetter
        (
            PropertyType defaultValue = default(PropertyType)
        )
        {
            DefaultValue = defaultValue;
        }

        ~GenericSimplePropWithDefaultGetter()
        {
            //this.TheObj = null;
        }

        public event Action PropertyChangedEvent;

        public abstract PropertyType GetPropValue();

        public void TriggerPropertyChanged()
        {
            PropertyChangedEvent?.Invoke();
        }
    }


    public abstract class GenericSimplePropSetter<PropertyType> :
        IObjWithPropSetter<PropertyType>
    {
        ValueHolder<PropertyType> _lastValueHolder = new ValueHolder<PropertyType>();

        protected virtual void OnObjUnset() {}

        protected abstract void OnObjSet();

        protected abstract void SetPropValue(PropertyType propValue);

        object _obj;
        public object TheObj
        {
            get
            {
                return _obj;
            }
            set
            {
                if (Object.ReferenceEquals(_obj, value))
                    return;

                OnObjUnset();

                _obj = value;

                OnObjSet();

                if (_lastValueHolder.HasBeenSet)
                    Set(_lastValueHolder.TheValue);
            }
        }

        public GenericSimplePropSetter(object obj = null) 
        {
            TheObj = obj;
        }

        public void Set(PropertyType propValue)
        {
            try
            {
                if (_obj == null)
                    return;

                SetPropValue(propValue);
            }
            finally
            {
                _lastValueHolder.TheValue = propValue;
            }
        }
    }
}
