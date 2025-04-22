using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace TodoList.Utils;

public static class LoggerAssertions
{
    public static void ShouldHaveSingleError(this FakeLogger logger, string expectedMessage)
    {
        logger.Collector.Count.Should().Be(1);
        logger.LatestRecord.Level.Should().Be(LogLevel.Error);
        logger.Collector.LatestRecord.Message.Should().Contain(expectedMessage);
    }

    public static void ShouldHaveSingleInfo(this FakeLogger logger)
    {
        logger.Collector.Count.Should().Be(1);
        logger.LatestRecord.Message.Should().NotBeNullOrWhiteSpace();
        logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }
}
