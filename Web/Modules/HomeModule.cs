using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

using InstaSharp;
using InstaSharp.Models.Responses;

using Nancy;
using Nancy.Session;

using Newtonsoft.Json;

namespace Blinnikov.LikeMeUp.Web.Modules {
    public class HomeModule : NancyModule {
        private const string LocalhostIp = "127.0.0.1";
        private const string SessionKey = "InstaSharp.AuthInfo";
        private readonly InstagramConfig config;
        public HomeModule() {
            config = this.CreateConfig();
            this.Get["/", true] = this.HomeView();
            this.Get["/authdone", true] = this.AuthDone();
        }

        private Func<dynamic, CancellationToken, Task<dynamic>> HomeView() {
            return async (_, token) => {
                var oauthResponse = this.GetResponseFromSession();
                if (oauthResponse != null) {
                    return await this.MakeRequest(oauthResponse);
                }

                var link = this.CreateAuthLink();
                return Response.AsRedirect(link);
            };
        }

        private async Task<dynamic> MakeRequest(OAuthResponse oauthResponse) {
            var tags = new InstaSharp.Endpoints.Tags(config, oauthResponse);
            tags.EnableEnforceSignedHeader(LocalhostIp);
            var likes = new InstaSharp.Endpoints.Likes(config, oauthResponse);
            likes.EnableEnforceSignedHeader(LocalhostIp);
            var photos = await tags.Recent("likeforlike");
            foreach (var photo in photos.Data) {
                if (!(photo.UserHasLiked ?? false)) {
                    var likeResult = await likes.Post(photo.Id);
                }
            }

            ////var users = new InstaSharp.Endpoints.Users(config, oauthResponse);
            ////users.EnableEnforceSignedHeader(LocalhostIp);
            ////var feed = await users.GetSelf();
            return this.View["index"];
        }

        private Func<dynamic, CancellationToken, Task<dynamic>> AuthDone() {
            return async (_, token) => {
                var code = Request.Query.code;
                
                // add this code to the auth object
                var auth = new OAuth(config);

                // now we have to call back to instagram and include the code they gave us
                // along with our client secret
                var oauthResponse = await auth.RequestToken(code);

                // both the client secret and the token are considered sensitive data, so we won't be
                // sending them back to the browser. we'll only store them temporarily.  If a user's session times
                // out, they will have to click on the authenticate button again - sorry bout yer luck.
                this.Session[SessionKey] = JsonConvert.SerializeObject(oauthResponse);

                // all done, lets redirect to the home controller which will send some intial data to the app
                return Response.AsRedirect("/");
            };
        }

        private InstagramConfig CreateConfig() {
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            var redirectUri = ConfigurationManager.AppSettings["RedirectUri"];

            return new InstagramConfig(clientId, clientSecret, redirectUri);
        }

        private string CreateAuthLink() {
            var scopes = new List<OAuth.Scope>();
            scopes.Add(OAuth.Scope.Likes);
            scopes.Add(OAuth.Scope.Comments);

            return OAuth.AuthLink(config.OAuthUri + "/authorize", config.ClientId, config.RedirectUri, scopes, OAuth.ResponseType.Code);
        }

        private OAuthResponse GetResponseFromSession() {
            OAuthResponse oauthResponse = null;
            var sessionValue = this.Session[SessionKey] as string;
            if (!string.IsNullOrEmpty(sessionValue)) {
                oauthResponse = JsonConvert.DeserializeObject<OAuthResponse>(sessionValue);
            }

            return oauthResponse;
        }
    }
}