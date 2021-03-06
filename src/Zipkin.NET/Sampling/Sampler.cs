﻿namespace Zipkin.NET.Sampling
{
    /// <summary>
    /// Samplers are responsible for deciding if a particular trace should be sampled.
    /// <remarks>
    /// The sampling decision should be made once and the decision propagated downstream.
    /// </remarks>
    /// </summary>
    public abstract class Sampler : ISampler
    {
        /// <summary>
        /// Make a sampling decision based on the value of the debug and sampled flags.
        /// <remarks>
        /// If the debug flag has been set then always sample. Otherwise,
        /// if the sampled flag has not been set, make a sampling decision return the decision.
        /// </remarks>
        /// </summary>
        /// <param name="spanContext">
        /// The <see cref="SpanContext"/>.
        /// <remarks>
        /// The sampled property will be set based on the result of the sampling decision.
        /// </remarks>
        /// </param>
        /// <returns>
        /// True if the trace is sampled.
        /// </returns>
        public bool IsSampled(SpanContext spanContext)
        {
            if (spanContext.Debug)
            {
                return true;
            }

            if (spanContext.Sampled != null)
            {
                return spanContext.Sampled == true;
            }

            // No sampling decision has been made upstream, make a sampling decision.
            return IsSampled(spanContext.TraceId);
        }

        /// <summary>
        /// Make a sampling decision if the sampled flags has not been set.
        /// </summary>
        /// <param name="traceId">
        /// The trace ID.
        /// </param>
        /// <returns>
        /// True if the trace is sampled.
        /// </returns>
        protected abstract bool IsSampled(string traceId);
    }
}
