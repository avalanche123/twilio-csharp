﻿#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using RestSharp;
using RestSharp.Extensions;
using RestSharp.Validation;


namespace Twilio
{
	public partial class TwilioRestClient
	{
		/// <summary>
		/// Retrieve the details for an incoming phone number. Makes a GET request to a IncomingPhoneNumber instance resource.
		/// </summary>
		/// <param name="incomingPhoneNumberSid">The Sid of the number to retrieve</param>
		/// <param name="callback">Method to call upon successful completion</param>
		public void GetIncomingPhoneNumber(string incomingPhoneNumberSid, Action<IncomingPhoneNumber> callback)
		{
			var request = new RestRequest();
			request.Resource = "Accounts/{AccountSid}/IncomingPhoneNumbers/{IncomingPhoneNumberSid}";
			request.RootElement = "IncomingPhoneNumber";

			request.AddParameter("IncomingPhoneNumberSid", incomingPhoneNumberSid, ParameterType.UrlSegment);

			ExecuteAsync<IncomingPhoneNumber>(request, (response) => callback(response));
		}

		/// <summary>
		/// List all incoming phone numbers on current account. Makes a GET request to the IncomingPhoneNumber List resource.
		/// </summary>
		/// <param name="callback">Method to call upon successful completion</param>
		public void ListIncomingPhoneNumbers(Action<IncomingPhoneNumberResult> callback)
		{
			ListIncomingPhoneNumbers(null, null, null, null, callback);
		}

		/// <summary>
		/// List incoming phone numbers on current account with filters. Makes a GET request to the IncomingPhoneNumber List resource.
		/// </summary>
		/// <param name="phoneNumber">Optional phone number to match</param>
		/// <param name="friendlyName">Optional friendly name to match</param>
		/// <param name="pageNumber">Page number to start retrieving results from</param>
		/// <param name="count">How many results to return</param>
		/// <param name="callback">Method to call upon successful completion</param>
		public void ListIncomingPhoneNumbers(string phoneNumber, string friendlyName, int? pageNumber, int? count, Action<IncomingPhoneNumberResult> callback)
		{
			var request = new RestRequest();
			request.Resource = "Accounts/{AccountSid}/IncomingPhoneNumbers";

			if (phoneNumber.HasValue()) request.AddParameter("PhoneNumber", phoneNumber);
			if (friendlyName.HasValue()) request.AddParameter("FriendlyName", friendlyName);

			if (pageNumber.HasValue) request.AddParameter("Page", pageNumber.Value);
			if (count.HasValue) request.AddParameter("PageSize", count.Value);

			ExecuteAsync<IncomingPhoneNumberResult>(request, (response) => callback(response));
		}

		/// <summary>
		/// Purchase/provision a local phone number. Makes a POST request to the IncomingPhoneNumber List resource.
		/// </summary>
		/// <param name="options">Optional parameters to use when purchasing number</param>
		/// <param name="callback">Method to call upon successful completion</param>
		public void AddIncomingPhoneNumber(PhoneNumberOptions options, Action<IncomingPhoneNumber> callback)
		{
			var request = new RestRequest(Method.POST);
			request.Resource = "Accounts/{AccountSid}/IncomingPhoneNumbers";
			request.RootElement = "IncomingPhoneNumber";

			if (options.PhoneNumber.HasValue())
			{
				request.AddParameter("PhoneNumber", options.PhoneNumber);
			}
			else
			{
				if (options.AreaCode.HasValue()) request.AddParameter("AreaCode", options.AreaCode);
			}

			AddPhoneNumberOptionsToRequest(request, options);
			AddSmsOptionsToRequest(request, options);

			ExecuteAsync<IncomingPhoneNumber>(request, (response) => callback(response));
		}

		/// <summary>
		/// Update the settings of an incoming phone number. Makes a POST request to an IncomingPhoneNumber Instance resource.
		/// </summary>
		/// <param name="incomingPhoneNumberSid">The Sid of the phone number to update</param>
		/// <param name="options">Which settings to update. Only properties with values set will be updated.</param>
		/// <param name="callback">Method to call upon successful completion</param>
		public void UpdateIncomingPhoneNumber(string incomingPhoneNumberSid, PhoneNumberOptions options, Action<IncomingPhoneNumber> callback)
		{
			Require.Argument("IncomingPhoneNumberSid", incomingPhoneNumberSid);

			var request = new RestRequest(Method.POST);
			request.Resource = "Accounts/{AccountSid}/IncomingPhoneNumbers/{IncomingPhoneNumberSid}";
			request.RootElement = "IncomingPhoneNumber";

			request.AddParameter("IncomingPhoneNumberSid", incomingPhoneNumberSid, ParameterType.UrlSegment);
			AddPhoneNumberOptionsToRequest(request, options);
			AddSmsOptionsToRequest(request, options);

			ExecuteAsync<IncomingPhoneNumber>(request, (response) => callback(response));
		}

		/// <summary>
		/// Remove (deprovision) a phone number from the current account. Makes a DELETE request to an IncomingPhoneNumber Instance resource.
		/// </summary>
		/// <param name="incomingPhoneNumberSid">The Sid of the number to remove</param>
		/// <param name="callback">Method to call upon successful completion</param>
		public void DeleteIncomingPhoneNumber(string incomingPhoneNumberSid, Action<DeleteStatus> callback)
		{
			Require.Argument("IncomingPhoneNumberSid", incomingPhoneNumberSid);
			var request = new RestRequest(Method.DELETE);
			request.Resource = "Accounts/{AccountSid}/IncomingPhoneNumbers/{IncomingPhoneNumberSid}";

			request.AddParameter("IncomingPhoneNumberSid", incomingPhoneNumberSid, ParameterType.UrlSegment);

			ExecuteAsync(request, (response) => { callback(response.StatusCode == System.Net.HttpStatusCode.NoContent ? DeleteStatus.Success : DeleteStatus.Failed); });
		}

		private void AddPhoneNumberOptionsToRequest(RestRequest request, PhoneNumberOptions options)
		{
			if (options.FriendlyName.HasValue())
			{
				Validate.IsValidLength(options.FriendlyName, 64);
				request.AddParameter("FriendlyName", options.FriendlyName);
			}
			// some check for null. in those cases an empty string is a valid value (to remove a URL assignment)
			if (options.VoiceApplicationSid != null) request.AddParameter("VoiceApplicationSid", options.VoiceApplicationSid);

			if (options.VoiceUrl != null) request.AddParameter("VoiceUrl", options.VoiceUrl);
			if (options.VoiceMethod.HasValue()) request.AddParameter("VoiceMethod", options.VoiceMethod.ToString());
			if (options.VoiceFallbackUrl != null) request.AddParameter("VoiceFallbackUrl", options.VoiceFallbackUrl);
			if (options.VoiceFallbackMethod.HasValue()) request.AddParameter("VoiceFallbackMethod", options.VoiceFallbackMethod.ToString());
			if (options.VoiceCallerIdLookup.HasValue) request.AddParameter("VoiceCallerIdLookup", options.VoiceCallerIdLookup.Value);
			if (options.StatusCallback.HasValue()) request.AddParameter("StatusCallbackUrl", options.StatusCallback);
			if (options.StatusCallbackMethod.HasValue()) request.AddParameter("StatusCallbackMethod", options.StatusCallbackMethod.ToString());
		}

		private void AddSmsOptionsToRequest(RestRequest request, PhoneNumberOptions options)
		{
			// some check for null. in those cases an empty string is a valid value (to remove a URL assignment)
			if (options.SmsApplicationSid != null) request.AddParameter("SmsApplicationSid", options.SmsApplicationSid);
			if (options.SmsUrl != null) request.AddParameter("SmsUrl", options.SmsUrl);
			if (options.SmsMethod.HasValue()) request.AddParameter("SmsMethod", options.SmsMethod.ToString());
			if (options.SmsFallbackUrl != null) request.AddParameter("SmsFallbackUrl", options.SmsFallbackUrl);
			if (options.SmsFallbackMethod.HasValue()) request.AddParameter("SmsFallbackMethod", options.SmsFallbackMethod.ToString());
		}
	}
}