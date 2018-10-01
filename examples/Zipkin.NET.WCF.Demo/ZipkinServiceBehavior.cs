﻿using System;
using System.Collections.Generic;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Framework;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;
using Zipkin.NET.WCF.Behaviors;

namespace Zipkin.NET.WCF.Demo
{
    public class ZipkinServiceBehavior : ServiceTracingBehavior
    {
        public ZipkinServiceBehavior(string name) : base(name)
        {
        }

        protected override ISampler Sampler => new RateSampler(1f);

        protected override ITraceContextAccessor TraceContextAccessor => new CallContextTraceContextAccessor();

        protected override IInstrumentationLogger Logger => new ConsoleInstrumentationLogger();

        protected override IEnumerable<IReporter> Reporters
        {
            get
            {
                var sender = new ZipkinHttpSender("http://localhost:9411");
                var reporter = new ZipkinReporter(sender);
                return new[] { reporter };
            }
        }

        protected override IDispatcher Dispatcher => 
            new AsyncActionBlockDispatcher(
                Reporters, Logger, TraceContextAccessor);

        public override Type BehaviorType => typeof(ZipkinServiceBehavior);

        protected override object CreateBehavior()
        {
            return new ZipkinServiceBehavior(Name);
        }

    }
}