using NUnit.Framework;

namespace NeosCoding.WebApi.Tests
{
    public record Person(string Firstname, string Lastname)
    {
        public virtual bool Equals(Person? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Firstname == other.Firstname;
        }

        public override int GetHashCode()
        {
            return Firstname != null ? Firstname.GetHashCode() : 0;
        }
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var p1 = new Person("jean", "Pedro");
            var p2 = new Person("mathilde", "Pedro");
            Assert.True(p1 != p2);
            // Assert.AreNotEqual(p1, p2);
        }
    }
}