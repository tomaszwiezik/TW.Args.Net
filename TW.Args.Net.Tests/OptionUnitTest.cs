namespace TW.Args.Net.Tests
{
    public class OptionUnitTest
    {
        [Fact]
        public void TestInitializationWithValue()
        {
            var nameValue = "name=value";
            var option = new Option(nameValue);

            Assert.Equal("name", option.Name);
            Assert.Equal("value", option.Value);
            Assert.True(option.HasValue);
        }


        [Fact]
        public void TestInitializationWithValueContainingEqualChar()
        {
            var nameValue = "name=value=999";
            var option = new Option(nameValue);

            Assert.Equal("name", option.Name);
            Assert.Equal("value=999", option.Value);
            Assert.True(option.HasValue);
        }


        [Fact]
        public void TestInitializationWithoutValue()
        {
            var nameValue = "name";
            var option = new Option(nameValue);

            Assert.Equal("name", option.Name);
            Assert.Null(option.Value);
            Assert.False(option.HasValue);
        }

    }
}
