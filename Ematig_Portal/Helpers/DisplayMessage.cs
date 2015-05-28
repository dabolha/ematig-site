using Ematig_Portal.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ematig_Portal.Helpers
{
    public enum DisplayMessageType
    {
        Success,
        Information,
        Warning,
        Error
    }

    public class DisplayMessage
    {
        public TempDataDictionary TempData { get; set; }

        public DisplayMessage()
        {
            TempData = new TempDataDictionary();
        }

        public DisplayMessage(TempDataDictionary tempData)
        {
            TempData = tempData;
        }

        public void Success(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Success, message, dismissable);
        }

        public void Information(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Information, message, dismissable);
        }

        public void Warning(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Warning, message, dismissable);
        }

        public void Error(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Danger, message, dismissable);
        }

        private void AddAlert(string alertStyle, string message, bool dismissable)
        {
            var alerts = TempData.ContainsKey(Alert.TempDataKey)
                ? (List<Alert>)TempData[Alert.TempDataKey]
                : new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable
            });

            TempData[Alert.TempDataKey] = alerts;
        }

        public void Add(DisplayMessageType displayMessageType, string message)
        {
            switch (displayMessageType)
            {
                case DisplayMessageType.Error:
                    Error(message);
                    break;

                case DisplayMessageType.Information:
                    Information(message);
                    break;

                case DisplayMessageType.Warning:
                    Warning(message);
                    break;

                case DisplayMessageType.Success:
                    Success(message);
                    break;

            }
        }
    }
}