﻿using System;

namespace ZiggyCreatures.Caching.Fusion.Backplane
{
	/// <summary>
	/// Represents a message on a backplane.
	/// </summary>
	public class BackplaneMessage
	{

		/// <summary>
		/// Creates a new instance of a backplane message.
		/// </summary>
		/// <param name="sourceId">The InstanceId of the source cache.</param>
		public BackplaneMessage(string sourceId)
		{
			if (string.IsNullOrEmpty(sourceId))
				throw new ArgumentException("The sourceId cannot be null nor empty", nameof(sourceId));

			SourceId = sourceId;
		}

		/// <summary>
		/// The InstanceId of the source cache.
		/// </summary>
		public string SourceId { get; }

		/// <summary>
		/// The action to broadcast to the backplane.
		/// </summary>
		public BackplaneMessageAction Action { get; private set; }

		/// <summary>
		/// The cache key related to the action, if any.
		/// </summary>
		public string? CacheKey { get; private set; }

		/// <summary>
		/// Checks if a message is valid.
		/// </summary>
		/// <returns>True if it seems valid, false otherwise.</returns>
		public bool IsValid()
		{
			if (string.IsNullOrEmpty(SourceId))
				return false;

			switch (Action)
			{
				case BackplaneMessageAction.Evict:
					if (string.IsNullOrEmpty(CacheKey))
						return false;
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Creates a message.
		/// </summary>
		/// <param name="sourceId">The InstanceId of the source cache.</param>
		/// <param name="action">The action related to the message.</param>
		/// <param name="cacheKey">The cache key related to the message.</param>
		/// <returns>The message.</returns>
		public static BackplaneMessage Create(string sourceId, BackplaneMessageAction action, string? cacheKey)
		{
			return new BackplaneMessage(sourceId)
			{
				Action = action,
				CacheKey = cacheKey
			};
		}

		/// <summary>
		/// Creates a message for a single key eviction.
		/// </summary>
		/// <param name="sourceId">The InstanceId of the source cache.</param>
		/// <param name="cacheKey">The cache key related to the eviction.</param>
		/// <returns>The message.</returns>
		public static BackplaneMessage CreateForEviction(string sourceId, string cacheKey)
		{
			if (string.IsNullOrEmpty(cacheKey))
				throw new ArgumentException("The cache key cannot be null or empty", nameof(cacheKey));

			return new BackplaneMessage(sourceId)
			{
				Action = BackplaneMessageAction.Evict,
				CacheKey = cacheKey
			};
		}
	}
}