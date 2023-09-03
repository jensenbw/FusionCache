﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ZiggyCreatures.Caching.Fusion.Chaos.Internals
{
	/// <summary>
	/// A base implementation for a component with a controllable amount of chaos.
	/// </summary>
	public abstract class AbstractChaosComponent
	{
		private readonly string _className;

		/// <summary>
		/// The <see cref="ILogger"/> to use, or <see langword="null"/> for no logging.
		/// </summary>
		protected readonly ILogger? _logger;

		/// <summary>
		/// Initializes a new instance of the AbstractChaosComponent class.
		/// </summary>
		/// <param name="logger">The logger to use, or <see langword="null"/>.</param>
		protected AbstractChaosComponent(ILogger? logger)
		{
			_className = GetType().Name;

			_logger = logger;

			ChaosThrowProbability = 0f;
			ChaosMinDelay = TimeSpan.Zero;
			ChaosMaxDelay = TimeSpan.Zero;
		}

		/// <summary>
		/// The maximum amount of randomized delay.
		/// </summary>
		public TimeSpan ChaosMaxDelay { get; set; }

		/// <summary>
		/// The minimum amount of randomized delay.
		/// </summary>
		public TimeSpan ChaosMinDelay { get; set; }

		/// <summary>
		/// A <see cref="float"/> value from 0.0 to 1.0 that represents the probabilty of throwing an exception: set it to 0.0 to never throw or to 1.0 to always throw.
		/// </summary>
		public float ChaosThrowProbability { get; set; }

		/// <summary>
		/// Force chaos exceptions to always throw, and chaos delays to always be of exactly this amount.
		/// </summary>
		/// <param name="delay"></param>
		public virtual void SetAlwaysChaos(TimeSpan delay)
		{
			if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
				_logger.Log(LogLevel.Debug, $"FUSION {_className}: SetAlwaysChaos");

			SetAlwaysThrow();
			SetAlwaysDelayExactly(delay);
		}

		/// <summary>
		/// Force chaos delays to always be of exactly this amount.
		/// </summary>
		/// <param name="delay"></param>
		public virtual void SetAlwaysDelayExactly(TimeSpan delay)
		{
			if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
				_logger.Log(LogLevel.Debug, $"FUSION {_className}: SetAlwaysDelayExactly");

			ChaosMinDelay = delay;
			ChaosMaxDelay = delay;
		}

		/// <summary>
		/// Force chaos exceptions to always be thrown.
		/// </summary>
		public virtual void SetAlwaysThrow()
		{
			if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
				_logger.Log(LogLevel.Debug, $"FUSION {_className}: SetAlwaysThrow");

			ChaosThrowProbability = 1f;
		}

		/// <summary>
		/// Force chaos exceptions and delays to never happen.
		/// </summary>
		public virtual void SetNeverChaos()
		{
			if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
				_logger.Log(LogLevel.Debug, $"FUSION {_className}: SetNeverChaos");

			SetNeverThrow();
			SetNeverDelay();
		}

		/// <summary>
		/// Force chaos delays to never happen.
		/// </summary>
		public virtual void SetNeverDelay()
		{
			if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
				_logger.Log(LogLevel.Debug, $"FUSION {_className}: SetNeverDelay");

			ChaosMinDelay = TimeSpan.Zero;
			ChaosMaxDelay = TimeSpan.Zero;
		}

		/// <summary>
		/// Force chaos exceptions to never be thrown.
		/// </summary>
		public virtual void SetNeverThrow()
		{
			if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
				_logger.Log(LogLevel.Debug, $"FUSION {_className}: SetNeverThrow");

			ChaosThrowProbability = 0f;
		}

		/// <summary>
		/// Randomize an actual delay with a value between the configured min/max values, and if needed waits for it.
		/// Then, maybe, throw a <see cref="ChaosException"/> based on the specified probabilty.
		/// </summary>
		protected void MaybeChaos()
		{
			FusionCacheChaosUtils.MaybeChaos(ChaosMinDelay, ChaosMaxDelay, ChaosThrowProbability);
		}

		/// <summary>
		/// Randomize an actual delay with a value between the configured min/max values, and if needed waits for it.
		/// Then, maybe, throw a <see cref="ChaosException"/> based on the specified probabilty.
		/// </summary>
		protected async Task MaybeChaosAsync()
		{
			await FusionCacheChaosUtils.MaybeChaosAsync(ChaosMinDelay, ChaosMaxDelay, ChaosThrowProbability).ConfigureAwait(false);
		}
	}
}