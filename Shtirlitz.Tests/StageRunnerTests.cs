using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Shtirlitz.Common;
using Xunit;

namespace Shtirlitz.Tests
{
    public class StageRunnerTests
    {
        private readonly IList<Mock<IStage>> stageMocks = new List<Mock<IStage>>();
        private readonly StageRunner runner;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        private int stagesStarted;

        public StageRunnerTests()
        {
            IList<IStage> stages = new List<IStage>();

            // create stage mocks
            for (int i = 0; i < 10; i++)
            {
                Mock<IStage> mock = new Mock<IStage>();
                
                // increment counter when Run is called
                mock.Setup(stage => stage.Run()).Callback(() => stagesStarted++);
                mock.Setup(stage => stage.Weight).Returns(1.0);

                stageMocks.Add(mock);
                stages.Add(mock.Object);
            }

            // create the runner
            runner = new StageRunner("nowhere", stages, tokenSource.Token);
        }

        [Fact]
        public void RunsAllStages()
        {
            runner.Run();

            // test
            Assert.Equal(stageMocks.Count, stagesStarted);
        }

        [Fact]
        public void ReportsProgress()
        {
            double previousProgress = 0.0;
            int progressReports = 0;

            runner.Progress +=
                (progress, stageName, stageType) =>
                    {
                        // check that the progress reported is always bigger than the previous one
                        Assert.True(progress >= previousProgress);

                        // remember previous progress
                        previousProgress = progress;
                        progressReports++;
                    };

            runner.Run();

            // test
            Assert.NotEqual(0, progressReports);
        }

        [Fact]
        public void DoesntRunIfCanceled()
        {
            tokenSource.Cancel();

            Assert.Throws<OperationCanceledException>(() => runner.Run());
            Assert.Equal(0, stagesStarted);
        }

        [Fact]
        public void StopsIfCanceledInTheMiddle()
        {
            // cancel token in the middle
            runner.Progress +=
                (progress, stageName, stageType) =>
                {
                    if (stagesStarted == stageMocks.Count / 2)
                    {
                        tokenSource.Cancel();
                    }
                };

            Assert.Throws<OperationCanceledException>(() => runner.Run());
            Assert.InRange(stagesStarted, stageMocks.Count / 2, stageMocks.Count / 2 + 1);
        }
    }
}