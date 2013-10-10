﻿using Common.Logging;
using PMCG.Messaging.Client.Configuration;
using System;
using System.Collections.Concurrent;


namespace PMCG.Messaging.Client.BusState
{
	public class Connecting : State
	{
		public Connecting(
			ILog logger,
			BusConfiguration configuration,
			IConnectionManager connectionManager,
			BlockingCollection<QueuedMessage> queuedMessages,
			IBusContext context)
			: base(logger, configuration, connectionManager, queuedMessages, context)
		{
			base.Logger.Info("ctor Completed");
		}


		public override void Close()
		{
			base.Logger.Info("Close Starting");

			base.CloseConnection();
			base.TransitionToNewState(typeof(Closed));

			base.Logger.Info("Close Completed");
		}
	}
}