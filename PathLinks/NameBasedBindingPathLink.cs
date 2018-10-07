using NP.Utilities;

namespace NP.Concepts.Binding.PathLinks
{
    public abstract class NameBasedBindingPathLink<PropertyType>
        : BindingPathLinkBase<PropertyType>
    {
        public string PropertyName { get; }

        public NameBasedBindingPathLink
        (
            string propName,
            PropertyKind propKind 
        ) : base(propKind)
        {
            PropertyName = propName;
        }

        public override string ToString() => PropertyName;

        public override bool Equals(object targetObj)
        {
            if (!base.Equals(targetObj))
                return false;

            NameBasedBindingPathLink<PropertyType> target =
                targetObj as NameBasedBindingPathLink<PropertyType>;

            if (target == null)
                return false;

            return target.PropertyName == this.PropertyName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ 
                this.PropertyName.GetHashCodeExtension();
        }
    }
}
