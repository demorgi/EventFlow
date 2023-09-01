using System;

namespace EventFlow.Cap.Options;

public class CapPublisherConfiguration
{
    private CapPublisherConfiguration()
    {
    }

    public bool PostEventMetadataAsMessageHeaders { get; set; }

    public Func<string, bool> MetadataKeyFilter { get; set; }

    public static readonly Action<CapPublisherConfiguration> DefaultConfiguration = opt =>
    {
        opt.PostEventMetadataAsMessageHeaders = false;
        opt.MetadataKeyFilter = null;
    };
}