/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.AspNet.Builder;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Host.Config;
using Owin;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;

namespace HostvNext
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Map("/core", core =>
            {
                var factory = InMemoryFactory.Create(
                                        users: Users.Get(),
                                        clients: Clients.Get(),
                                        scopes: Scopes.Get());

                var idsrvOptions = new IdentityServerOptions
                {
                    IssuerUri = "https://idsrv3.com",
                    SiteName = "Thinktecture IdentityServer v3 - beta 3",
                    Factory = factory,
                    SigningCertificate = Cert.Load(),
                    RequireSsl = false,

                    CorsPolicy = CorsPolicy.AllowAll,
                    CspOptions = new CspOptions
                    {
                        ReportEndpoint = EndpointSettings.Enabled,
                    },

                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders,
                    }
                };

                core.UseIdentityServer(idsrvOptions);
            });
        }

        public static void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Google",
                SignInAsAuthenticationType = signInAsType,

                ClientId = "767400843187-8boio83mb57ruogr9af9ut09fkg56b27.apps.googleusercontent.com",
                ClientSecret = "5fWcBT0udKY7_b6E3gEiJlze"
            };
            app.UseGoogleAuthentication(google);

            var aad = new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "aad",
                Caption = "Azure AD",
                SignInAsAuthenticationType = signInAsType,

                Authority = "https://login.windows.net/4ca9cb4c-5e5f-4be9-b700-c532992a3705",
                ClientId = "65bbbda8-8b85-4c9d-81e9-1502330aacba",
                RedirectUri = "https://localhost:44333/core/aadcb"
            };

            app.UseOpenIdConnectAuthentication(aad);
        }

    }
}
