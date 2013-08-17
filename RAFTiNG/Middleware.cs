﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Middleware.cs" company="Cyrille DUPUYDAUBY">
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

namespace RAFTiNG
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Middleware simulates the message middleware used by rafting nodes to communicate and synchronize
    /// </summary>
    public class Middleware
    {
        private readonly Dictionary<string, Action<object>> endpoints = new Dictionary<string, Action<object>>();

        /// <summary>
        /// Sends a message to a specific address.
        /// </summary>
        /// <param name="address">The address to send the message to.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>false if the message was not sent.</returns>
        /// <remarks>This is a best effort delivery contract. There is no guaranteed delivery.</remarks>
        public bool SendMessage(string address, object message)
        {
            if (this.endpoints.ContainsKey(address))
            {
                try
                {
                    this.endpoints[address].Invoke(message);
                }
                catch (Exception)
                {
                    // exceptions must not cross middleware boundaries
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Registers the end point to process received messages.
        /// </summary>
        /// <param name="address">The address to register to.</param>
        /// <param name="messageReceived">The message processing method.</param>
        /// <exception cref="System.InvalidOperationException">If an endpoint is registered more than once.</exception>
        public void RegisterEndPoint(string address, Action<object> messageReceived)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException(address, "address must contain a value.");
            }

            if (messageReceived == null)
            {
                throw new ArgumentNullException("messageReceived");
            }

            if (this.endpoints.ContainsKey(address))
            {
                // double registration is development error.
                throw new InvalidOperationException("Invalid registration attempt: endpoints can only be registered once.");
            }

            this.endpoints[address] = messageReceived;
        }
    }
}