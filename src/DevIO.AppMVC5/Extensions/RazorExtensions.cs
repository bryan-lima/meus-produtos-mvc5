using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevIO.AppMVC5.Extensions
{
    public static class RazorExtensions
    {
        public static bool PermitirExecucao(this WebViewPage page, string claimName, string claimValue)
        {
            return CustomAuthorization.ValidarClaimsUsuario(claimName, claimValue);
        }

        public static MvcHtmlString PermitirExecucao(this MvcHtmlString value, string claimName, string claimValue)
        {
            return CustomAuthorization.ValidarClaimsUsuario(claimName, claimValue) ? value : MvcHtmlString.Empty;
        }

        public static string ActionComPermissao(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues, string claimName, string claimValue)
        {
            return CustomAuthorization.ValidarClaimsUsuario(claimName, claimValue) ? urlHelper.Action(actionName, controllerName, routeValues) : "";
        }

        public static string FormatarDocumento(this WebViewPage page, int tipoPessoa, string documento)
        {
            return tipoPessoa == 1
                ? Convert.ToUInt64(documento).ToString(@"000\.000\.000\-00")
                : Convert.ToUInt64(documento).ToString(@"00\.000\.000\/0000\-00");
        }

        public static bool ExibirNaURL(this WebViewPage page, Guid id)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var urlTarget = urlHelper.Action("Edit", "Fornecedores", new { id = id });
            var urlTarget2 = urlHelper.Action("ObterEndereco", "Fornecedores", new { id = id });

            var urlEmUso = HttpContext.Current.Request.Path;

            return urlTarget == urlEmUso || urlTarget2 == urlEmUso;
        }
    }
}