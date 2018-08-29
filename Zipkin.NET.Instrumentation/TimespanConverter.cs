﻿using System;
using Newtonsoft.Json;

namespace Zipkin.NET.Instrumentation
{
	public class TimeSpanConverter : JsonConverter<TimeSpan>
	{
		public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
		{
			writer.WriteRawValue((value.Milliseconds * 1000).ToString());
		}

		public override TimeSpan ReadJson(
			JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return existingValue;
		}
	}
}