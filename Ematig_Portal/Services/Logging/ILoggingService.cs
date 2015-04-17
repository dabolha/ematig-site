namespace Ematig_Portal.Services
{
    using System;

    public interface ILoggingService
    {
        void Log(Exception exception);
    }
}
