using System.Collections.Generic;

namespace ArangoDB.Client.Data
{
    public class CreateSearchViewResult : BaseResult
    {
        public string Id { get; set; }
        public string GloballyUniqueId { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }

        public ViewLinkData Links { get; set; }

        public IList<PrimarySort> PrimarySort { get; set; }

        public int? CleanupIntervalStep { get; set; }

        public int? CommitIntervalMsec { get; set; }

        public int? ConsolidationIntervalMsec { get; set; }

        public ConsolidationPolicy ConsolidationPolicy { get; set; }

        public int? WritebufferIdle { get; set; }

        public int? WritebufferActive { get; set; }

        public int? WritebufferSizeMax { get; set; }
    }
}