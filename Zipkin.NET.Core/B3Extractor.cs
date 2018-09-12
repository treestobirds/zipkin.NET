﻿using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Core
{
    /// <summary>
    /// Extracts a <see cref="TraceContext"/> from a <see cref="HttpRequest"/>.
    /// </summary>
    public class B3Extractor : IExtractor<HttpRequest>
    {
        private readonly ISampler _sampler;

        public B3Extractor(ISampler sampler)
        {
            _sampler = sampler;
        }

        /// <summary>
        /// Extracts the X-B3 trace ID header values from the request.
        /// </summary>
        /// <param name="request">
        /// The request from which to extract the request headers.
        /// </param>
        /// <returns>
        /// A <see cref="TraceContext"/> containing the header values.
        /// </returns>
        public TraceContext Extract(HttpRequest request)
        {
            string traceId = null;
            if (request.Headers.TryGetValue(B3HeaderConstants.TraceId, out var value))
            {
                traceId = value;
            }

            string spanId = null;
            if (request.Headers.TryGetValue(B3HeaderConstants.SpanId, out value))
            {
                spanId = value;
            }

            bool? sampled = null;
            if (request.Headers.TryGetValue(B3HeaderConstants.Sampled, out value))
            {
                sampled = value == "1";
            }

            bool? debug = null;
            if (request.Headers.TryGetValue(B3HeaderConstants.Flags, out value))
            {
                debug = value == "1";
            }

            return new TraceContext(_sampler)
            {
                TraceId = traceId,
                SpanId = spanId,
                Sampled = sampled,
                Debug = debug
            };
        }
    }
}
