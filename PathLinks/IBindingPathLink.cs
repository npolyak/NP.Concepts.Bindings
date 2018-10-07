using NP.Concepts.Binding.GettersAndSetters;

namespace NP.Concepts.Binding.PathLinks
{
    // public interface of a path link
    public interface IBindingPathLink<PropertyType>
    {
        // type of the target property of the link
        PropertyKind ThePropertyKind { get; }

        /// <summary>
        /// returns a getter 
        /// (that notifies when the property changes on the object)</summary>
        /// (you can also trigger the notification by calling 
        /// method TriggerPropertyChanged())
        /// <returns></returns>
        IObjWithPropGetter<PropertyType> CreateGetter();

        // returns a setter (that sets the property value on the containing object)
        IObjWithPropSetter<PropertyType> CreateSetter();
    }
}
