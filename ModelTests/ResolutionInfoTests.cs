using Model;
using Model.Dto;

namespace ModelTests
{
    public class ResolutionInfoTests
    {
        [Theory]
        [InlineData("720p", 720)]
        [InlineData("Some name 480p - 480p", 480)]
        [InlineData("360p name", 360)]
        [InlineData("Name 1080p", 1080)]
        public void Should_CreateResolutionWithHeightOnly_When_RecordWithOnlyFormatHeightValueIsProvided(string format, int expectedHeight)
        {
            FormatDto formatDto = new(format, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default);

            ResolutionInfo? resolutionInfo = ResolutionInfo.FromDto(formatDto);
            Assert.NotNull(resolutionInfo);
            Assert.Null(resolutionInfo!.Width);
            Assert.Equal(expectedHeight, resolutionInfo.Height);
        }

        [Theory]
        [InlineData("640x480", 640, 480)]
        [InlineData("Some name 640x480 - 640x480", 640, 480)]
        [InlineData("640x480 some name", 640, 480)]
        [InlineData("Some name 640x480", 640, 480)]
        public void Should_CreateResolutionWithHeightAndWidth_When_RecordWithOnlyFormatValueIsProvided(string format, int expectedWidth, int expectedHeight)
        {
            FormatDto formatDto = new(format, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default);

            ResolutionInfo? resolutionInfo = ResolutionInfo.FromDto(formatDto);
            Assert.NotNull(resolutionInfo);
            Assert.Equal(expectedWidth, resolutionInfo!.Width);
            Assert.Equal(expectedHeight, resolutionInfo.Height);
        }
    }
}