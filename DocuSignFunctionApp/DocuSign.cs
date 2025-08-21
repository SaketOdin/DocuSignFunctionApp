using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using DocuSign.CodeExamples.Authentication;
using DocuSign.CodeExamples.Common;
using DocuSign.eSign.Client;
using ESignature.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;
using UserInfo = DocuSign.eSign.Client.Auth.OAuth.UserInfo;

namespace DocuSignFunctionApp
{
    public  class CustomDocuSign
    {
        public readonly string DevCenterPage = "https://developers.docusign.com/platform/auth/consent";
        public void InvokeDocusign(XDocument xmlDoc,string signerEmail, string signerName, string ccEmail,string ccName,string clientId, string ImpersonatedUserId, string AuthServer ,byte[] PrivateKeybyte, string blobConnectionString, string blobContainerName)
        {
            Console.ForegroundColor = ConsoleColor.White;
            OAuthToken accessToken = null;
            try
            {
                accessToken = JwtAuth.AuthenticateWithJwt("ESignature", clientId, ImpersonatedUserId, AuthServer, PrivateKeybyte);
            }
            catch (ApiException apiExp)
            {
                // Consent for impersonation must be obtained to use JWT Grant
                if (apiExp.Message.Contains("consent_required"))
                {
                    // Caret needed for escaping & in windows URL
                    string caret = "";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        caret = "^";
                    }

                    // build a URL to provide consent for this Integration Key and this userId
                    string url = "https://" + ConfigurationManager.AppSettings["AuthServer"] + "/oauth/auth?response_type=code" + caret + "&scope=impersonation%20signature" + caret +
                        "&client_id=" + ConfigurationManager.AppSettings["ClientId"] + caret + "&redirect_uri=" + DevCenterPage;

                    string consentRequiredMessage = $"Consent is required - launching browser (URL is {url})";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        consentRequiredMessage = consentRequiredMessage.Replace(caret, "");
                    }



                    // Start new browser window for login and consent to this app by DocuSign user
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = false });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }

                    
                }
            }
            
            var docuSignClient = new DocuSignClient();
            docuSignClient.SetOAuthBasePath(ConfigurationManager.AppSettings["AuthServer"]);
            UserInfo userInfo = docuSignClient.GetUserInfo(accessToken.access_token);
            Account acct = userInfo.Accounts.FirstOrDefault();

            try
            {

                string envelopeId = SigningViaEmail.SendEnvelopeViaEmail(xmlDoc, signerEmail, signerName, ccEmail, ccName, accessToken.access_token, acct.BaseUri + "/restapi", acct.AccountId, blobConnectionString, blobContainerName, "sent");
            }
            catch (Exception ex)
            {

                throw new Exception($"Error while sending envelope: {ex.Message}", ex);
            }
                Console.ForegroundColor = ConsoleColor.White;
            

        }
    }
}
