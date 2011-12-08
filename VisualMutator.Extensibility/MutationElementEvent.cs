namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public class MutationElementEvent: IMutationElement<EventDefinition>
    {
        private TypeIdentifier _typeIdentifier;

        private string _eventFullName;

        public MutationElementEvent(EventDefinition property)
        {
            _typeIdentifier = new TypeIdentifier(property.DeclaringType);
            _eventFullName = property.FullName;
      
        }
        public EventDefinition FindIn(ICollection<AssemblyDefinition> assemblies)
        {
            return _typeIdentifier.FindType(assemblies).Events.Single(f => f.FullName == _eventFullName );
        }
    }
}