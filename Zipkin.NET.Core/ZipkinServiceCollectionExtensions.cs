﻿using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Core
{
    public static class ZipkinServiceCollectionExtensions
    {
        public static IServiceCollection AddZipkin(
            this IServiceCollection services, string applicationName, string zipkinHost)
        {
            // TODO is this needed?
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISampler, DebugSampler>();
            services.AddTransient<IReporter, Reporter>();
            services.AddTransient<ISampler, DebugSampler>();
            services.AddTransient<ISender>(provider => new HttpSender(zipkinHost));
            services.AddTransient<ITraceContextAccessor, HttpContextTraceContextAccessor>();
            services.AddTransient<IPropagator<HttpRequestMessage>, HttpRequestMessageB3Extractor>();
            services.AddTransient<IExtractor<HttpRequest>, HttpRequestB3Propagator>();

            // Register middleware
            services.AddTransient(provider =>
            {
                var reporter = provider.GetService<IReporter>();
                var sampler = provider.GetService<ISampler>();
                var extractor = provider.GetService<IExtractor<HttpRequest>>();
                var traceContextAccessor = provider.GetService<ITraceContextAccessor>();
                var middleware = new ZipkinMiddleware(
                    applicationName, reporter, sampler, traceContextAccessor, extractor);
                return middleware;
            });

            return services;
        }
    }
}
