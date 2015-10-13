using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using ApiDemo;

namespace BusinessLogicService.Security // Do not change this
{
    /// <summary>
    /// Sample code to attach credentials to the service message.
    /// </summary>
    public class ClientMessageInspector : BehaviorExtensionElement, IClientMessageInspector, IEndpointBehavior
    {
        #region [ Public Members ]

        public Guid AuthToken { get; set; }
        
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var lToken = new Token { AuthToken = AuthToken };
            request.Headers.Add(lToken.ToMessageHeader());
            return null;
        }

        public override Type BehaviorType
        {
            get { return GetType(); }
        }

        protected override object CreateBehavior()
        {
            return this;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}
