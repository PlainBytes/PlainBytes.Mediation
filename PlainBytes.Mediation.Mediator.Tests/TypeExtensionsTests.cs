namespace PlainBytes.Mediation.Mediator.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void GetFormattedName_NonGenericType_ReturnsTypeName()
        {
            // Arrange & Act
            var result = typeof(int).GetFormattedName();

            // Assert
            Assert.Equal("Int32", result);
        }

        [Fact]
        public void GetFormattedName_GenericType_ReturnsFormattedName()
        {
            // Arrange & Act
            var result = typeof(Dictionary<string, int>).GetFormattedName();

            // Assert
            Assert.Equal("Dictionary<String,Int32>", result);
        }

        [Fact]
        public void GetFormattedName_NestedGenericType_ReturnsFullyFormattedName()
        {
            // Arrange & Act
            var result = typeof(Dictionary<string, List<int>>).GetFormattedName();

            // Assert
            Assert.Equal("Dictionary<String,List<Int32>>", result);
        }

        [Fact]
        public void GetFormattedName_GenericTypeDefinition_ReturnsFormattedName()
        {
            // Arrange & Act
            var result = typeof(List<>).GetFormattedName();

            // Assert
            Assert.Equal("List<T>", result);
        }
    }
}
