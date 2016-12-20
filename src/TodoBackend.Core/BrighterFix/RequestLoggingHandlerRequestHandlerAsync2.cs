using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.Logging;

namespace TodoBackend.Core.BrighterFix
{
    public class RequestLoggingHandlerRequestHandlerAsync2<TRequest> : RequestHandlerAsync<TRequest> where TRequest : class, IRequest
    {
        private HandlerTiming _timing;


        public override void InitializeFromAttributeParams(params object[] initializerList)
        {
            _timing = (HandlerTiming)initializerList[0];
        }


        public override async Task<TRequest> HandleAsync(TRequest command, CancellationToken? ct = null)
        {
            LogCommand(command);
            return await base.HandleAsync(command, ct).ConfigureAwait(ContinueOnCapturedContext);
        }


        public override async Task<TRequest> FallbackAsync(TRequest command, CancellationToken? ct = null)
        {
            LogFailure(command);
            return await base.FallbackAsync(command, ct).ConfigureAwait(ContinueOnCapturedContext);
        }

        private void LogCommand(TRequest request)
        {
            //TODO: LibLog has no async support, so remains a blocking call for now
            logger.InfoFormat("Logging handler pipeline call. Pipeline timing {0} target, for {1} with values of {2} at: {3}", _timing.ToString(), typeof(TRequest), JsonConvert.SerializeObject(request), DateTime.UtcNow);
        }

        private void LogFailure(TRequest request)
        {
            //TODO: LibLog has no async support, so remains a blocking call for now
            logger.InfoFormat("Failure in pipeline call for {0} with values of {1} at: {2}", typeof(TRequest), JsonConvert.SerializeObject(request), DateTime.UtcNow);
        }
    }
}