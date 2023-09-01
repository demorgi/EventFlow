// <copyright file="CapPublisherSubscriber.cs" company="MyCarrier LLC">
// Copyright (c) MyCarrier LLC. All rights reserved.
// Reproduction or transmission in whole or in part, in any form or
// by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using EventFlow.Aggregates;
using EventFlow.Cap.Options;
using EventFlow.Subscribers;
using Microsoft.Extensions.Options;

namespace EventFlow.Cap
{
    public class CapPublisherSubscriber : ISubscribeSynchronousToAll
    {
        private readonly ICapPublisher _publisher;
        private readonly IOptions<CapPublisherConfiguration> _options;

        public CapPublisherSubscriber(ICapPublisher publisher, IOptions<CapPublisherConfiguration> options)
        {
            _publisher = publisher;
            _options = options;
        }

        public async Task HandleAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                var defaultHeaders = new Dictionary<string, string>
                {
                    { CapPublisherDefaultHeaders.AggregateId, domainEvent.GetIdentity().Value },
                    { CapPublisherDefaultHeaders.AggregateVersion, domainEvent.AggregateSequenceNumber.ToString() },
                };

                if (_options.Value.PostEventMetadataAsMessageHeaders)
                {
                    AddMetadataAsHeaders(defaultHeaders, domainEvent);
                }

                await _publisher.PublishAsync(
                    domainEvent.EventType.FullName!,
                    domainEvent.GetAggregateEvent(),
                    defaultHeaders,
                    cancellationToken);
            }
        }

        private void AddMetadataAsHeaders(Dictionary<string, string> defaultHeaders, IDomainEvent domainEvent)
        {
            if (_options.Value.MetadataKeyFilter is null)
            {
                return;
            }

            var additionalKeys = domainEvent.Metadata.Keys.Where(_options.Value.MetadataKeyFilter);

            foreach (var key in additionalKeys)
            {
                defaultHeaders.Add(key, domainEvent.Metadata[key]);
            }
        }
    }
}