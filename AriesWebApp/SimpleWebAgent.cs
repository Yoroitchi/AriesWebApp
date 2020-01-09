using Hyperledger.Aries.Agents;
using System;


namespace AriesWebApp
{
    internal class SimpleWebAgent : AgentBase
    {
        public SimpleWebAgent(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddForwardHandler();
            AddBasicMessageHandler();
            AddTrustPingHandler();            
        }
    }
}