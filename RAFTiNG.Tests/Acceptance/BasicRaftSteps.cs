﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicRaftSteps.cs" company="Cyrille DUPUYDAUBY">
//   Copyright 2013 Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RAFTiNG.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using NFluent;

    using TechTalk.SpecFlow;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK here.")][Binding]
    public class BasicRaftSteps: IDisposable
    {
        private Middleware middleware;

        private IDisposable logHandle;

        private List<Node<string>> nodes;

        public BasicRaftSteps()
        {
            logHandle = Helpers.InitLog4Net();
        }

        [Given(@"I have deployed (.*) instances")]
        public void GivenIHaveDeployedInstances(int p0)
        {
            var names = new List<string>(p0);
            for (var i = 0; i < p0; i++)
            {
                names.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            this.middleware = new Middleware();
            this.nodes = new List<Node<string>>(p0);
            var settings = new NodeSettings();
            settings.Nodes = names.ToArray();
            settings.TimeoutInMs = 100;

            for (var i = 0; i < p0; i++)
            {
                settings.NodeId = names[i];
                var node = new Node<string>(settings);
                node.SetMiddleware(this.middleware);
                this.nodes.Add(node);
            }
        }
        
        [When(@"I start instances (.*), (.*) and (.*)")]
        public void WhenIStartInstancesAnd(int p0, int p1, int p2)
        {
            this.nodes[p0 - 1].Initialize();
            this.nodes[p1 - 1].Initialize();
            this.nodes[p2 - 1].Initialize();
        }

        [Then(@"there is (.*) leader")]
        public void ThenThereIsLeader(int p0)
        {
            var leaders = this.nodes.Count(node => node.Status == NodeStatus.Leader);

            Check.That(leaders).IsEqualTo(p0);
        }

        [When(@"I wait (.*) seconde")]
        public void WhenIWaitSeconde(int p0)
        {
            Thread.Sleep(p0 * 1000);
        }

        /// <summary>
        /// Exécute les tâches définies par l'application associées à la libération ou à la redéfinition des ressources non managées.
        /// </summary>
        public void Dispose()
        {
            foreach (var node in nodes)
            {
                node.Dispose();
            }
            logHandle.Dispose();
        }
    }
}
