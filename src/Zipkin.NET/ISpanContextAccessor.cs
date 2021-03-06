﻿namespace Zipkin.NET
{
    /// <summary>
    /// Used to access the span context of the parent span across the application.
    /// <remarks>
    /// Implementations should use some persistent store across the 
    /// context of the current request such as the HttpContext or CallContext.
    /// </remarks>
    /// </summary>
    public interface ISpanContextAccessor
    {
        /// <summary>
        /// Saves the current <see cref="SpanContext"/>.
        /// </summary>
        /// <param name="spanContext">
        /// The <see cref="SpanContext"/>.
        /// </param>
        void SaveContext(SpanContext spanContext);

        /// <summary>
        /// Retrieves the parent <see cref="SpanContext"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="SpanContext"/>.
        /// </returns>
        SpanContext GetContext();

        /// <summary>
        /// Checks if a <see cref="SpanContext"/> is stored in the context of the current request.
        /// </summary>
        /// <returns>
        /// True if a span context is stored, else false.
        /// </returns>
        bool HasContext();
    }
}
