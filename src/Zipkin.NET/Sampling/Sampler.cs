﻿namespace Zipkin.NET.Sampling
{
    /// <summary>
    /// Samplers are responsible for deciding if a particular trace should be sampled.
    /// <remarks>
    /// The sampling decision should be made once and the decision propagated downstream.
    /// </remarks>
    /// </summary>
    public abstract class Sampler
    {
        /// <summary>
        /// Make a sampling decision based on the value of the debug and sampled flags.
        /// <remarks>
        /// If the debug flag has been set then always sample. Otherwise,
        /// if the sampled flag has not been set, make a sampling decision.
        /// Returns the sampling decision.
        /// </remarks>
        /// </summary>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/>.
        /// <remarks>
        /// The sampled property will be set based on the result of the sampling decision.
        /// </remarks>
        /// </param>
        /// <returns>
        /// True if the trace is sampled.
        /// </returns>
        public bool Sample(ref TraceContext traceContext)
        {
            if (traceContext.Debug)
            {
                traceContext.Sampled = true;
                return true;
            }

            if (traceContext.Sampled == null)
            {
                traceContext.Sampled = MakeSamplingDecision(traceContext);
            }

            return traceContext.Sampled == true;
        }

        /// <summary>
        /// Make a sampling decision if the sampled flags has not been set.
        /// </summary>
        /// <param name="traceContext">
        /// The <see cref="TraceContext"/>.
        /// </param>
        /// <returns>
        /// True if the trace is sampled.
        /// </returns>
        protected abstract bool MakeSamplingDecision(TraceContext traceContext);
    }
}
