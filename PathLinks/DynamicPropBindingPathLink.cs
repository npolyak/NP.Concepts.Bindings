using NP.Concepts.Binding.GettersAndSetters;

namespace NP.Concepts.Binding.PathLinks
{
    public class DynamicPropBindingPathLink<PropertyType> : NameBasedBindingPathLink<PropertyType>
    {
        public DynamicPropBindingPathLink(string propName) :
            base(propName, PropertyKind.Map)
        {
        }

        public override IObjWithPropGetter<PropertyType> CreateGetter() =>
            new DynamicPropWithDefaultGetter<PropertyType>(PropertyName);

        public override IObjWithPropSetter<PropertyType> CreateSetter() =>
            new DynamicPropertySetter<PropertyType>(PropertyName);
    }
}
