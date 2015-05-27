namespace Ematig_Portal.Domain.Interface
{
    using System;

    public interface ILoggingService
    {
        void Log(Exception exception);
    }
}
