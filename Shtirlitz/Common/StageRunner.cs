using System.Collections.Generic;
using System.Threading;

namespace Shtirlitz.Common
{
    /// <summary>
    /// This class implements the stage that runs several stages inside itself.
    /// </summary>
    public class StageRunner : AbstractStage
    {
        private readonly IList<IStage> stages;
        private readonly double weight;

        public StageRunner(string rootPath, IList<IStage> stages, CancellationToken cancellationToken, string name = null) 
            : base(rootPath, name, ReportStageType.Composite, cancellationToken)
        {
            this.stages = stages;

            // calculate the cumulative weight of all the stages
            foreach (IStage stage in stages)
            {
                weight += stage.Weight;
            }
        }

        public override double Weight
        {
            get { return weight; }
        }

        public override void Run()
        {
            // allocate completed weight counter
            double overallWeightComplete = 0;
            
            // run stages sequentially
            for (int i = 0; i < stages.Count; i++)
            {
                // copy variables to the loop code block
                IStage stage = stages[i];
                double weightComplete = overallWeightComplete;

                // try to cancel the operation
                CancellationToken.ThrowIfCancellationRequested();

                // notify about our progress
                RaiseProgress(overallWeightComplete / weight, stage.Name, stage.Type);

                // create stage progress callback
                ProgressCallback progressCallback = (progress, name, type) => RaiseProgress((weightComplete + stage.Weight * progress) / weight, name, type);

                // subscribe for stage progress
                stage.Progress += progressCallback;

                // run the stage
                stage.Run();

                // unsubscribe from stage progress
                stage.Progress -= progressCallback;

                // add stage weight to the completed weight
                overallWeightComplete += stage.Weight;
            }

            // signal operation complete
            RaiseProgress(1.0);
        }
    }
}