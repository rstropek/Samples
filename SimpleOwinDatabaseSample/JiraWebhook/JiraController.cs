using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace JiraWebhook
{
	public class JiraController : ApiController
	{
		[Route("jirawebhook")]
		[HttpPost]
		public async Task<IHttpActionResult> JiraPostAsync([FromBody]JObject jsonPayload)
		{
			// For details about Jira Webhooks see https://developer.atlassian.com/display/JIRADEV/JIRA+Webhooks+Overview

			// For demo purposes, we only handle creation of Jira tickets here
			if (jsonPayload["webhookEvent"] != null && jsonPayload["webhookEvent"].ToString() == "jira:issue_created"
				&& jsonPayload["issue"] != null)
			{
				var issue = jsonPayload["issue"]["fields"];
				using (var client = new HttpClient())
				{
					// In this sample we use basic authentication 
					// (see http://www.visualstudio.com/en-us/integrate/get-started/get-started-auth-introduction-vsi);
					// in practice you should use OAuth2 instead
					// (see http://www.visualstudio.com/en-us/integrate/get-started/get-started-auth-oauth2-vsi).
					var tfsCredentials = ConfigurationManager.ConnectionStrings["TfsConnection"].ConnectionString;
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
						"Basic",
						Convert.ToBase64String(Encoding.ASCII.GetBytes(tfsCredentials)));

					// For details about VSO REST API 
					// see http://www.visualstudio.com/en-us/integrate/reference/reference-vso-overview-vsi
					const string tenant = "rainerdemotfs-westeu.visualstudio.com";
					const string project = "oop";
					const string workItemType = "Product Backlog Item";
					const string url = "https://" + tenant + "/defaultcollection/" + project + "/_apis/wit/workitems/$" + workItemType + "?api-version=1.0";
					const string fieldOperation = "add";

					// Build payload for creating a new product backlog item
					var payload = new List<object>() {
						// New work items always have the state "New"
						new { op = fieldOperation, path = "/fields/System.State", value = "New" }
					};

					// Set some properties in VSO to demonstrate the general workflow
					if (issue["summary"] != null)
					{
						payload.Add(new { op = fieldOperation, path = "/fields/System.Title", value = issue["summary"].ToString() });
					}

					if (issue["description"] != null)
					{
						payload.Add(new { op = fieldOperation, path = "/fields/System.Description", value = issue["description"].ToString() });
					}

					if (issue["assignee"] != null && issue["assignee"]["displayName"] != null)
					{
						payload.Add(new { op = fieldOperation, path = "/fields/System.AssignedTo", value = issue["assignee"]["displayName"].ToString() });
					}

					// Send the REST request to VSO
					var method = new HttpMethod("PATCH");
					var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json-patch+json");
					using (var request = new HttpRequestMessage(method, url) { Content = content })
					{
						using (var response = await client.SendAsync(request))
						{
							// Write result to trace log for troubleshooting purposes
							Trace.WriteLine(response.StatusCode);
							Trace.WriteLine(await response.Content.ReadAsStringAsync());
						}
					}
				}
			}

			return this.Ok();
		}
	}
}