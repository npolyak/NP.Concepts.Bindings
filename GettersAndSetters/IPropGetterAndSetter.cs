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
    public interface IPropGetter<PropertyType>
    {
        // fires when the property changes
        event Action PropertyChangedEvent;

        PropertyType GetPropValue();

        // forces PropertyChangedEvent to fire
        // (it is needed e.g. when when two properties
        // are bound - the source property should 
        // trigger the target property change even
        // if the source property does not change)
        void TriggerPropertyChanged();
    }

    public interface IObjectContainer
    {
        object TheObj
        {
            get;
            set;
        }
    }

    public interface IObjWithPropGetter<PropertyType> : IObjectContainer, IPropGetter<PropertyType>
    {
        bool HasObj
        {
            get;
        }
    }

    public interface IPropSetter<PropertyType>
    {
        // sets the target property
        void Set(PropertyType propValue);
    }

    public interface IObjWithPropSetter<PropertyType> : IObjectContainer, IPropSetter<PropertyType>
    {

    }

    public static class SetterGetterUtils
    {
        public static void SetterFromGetter<SetterPropType, GetterPropType>
        (
            this IPropSetter<SetterPropType> setter,
            IPropGetter<GetterPropType> getter,
            Func<GetterPropType, SetterPropType> converter)
        {
            setter.Set(converter( getter.GetPropValue()));
        }

        public static void SetterFromGetter<PropType>
        (
            this IPropSetter<PropType> setter, 
            IPropGetter<PropType> getter)
        {
            setter.SetterFromGetter(getter, (val) => val);
        }
    }
}
