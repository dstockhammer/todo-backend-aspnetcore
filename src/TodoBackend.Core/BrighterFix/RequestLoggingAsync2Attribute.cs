using System;
using paramore.brighter.commandprocessor;

namespace TodoBackend.Core.BrighterFix
{
    public class RequestLoggingAsync2Attribute : RequestHandlerAttribute
    {
        public RequestLoggingAsync2Attribute(int step, HandlerTiming timing)
            : base(step, timing)
        { }

        public override object[] InitializerParams()
        {
            return new object[] { Timing };
        }

        public override Type GetHandlerType()
        {
            return typeof(RequestLoggingHandlerRequestHandlerAsync2<>);
        }
    }
}
