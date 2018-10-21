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
using NP.Concepts.Binding.Extensions;
using NP.Utilities;
using System;
using System.Collections;
using System.ComponentModel;

namespace NP.Concepts.Binding.GettersAndSetters
{
    public abstract class NotifiedPropWithDefaultGetter<PropertyType> : 
        GenericSimplePropWithDefaultGetter<PropertyType>
    {
        string _propName;
        Func<object, object> _propertyGetter = null;

        public override int GetHashCode()
        {
            return _propName.GetHashCodeExtension();
        }

        INotifyPropertyChanged NotifyingObj
        {
            get
            {
                return TheObj as INotifyPropertyChanged;
            }
        }

        protected override void OnObjUnset()
        {
           // _propertyGetter = null;

            if (NotifyingObj != null)
            {
                NotifyingObj.PropertyChanged -= obj_PropertyChanged;
            }
        }

        protected abstract Func<object, object> GetPropGetter(string propName);

        protected override void OnObjSet()
        {
            if (NotifyingObj != null)
                NotifyingObj.PropertyChanged += obj_PropertyChanged;
        }

        public NotifiedPropWithDefaultGetter
        (
            string propName,
            PropertyType defaultValue = default(PropertyType)
        ) : base(defaultValue)
        {
            _propName = propName;

            SetGetter();
        }

        protected void SetGetter()
        {
            if (_propertyGetter != null)
                return;

            _propertyGetter = GetPropGetter(_propName);
        }

        void obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _propName)
                return;

            TriggerPropertyChanged();
        }

        public override PropertyType GetPropValue()
        {
            if ( (_propertyGetter == null) || (TheObj == null) )
            {
                return DefaultValue;
            }

            return (PropertyType)_propertyGetter(TheObj);
        }
    }

    public class PlainPropWithDefaultGetter<PropertyType> : NotifiedPropWithDefaultGetter<PropertyType>
    {
        public Type TheObjType { get; private set; } = null;

        protected override void OnObjSet()
        {
            base.OnObjSet();

            if (this.TheObjType == null)
            {
                this.TheObjType = this.TheObj?.GetType();

                SetGetter();
            }
        }

        public PlainPropWithDefaultGetter
        (
            string propName,
            Type objType,
            PropertyType defaultValue = default(PropertyType)
        )
            : base(propName, defaultValue)
        {
            TheObjType = objType;

            SetGetter();
        }

        protected override Func<object, object> GetPropGetter(string propName)
        {
            if (TheObjType == null)
                return null;

            Func<object, object> result =
                CompiledExpressionUtils.GetUntypedCSPropertyGetterByObjType(TheObjType, propName);

            return result;
        }
    }

    public class DynamicPropWithDefaultGetter<PropertyType> : NotifiedPropWithDefaultGetter<PropertyType>
    {
        public DynamicPropWithDefaultGetter
        (
            string propName,
            PropertyType defaultValue = default(PropertyType)
        )
            : base(propName, defaultValue)
        {

        }
        protected override Func<object, object> GetPropGetter(string propName)
        {
            Func<object, object> result = (theObj) =>
                {
                    IDictionary mapPropContainer = theObj as IDictionary;

                    return mapPropContainer[propName];
                };

            return result;
        }
    }

    public abstract class PlainPropSetterBase<PropertyType> : 
        GenericSimplePropSetter<PropertyType>
    { 
        Action<object, object> _propertySetter = null;
        string _propName;

        protected override void OnObjSet()
        {
        }

        protected abstract Action<object, object> GetPropSetter(string propName);

        public PlainPropSetterBase(string propName) : base()
        {
            Init(propName);
        }

        public PlainPropSetterBase(object obj, string propName)
            : base(obj)
        {
            Init(propName);

            SetSetter();
        }

        protected void SetSetter()
        {
            _propertySetter = GetPropSetter(_propName);
        }

        protected override void SetPropValue(PropertyType propValue)
        {
            _propertySetter(TheObj, propValue);
        }

        void Init(string propName)
        {
            _propName = propName;
        }

    }

    public class PlainPropertySetter<PropertyType> : PlainPropSetterBase<PropertyType>
    {
        public Type TheObjType { get; private set; }

        protected override Action<object, object> GetPropSetter(string propName)
        {
            if (this.TheObjType == null)
                return null;

            Action<object, object> result = CompiledExpressionUtils.GetUntypedCSPropertySetterByObjType(TheObjType, propName);

            return result;
        }

        protected override void OnObjSet()
        {
            base.OnObjSet();

            if (TheObjType == null)
            {
                TheObjType = TheObj?.GetType();

                SetSetter();
            }
        }

        void SetObjType(Type objType)
        {
            this.TheObjType = objType;

            SetSetter();
        }

        public PlainPropertySetter(string propName, Type objType)
            : base(propName)
        {
            SetObjType(objType);
        }

        public PlainPropertySetter(object obj, string propName, Type objType) : base(obj, propName)
        {
            SetObjType(objType);
        }
    }

    public class DynamicPropertySetter<PropertyType> : PlainPropSetterBase<PropertyType>
    {
        protected override Action<object, object> GetPropSetter(string propName)
        {
            Action<object, object> result =
                (theObj, propValue) =>
                {
                    IDictionary map = theObj as IDictionary;

                    map[propName] = propValue;
                };

            return result;
        }

        public DynamicPropertySetter(string propName)
            : base(propName)
        {
        }

        public DynamicPropertySetter(object obj, string propName)
            : base(obj, propName)
        {
        }
    }
}
