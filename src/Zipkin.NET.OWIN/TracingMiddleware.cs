﻿using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.OWIN
{
    /// <summary>
    /// OWIN middleware used to build and report server spans from incoming requests.
    /// </summary>
    public class TracingMiddleware
    {
        private readonly string _localEndpointName;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly IDispatcher _dispatcher;
        private readonly ISampler _sampler;
        private readonly IExtractor<IOwinContext> _extractor;

        /// <summary>
        /// Construct a new <see cref="TracingMiddleware"/>.
        /// </summary>
        /// <param name="localEndpointName">
        /// The endpoint name describes the host recording the span.
        /// </param>
        /// <param name="traceContextAccessor">
        /// A <see cref="ITraceContextAccessor"/> used to access trace context.
        /// </param>
        /// <param name="dispatcher">
        /// A <see cref="IDispatcher"/> used to dispatch completed spans to reporters.
        /// </param>
        /// <param name="sampler">
        /// A <see cref="ISampler"/> used to make sampling decisions.
        /// </param>
        public TracingMiddleware(
            string localEndpointName,
            ITraceContextAccessor traceContextAccessor,
            IDispatcher dispatcher,
            ISampler sampler)
        {
            _localEndpointName = localEndpointName;
            _traceContextAccessor = traceContextAccessor;
            _dispatcher = dispatcher;
            _sampler = sampler;
            _extractor = new OwinContextB3Extractor();
        }

        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            var traceContext = _extractor
                .Extract(context)
                .Sample(_sampler);

            var spanBuilder = traceContext
                .GetSpanBuilder()
                .Start()
                .Name(context.Request.Method)
                .Kind(SpanKind.Server)
                .Tag("host", context.Request.Host.Value)
                .Tag("resource", context.Request.Path.Value)
                .Tag("method", context.Request.Method)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _localEndpointName
                });

            _traceContextAccessor.SaveTrace(traceContext);

            try
            {
                await next();
            }
            catch (Exception ex)
            {
                spanBuilder.Error(ex.Message);
            }
            finally
            {
                var span = spanBuilder
                    .End()
                    .Build();

                _dispatcher.Dispatch(span);
            }
        }
    }
}
