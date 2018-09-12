﻿using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.OWIN
{
	public class ZipkinMiddleware
	{
		private readonly string _applicationName;
		private readonly IReporter _reporter;
		private readonly IB3Propagator _propagator;
		private readonly ITraceContextAccessor _traceContextAccessor;

		public ZipkinMiddleware(
			string applicationName,
			IReporter reporter,
			IB3Propagator propagator,
			ITraceContextAccessor traceContextAccessor)
		{
			_applicationName = applicationName;
			_reporter = reporter;
			_propagator = propagator;
			_traceContextAccessor = traceContextAccessor;
		}

		public async Task Invoke(IOwinContext context, Func<Task> next)
		{
			// Extract X-B3 headers
			var traceContext = _propagator
				.Extract(context)
				.NewChild();

			// Record the server trace context so we can
			// later retrieve the values for the client trace.
			_traceContextAccessor.Context = traceContext;

			var serverTrace = new ServerTrace(
				traceContext,
				context.Request.Method,
				localEndpoint: new Endpoint
				{
					ServiceName = _applicationName
				});

			// Record server recieve start time and start duration timer
			serverTrace.Start();

			try
			{
				await next();

			}
			catch (Exception ex)
			{
				serverTrace.Error(ex.Message);
				throw;
			}
			finally
			{
				serverTrace.End();

				if (traceContext.Sample())
					// Report completed span to Zipkin
					_reporter.Report(serverTrace.Span);
			}
		}
	}
}