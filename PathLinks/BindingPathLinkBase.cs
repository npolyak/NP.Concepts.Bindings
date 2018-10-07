using NP.Concepts.Binding.GettersAndSetters;

namespace NP.Concepts.Binding.PathLinks
{
    public abstract class BindingPathLinkBase<PropertyType> : PropKindContainer, IBindingPathLink<PropertyType>
    {
        public PropertyType DefaultValue
        {
            get;
        }

        public BindingPathLinkBase(PropertyKind propKind) : base(propKind)
        {
            DefaultValue = default(PropertyType);
        }


        public abstract IObjWithPropGetter<PropertyType> CreateGetter();

        public abstract IObjWithPropSetter<PropertyType> CreateSetter();
    }
}
