using NP.Concepts.Binding.GettersAndSetters;
using NP.Utilities;

namespace NP.Concepts.Binding.PathLinks
{
    public class APropBindingPathLink<PropertyType> :
        BindingPathLinkBase<PropertyType>
    {
        public AProp<object, PropertyType> TheAProp { get; }

        public APropBindingPathLink(AProp<object, PropertyType> aProp) :
            base(PropertyKind.AProperty)
        {
            TheAProp = aProp;
        }

        public override IObjWithPropGetter<PropertyType> CreateGetter() =>
            new APropWithDefaultGetter<PropertyType>(TheAProp, DefaultValue);

        public override IObjWithPropSetter<PropertyType> CreateSetter() =>
            new APropSetter<PropertyType>(TheAProp);

        public override string ToString()
        {
            return "AProp-" + TheAProp.GetType().FullName;
        }

        public override bool Equals(object targetObj)
        {
            if (!base.Equals(targetObj))
                return false;

            APropBindingPathLink<PropertyType> target =
                targetObj as APropBindingPathLink<PropertyType>;

            if (target == null)
                return false;

            return this.TheAProp.ObjEquals(target.TheAProp);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.TheAProp.GetHashCodeExtension();
        }
    }
}
