using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BIRA_Issue_Tracker.Extensions
{
	/*                                                                     *
	*      This extension was derive from Brad Christie's answer           *
	*      on StackOverflow.                                               *
	*                                                                      *
	*      The original code can be found at:                              *
	*      http://stackoverflow.com/a/18338264/998328                      *
	*                                                                      *
	*      Modified by Housey                                              *
	*                                                                      */

	public static class NotificationExtensions
	{
		private static readonly IDictionary<NotificationType, string> NotificationKey = new Dictionary
			<NotificationType, string>
			{
				{NotificationType.Error, "App.Notifications.Error"},
				{NotificationType.Warning, "App.Notifications.Warning"},
				{NotificationType.Success, "App.Notifications.Success"},
				{NotificationType.Info, "App.Notifications.Info"}
			};


		public static void AddNotification(this ControllerBase controller, string message, NotificationType notificationType)
		{
			var notificationKeyByType = GetNotificationKeyByType(notificationType);
			var messages = controller.TempData[notificationKeyByType] as ICollection<string>;

			if (messages == null)
			{
				controller.TempData[notificationKeyByType] = messages = new HashSet<string>();
			}

			messages.Add(message);
		}

		public static IEnumerable<string> GetNotifications(this HtmlHelper htmlHelper, NotificationType notificationType)
		{
			string notificationKeyByType = GetNotificationKeyByType(notificationType);
			return htmlHelper.ViewContext.Controller.TempData[notificationKeyByType] as ICollection<string>;
		}

		private static string GetNotificationKeyByType(NotificationType notificationType)
		{
			try
			{
				return NotificationKey[notificationType];
			}
			catch (IndexOutOfRangeException e)
			{
				ArgumentException exception = new ArgumentException("Key is invalid", nameof(notificationType), e);
				throw exception;
			}
		}
	}

	public enum NotificationType
	{
		Error,
		Warning,
		Success,
		Info
	}
}