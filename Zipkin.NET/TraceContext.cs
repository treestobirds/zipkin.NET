﻿using System;
using Zipkin.NET.Sampling;

namespace Zipkin.NET
{
    /// <summary>
    /// Represents trace context which has come into the application with a request.
    /// </summary>
    public class TraceContext
    {
        private readonly string _spanId;
        private readonly string _parentSpanId;

        /// <summary>
        /// Create a new trace context.
        /// <remarks>
        /// Generates a new trace ID and span builder.
        /// </remarks>
        /// </summary>
        public TraceContext()
        {
            TraceId = GenerateTraceId();
            _spanId = GenerateTraceId();
        }

        /// <summary>
        /// Create a trace context from a trace and span ID extracted from an upstream request.
        /// <remarks>
        /// Sets the parent span ID to the <see cref="spanId"/>
        /// and generates a new span ID for the current trace context.
        /// </remarks>
        /// </summary>
        /// <param name="traceId">
        /// An existing trace ID.
        /// </param>
        /// <param name="spanId">
        /// The upstream span ID.
        /// </param>
        public TraceContext(string traceId, string spanId)
        {
            TraceId = traceId ?? GenerateTraceId();
            _spanId = GenerateTraceId();
            _parentSpanId = spanId;
        }

        /// <summary>
        /// Create a trace context from specified trace ID's.
        /// </summary>
        /// <param name="traceId">
        /// An existing trace ID.
        /// </param>
        /// <param name="spanId">
        /// The span ID.
        /// </param>
        /// <param name="parentSpanId">
        /// The parent span ID.
        /// </param>
        public TraceContext(string traceId, string spanId, string parentSpanId)
        {
            TraceId = traceId;
            _spanId = spanId;
            _parentSpanId = parentSpanId;
        }

        /// <summary>
        /// The overall trace ID of the current trace.
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// Has the debug flag been set?
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Should this trace be sampled.]?
        /// <remarks>
        /// The Sample() method should be used to make a sampling decision.
        /// </remarks>
        /// </summary>
        public bool? Sampled { get; set; }

        /// <summary>
        /// Gets a <see cref="SpanBuilder"/> used to build spans.
        /// </summary>
        /// <param name="refresh">
        /// True if the trace ID's need to be updated (when starting a new child trace).
        /// </param>
        /// <returns>
        /// The a new <see cref="SpanBuilder"/>.
        /// </returns>
        public SpanBuilder GetSpanBuilder(bool refresh = false)
        {
            return refresh 
                ? Refresh().GetSpanBuilder(false)
                : new SpanBuilder(TraceId, _spanId, _parentSpanId);
        }

        /// <summary>
        /// Make a sampling decision.
        /// <remarks>
        /// The sampling decision is based on the presence of the sampling and debug
        /// flags. If no sampling flag exists and the debug flag has not been set,
        /// the <see cref="ISampler"/> is used to make a sampling decision.
        /// </remarks>
        /// </summary>
        /// <param name="sampler">
        /// An <see cref="ISampler"/> used to make sampling decisions.
        /// </param>
        /// <returns>
        /// The current <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext Sample(ISampler sampler)
        {
            Sampled =  Debug || sampler.IsSampled(this);
            return this;
        }

        /// <summary>
        /// Refresh the trace ID's by setting the parent span ID
        /// to the current span ID and generating a new span ID.
        /// </summary>
        /// <returns>
        /// A new <see cref="TraceContext"/>.
        /// </returns>
        public TraceContext Refresh()
        {
            var traceId = TraceId ?? GenerateTraceId();
            return new TraceContext(traceId, GenerateTraceId(), _spanId);
        }

        /// <summary>
        /// Generate a 64-bit trace ID.
        /// </summary>
        /// <returns>
        /// The trace ID as a string.
        /// </returns>
        public string GenerateTraceId()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        }
    }
}
