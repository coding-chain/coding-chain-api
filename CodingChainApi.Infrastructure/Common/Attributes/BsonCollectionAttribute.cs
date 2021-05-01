using System;

namespace CodingChainApi.Infrastructure.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAttribute : Attribute

    {
        public BsonCollectionAttribute(string collectionName)

        {
            CollectionName = collectionName;
        }

        public string CollectionName { get; }
    }
}