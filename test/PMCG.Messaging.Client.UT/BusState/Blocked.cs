﻿using NSubstitute;
using NUnit.Framework;
using PMCG.Messaging.Client.BusState;
using RabbitMQ.Client;
using System;


namespace PMCG.Messaging.Client.UT.BusState
{
	[TestFixture]
	public class Blocked
	{
		[Test]
		public void Ctor_Success()
		{
			var _busConfigurationBuilder = new PMCG.Messaging.Client.Configuration.BusConfigurationBuilder();
			_busConfigurationBuilder.ConnectionUris.Add(TestingConfiguration.LocalConnectionUri);
			_busConfigurationBuilder
				.RegisterPublication<MyEvent>(
					TestingConfiguration.ExchangeName,
					typeof(MyEvent).Name);
			var _busConfirguration = _busConfigurationBuilder.Build();

			var _connectionManager = Substitute.For<IConnectionManager>();
			var _context = Substitute.For<IBusContext>();

			var _SUT = new PMCG.Messaging.Client.BusState.Blocked(
				_busConfirguration,
				_connectionManager,
				_context);
		}


		[Test]
		public void Publish_Where_No_Publication_Configurations_Which_Results_In_A_NoConfigurationFound_Result()
		{
			var _busConfigurationBuilder = new PMCG.Messaging.Client.Configuration.BusConfigurationBuilder();
			_busConfigurationBuilder.ConnectionUris.Add(TestingConfiguration.LocalConnectionUri);
			var _busConfirguration = _busConfigurationBuilder.Build();

			var _connectionManager = Substitute.For<IConnectionManager>();
			var _connection = Substitute.For<IConnection>();
			var _context = Substitute.For<IBusContext>();

			var _SUT = new PMCG.Messaging.Client.BusState.Blocked(
				_busConfirguration,
				_connectionManager,
				_context);

			var _theEvent = new MyEvent(Guid.NewGuid(), null, "Some detail", 1);
			var _publicationResult = _SUT.PublishAsync(_theEvent);
			_publicationResult.Wait();

			Assert.AreEqual(PMCG.Messaging.PublicationResultStatus.NoConfigurationFound, _publicationResult.Result.Status);
		}

	
		[Test]
		public void Publish_Where_Publication_Configurations_Exist_Which_Results_In_Blocked_Result()
		{
			var _busConfigurationBuilder = new PMCG.Messaging.Client.Configuration.BusConfigurationBuilder();
			_busConfigurationBuilder.ConnectionUris.Add(TestingConfiguration.LocalConnectionUri);
			_busConfigurationBuilder
				.RegisterPublication<MyEvent>(
					TestingConfiguration.ExchangeName,
					typeof(MyEvent).Name);
			var _busConfirguration = _busConfigurationBuilder.Build();

			var _connectionManager = Substitute.For<IConnectionManager>();
			var _context = Substitute.For<IBusContext>();

			var _SUT = new PMCG.Messaging.Client.BusState.Blocked(
				_busConfirguration,
				_connectionManager,
				_context);

			var _theEvent = new MyEvent(Guid.NewGuid(), null, "Some detail", 1);
			var _publicationResult = _SUT.PublishAsync(_theEvent);
			_publicationResult.Wait();

			Assert.AreEqual(PMCG.Messaging.PublicationResultStatus.Blocked, _publicationResult.Result.Status);
		}


		[Test]
		public void Close_Results_In_Transition_To_Closed_State()
		{
			var _busConfigurationBuilder = new PMCG.Messaging.Client.Configuration.BusConfigurationBuilder();
			_busConfigurationBuilder.ConnectionUris.Add(TestingConfiguration.LocalConnectionUri);
			var _busConfirguration = _busConfigurationBuilder.Build();

			var _connectionManager = Substitute.For<IConnectionManager>();
			var _connection = Substitute.For<IConnection>();
			var _context = Substitute.For<IBusContext>();

			var _SUT = new PMCG.Messaging.Client.BusState.Blocked(
				_busConfirguration,
				_connectionManager,
				_context);

			_context.State.Returns(callInfo => _SUT);
			State _capturedState = null;
			_context.When(context => context.State = Arg.Any<State>()).Do(callInfo => _capturedState = callInfo[0] as State);
			_SUT.Close();

			Assert.IsInstanceOf<PMCG.Messaging.Client.BusState.Closed>(_capturedState);
			_connectionManager.Received().Close();
		}

	
		[Test]
		public void State_Changed_Where_Connection_Is_Disconnected_Results_In_Transition_To_Disconnected_State()
		{
			var _busConfigurationBuilder = new PMCG.Messaging.Client.Configuration.BusConfigurationBuilder();
			_busConfigurationBuilder.ConnectionUris.Add(TestingConfiguration.LocalConnectionUri);
			var _busConfirguration = _busConfigurationBuilder.Build();

			var _connectionManager = Substitute.For<IConnectionManager>();
			var _connection = Substitute.For<IConnection>();
			var _context = Substitute.For<IBusContext>();

			var _SUT = new PMCG.Messaging.Client.BusState.Blocked(
				_busConfirguration,
				_connectionManager,
				_context);

			_context.State.Returns(callInfo => _SUT);
			State _capturedState = null;
			_context.When(context => context.State = Arg.Any<State>()).Do(callInfo => _capturedState = callInfo[0] as State);
			_connectionManager.Disconnected += Raise.Event<EventHandler<ConnectionDisconnectedEventArgs>>(_connection, new ConnectionDisconnectedEventArgs(1, "."));

			Assert.IsInstanceOf<PMCG.Messaging.Client.BusState.Disconnected>(_capturedState);
		}


		[Test]
		public void State_Changed_Where_Connection_Is_Unblocked_Results_In_Transition_To_Connected_State()
		{
			var _busConfigurationBuilder = new PMCG.Messaging.Client.Configuration.BusConfigurationBuilder();
			_busConfigurationBuilder.ConnectionUris.Add(TestingConfiguration.LocalConnectionUri);
			var _busConfirguration = _busConfigurationBuilder.Build();

			var _connectionManager = Substitute.For<IConnectionManager>();
			var _connection = Substitute.For<IConnection>();
			var _context = Substitute.For<IBusContext>();

			var _SUT = new PMCG.Messaging.Client.BusState.Blocked(
				_busConfirguration,
				_connectionManager,
				_context);

			_context.State.Returns(callInfo => _SUT);
			State _capturedState = null;
			_context.When(context => context.State = Arg.Any<State>()).Do(callInfo => _capturedState = callInfo[0] as State);
			_connectionManager.Unblocked += Raise.Event<EventHandler<EventArgs>>(_connection, new EventArgs());

			Assert.IsInstanceOf<PMCG.Messaging.Client.BusState.Connected>(_capturedState);
		}
	}
}