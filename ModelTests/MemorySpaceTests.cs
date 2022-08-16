using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTests
{
    public class MemorySpaceTests
    {
        [Theory]
        [InlineData(1048576, 1024, 1, 0.001)]
        [InlineData(53718548, 52459.52, 51.23, 0.05)]
        public void Should_ReturnProperPropertiesValues_When_BytesConstructorIsUsed(long bytes, double kilobytes, double megabytes, double gigabytes)
        {
            MemorySpace memorySize = new(bytes);

            Assert.Equal(bytes, bytes);
            Assert.Equal(kilobytes, memorySize.Kilobytes, 3);
            Assert.Equal(megabytes, memorySize.Megabytes, 3);
            Assert.Equal(gigabytes, memorySize.Gigabytes, 3);
        }

        [Theory]
        [InlineData(20.5, 30.5, 51)]
        public void Should_ReturnProperValue_When_AddingTwoObjects(double x, double y, double expectedSum)
        {
            MemorySpace a = MemorySpace.FromMegabytes(x);
            MemorySpace b = MemorySpace.FromMegabytes(y);

            MemorySpace sumSpace = a + b;
            Assert.Equal(expectedSum, sumSpace.Megabytes);
        }

        [Theory]
        [InlineData(125962547, 120.13, MemoryUnit.Megabyte)]
        public void Should_ReturnValueWithGoodUnit_When_GetSizeWithMostSuitableUnitIsCalled(long bytes, double expectedValue, MemoryUnit expectedUnit)
        {
            MemorySpace space = new(bytes);
            MemoryUnit unit = space.GetSizeWithMostSuitableUnit(out double value);

            Assert.Equal(expectedValue, value, 2);
            Assert.Equal(expectedUnit, unit);
        }
    }
}
