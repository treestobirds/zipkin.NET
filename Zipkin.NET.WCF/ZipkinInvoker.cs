﻿using System;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.WCF
{
    public class ZipkinInvoker : IOperationInvoker
    {
        private readonly string _applicationName;
        private readonly IOperationInvoker _originalInvoker;
        private readonly IReporter _reporter;
        private readonly ITraceAccessor _traceAccecssor;
        private readonly IExtractor<IncomingWebRequestContext> _extractor;

        public ZipkinInvoker(
            string applicationName,
            IOperationInvoker originalInvoker,
            IReporter reporter,
            ITraceAccessor traceAccessor,
            IExtractor<IncomingWebRequestContext> extractor)
        {
            _applicationName = applicationName;
            _originalInvoker = originalInvoker;
            _reporter = reporter;
            _traceAccecssor = traceAccessor;
            _extractor = extractor;
        }

        public bool IsSynchronous => _originalInvoker.IsSynchronous;

        public object[] AllocateInputs() { return _originalInvoker.AllocateInputs(); }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            var parentSpan = _extractor.Extract(WebOperationContext.Current?.IncomingRequest);
            var trace = new Trace(parentSpan.TraceId, parentSpan.Id);

            var spanBuilder = trace
                .GetSpanBuilder()
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                })
                .Start();
                
            _traceAccecssor.SaveTrace(trace);

            var response = _originalInvoker.Invoke(instance, inputs, out outputs);

            spanBuilder.End();

            _reporter.Report(spanBuilder.Build());

            return response;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs,
            AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
            //var res = _originalInvoker.InvokeBegin(instance, inputs, callback, state);
            //return res;
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotImplementedException();
            //var res = _originalInvoker.InvokeEnd(instance, out outputs, result);
            //return res;
        }
    }
}