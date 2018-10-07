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
    public class APropGetter<PropertyType> : IPropGetter<PropertyType>
    {
        object _obj;
        AProp<object, PropertyType> _aProp;
        public APropGetter(object obj, AProp<object, PropertyType> aProp)
        {
            _obj = obj;
            _aProp = aProp;

            _aProp.AddOnPropertyChangedHandler(_obj, OnPropChanged);
        }

        ~APropGetter()
        {
            _aProp.RemoveOnPropertyChangedHandler(_obj, OnPropChanged);
        }

        void OnPropChanged(object obj, PropertyType oldValue, PropertyType newValue)
        {
            TriggerPropertyChanged();
        }

        public event Action PropertyChangedEvent;

        public PropertyType GetPropValue() => _aProp.GetProperty(_obj);

        public void TriggerPropertyChanged()
        {
            PropertyChangedEvent?.Invoke();
        }
    }

    public class APropWithDefaultGetter<PropertyType> : 
        GenericSimplePropWithDefaultGetter<PropertyType>
    {
        AProp<object, PropertyType> _aProp;

        protected override void OnObjUnset()
        {
            _aProp.RemoveOnPropertyChangedHandler(TheObj, OnPropChanged);
        }

        protected override void OnObjSet()
        {
            _aProp.AddOnPropertyChangedHandler(TheObj, OnPropChanged);
        }

        public APropWithDefaultGetter
        (
            AProp<object, PropertyType> aProp, 
            PropertyType defaultValue = default(PropertyType) 
        )
            : base(defaultValue)
        {
            _aProp = aProp;
        }

        void OnPropChanged(object obj, PropertyType oldValue, PropertyType newValue)
        {
            TriggerPropertyChanged();
        }

        public override PropertyType GetPropValue()
        {
            return _aProp.GetProperty(TheObj);
        }
    }

    public class APropSetter<PropertyType> :
        GenericSimplePropSetter<PropertyType>
    {
        protected override void OnObjSet()
        {
        }

        protected override void SetPropValue(PropertyType propValue)
        {
            _aProp.SetProperty(TheObj, propValue);
        }

        AProp<object, PropertyType> _aProp;

        void Init(AProp<object, PropertyType> aProp)
        {
            _aProp = aProp;
        }

        public APropSetter(AProp<object, PropertyType> aProp) : base()
        {
            Init(aProp);
        }

        public APropSetter(object obj, AProp<object, PropertyType> aProp) : base(obj)
        {
            Init(aProp);
        }
    }
}
