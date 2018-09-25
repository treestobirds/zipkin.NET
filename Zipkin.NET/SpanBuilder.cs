﻿using System;
using System.Collections.Generic;
using Zipkin.NET.Models;

namespace Zipkin.NET
{
    /// <summary>
    /// Helper class used to build spans.
    /// </summary>
    public class SpanBuilder
    {
        private readonly Span _span;

        /// <summary>
        /// Construct a new <see cref="SpanBuilder"/>.
        /// </summary>
        /// <param name="traceId">
        /// The trace ID for the current trace context.
        /// </param>
        /// <param name="id">
        /// The span ID for the current trace context.
        /// </param>
        /// <param name="parentId">
        /// The parent span ID for the current trace context.
        /// </param>
        public SpanBuilder(string traceId, string id, string parentId)
        {
            _span = new Span
            {
                TraceId = traceId,
                Id = id,
                ParentId = parentId
            };
        }

        /// <summary>
        /// Record the start time and start duration timer.
        /// </summary>
        public SpanBuilder Start()
        {
            _span.StartTime = DateTime.UtcNow;
            return this;
        }

        /// <summary>
        /// Calculate the duration from the time the  start time was recorded.
        /// </summary>
        public SpanBuilder End()
        {
            _span.Duration = DateTime.UtcNow.Subtract(_span.StartTime);
            return this;
        }

        /// <summary>
        /// Set the name of the logical operation the span represents.
        /// </summary>
        /// <param name="name">
        /// The name of the operation.
        /// </param>
        public SpanBuilder Name(string name)
        {
            _span.Name = name;
            return this;
        }

        /// <summary>
        /// Add additional context information to a trace.
        /// </summary>
        /// <param name="name">
        /// The tag key. This should be unique.
        /// </param>
        /// <param name="value">
        /// The tag value.
        /// </param>
        public SpanBuilder Tag(string name, string value)
        {
            if (_span.Tags == null)
                _span.Tags = new Dictionary<string, string>();

            _span.Tags.Add(name, value);
            return this;
        }

        /// <summary>
        /// Add error context information to a trace.
        /// </summary>
        /// <param name="message">
        /// The error message.
        /// </param>
        public SpanBuilder Error(string message)
        {
            Tag("error", message);
            return this;
        }

        /// <summary>
        /// Add an event that explains latency with a timestamp.
        /// </summary>
        /// <param name="annotation">
        /// The value and associated timestamp.
        /// </param>
        public SpanBuilder Annotate(Annotation annotation)
        {
            if (_span.Annotations == null)
                _span.Annotations = new List<Annotation>();

            _span.Annotations.Add(annotation);
            return this;
        }

        /// <summary>
        /// Set the span local endpoint.
        /// <remarks>
        /// The local endpoint describes the host that is recording the span.
        /// </remarks>
        /// </summary>
        /// <param name="endpoint">
        /// The <see cref="Endpoint"/>.
        /// </param>
        public SpanBuilder WithLocalEndpoint(Endpoint endpoint)
        {
            _span.LocalEndpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Set the span remote endpoint.
        /// <remarks>
        /// The span endpoint describes the other side of the connection.
        /// </remarks>
        /// </summary>
        /// <param name="endpoint">
        /// The <see cref="Endpoint"/>.
        /// </param>
        public SpanBuilder WithRemoteEndpoint(Endpoint endpoint)
        {
            _span.RemoteEndpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Set the span kind.
        /// </summary>
        /// <remarks>
        /// The span kind clarifies the duration and start time.
        /// </remarks>
        /// <param name="kind">
        /// The <see cref="SpanKind"/>.
        /// </param>
        public SpanBuilder Kind(SpanKind kind)
        {
            _span.Kind = kind;
            return this;
        }

        /// <summary>
        /// Build the span.
        /// </summary>
        /// <returns>
        /// A <see cref="Span"/>.
        /// </returns>
        public Span Build()
        {
            return _span;
        }
    }
}
