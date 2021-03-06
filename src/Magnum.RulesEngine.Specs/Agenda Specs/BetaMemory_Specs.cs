// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace BetaMemory_Specs
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Magnum.RulesEngine;
	using Magnum.RulesEngine.ExecutionModel;
	using Magnum.RulesEngine.Specs.Model;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class BetaMemory_Specs
	{
		[SetUp]
		public void Setup()
		{
			_customer = new Customer {Preferred = true};

			_actionNode = new ActionNode<Customer>(x => Trace.WriteLine("Called for " + x.Element.Object.Preferred));

			_constantNode = new ConstantNode<Customer>();

			_agenda = new PriorityQueueAgenda();

			_context = MockRepository.GenerateMock<RuleContext<Customer>>();
			var element = MockRepository.GenerateMock<SessionElement<Customer>>();
			element.Stub(x => x.Object).Return(_customer);
			_context.Stub(x => x.Element).Return(element);

			_context.Expect(x => x.EnqueueAgendaAction(0, null))
				.IgnoreArguments()
				.Repeat.AtLeastOnce()
				.WhenCalled(invocation =>
					{
						var priority = (int) invocation.Arguments[0];
						var action = invocation.Arguments[1] as Action;

						_agenda.Add(priority, action);
					});
		}

		private Customer _customer;
		private ActionNode<Customer> _actionNode;
		private ConstantNode<Customer> _constantNode;
		private RuleContext<Customer> _context;
		private Agenda _agenda;

		[Test]
		public void FirstTestName()
		{
			var junction = new JoinNode<Customer>(_constantNode);
			junction.AddSuccessor(_actionNode);

			var memoryA = new BetaMemory<Customer>(junction);

			memoryA.Activate(_context);

			_agenda.Execute();

			_context.VerifyAllExpectations();
		}

		[Test]
		public void Pulling_an_element_through_two_memories_should_merge_properly()
		{
			var junction = new JoinNode<Customer>(_constantNode);
			junction.AddSuccessor(_actionNode);

			var memoryB = new BetaMemory<Customer>(junction);

			var joinJunction = new JoinNode<Customer>(memoryB);

			var memoryA = new BetaMemory<Customer>(joinJunction);

			memoryA.Activate(_context);
			memoryB.Activate(_context);

			_agenda.Execute();

			_context.VerifyAllExpectations();
		}
	}
}