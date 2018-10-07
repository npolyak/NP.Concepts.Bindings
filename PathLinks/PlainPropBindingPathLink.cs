using NP.Concepts.Binding.GettersAndSetters;
using System;

namespace NP.Concepts.Binding.PathLinks
{
    public class PlainPropBindingPathLink<PropertyType> :
        NameBasedBindingPathLink<PropertyType>
    {
        public Type TheObjType { get; private set; }

        public PlainPropBindingPathLink(string propName, Type objType = null) :
            base(propName, PropertyKind.Plain)
        {
            this.TheObjType = objType;
        }

        public override IObjWithPropGetter<PropertyType> CreateGetter() =>
           new PlainPropWithDefaultGetter<PropertyType>(PropertyName, TheObjType);
           

        public override IObjWithPropSetter<PropertyType> CreateSetter() =>
            new PlainPropertySetter<PropertyType>(PropertyName, TheObjType);
    }
}
