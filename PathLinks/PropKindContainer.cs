namespace NP.Concepts.Binding.PathLinks
{
    public class PropKindContainer
    {
        public PropertyKind ThePropertyKind { get; }

        public PropKindContainer(PropertyKind propKind)
        {
            ThePropertyKind = propKind;
        }

        public override bool Equals(object targetObj)
        {
            PropKindContainer target =
                targetObj as PropKindContainer;

            if (target == null)
                return false;

            return this.ThePropertyKind == target.ThePropertyKind;
        }

        public override int GetHashCode()
        {
            return this.ThePropertyKind.GetHashCode();
        }
    }
}
