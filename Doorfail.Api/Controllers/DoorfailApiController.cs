using Doorfail.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace Doorfail.Api.Controllers
{
    //Debug.WriteLine(Reflection.CurrentMethodName);
    public abstract class DoorfailApiController : ApiController
    {
        private ServerVariableModel GetServerVariableModel(HttpRequestMessage request) => new ServerVariableModel(((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.ServerVariables);

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            GetServerVariableModel(controllerContext.Request).Insert();
            return base.ExecuteAsync(controllerContext, cancellationToken);
        }

        protected override BadRequestResult BadRequest()
        {
            return base.BadRequest();
        }

        protected override BadRequestErrorMessageResult BadRequest(string message)
        {
            return base.BadRequest(message);
        }

        protected override InvalidModelStateResult BadRequest(ModelStateDictionary modelState)
        {
            return base.BadRequest(modelState);
        }

        protected override ConflictResult Conflict()
        {
            return base.Conflict();
        }

        protected override NegotiatedContentResult<T> Content<T>(HttpStatusCode statusCode, T value)
        {
            return base.Content(statusCode, value);
        }

        protected override FormattedContentResult<T> Content<T>(HttpStatusCode statusCode, T value, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType)
        {
            return base.Content(statusCode, value, formatter, mediaType);
        }

        protected override CreatedNegotiatedContentResult<T> Created<T>(Uri location, T content)
        {
            return base.Created(location, content);
        }

        protected override CreatedAtRouteNegotiatedContentResult<T> CreatedAtRoute<T>(string routeName, IDictionary<string, object> routeValues, T content)
        {
            return base.CreatedAtRoute(routeName, routeValues, content);
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            GetServerVariableModel(controllerContext.Request).Insert();
            base.Initialize(controllerContext);
        }

        protected override InternalServerErrorResult InternalServerError()
        {
            return base.InternalServerError();
        }

        protected override ExceptionResult InternalServerError(Exception exception)
        {
            return base.InternalServerError(exception);
        }

        protected override JsonResult<T> Json<T>(T content, JsonSerializerSettings serializerSettings, Encoding encoding)
        {
            return base.Json(content, serializerSettings, encoding);
        }

        protected override NotFoundResult NotFound()
        {
            return base.NotFound();
        }

        protected override OkResult Ok()
        {
            return base.Ok();
        }

        protected override OkNegotiatedContentResult<T> Ok<T>(T content)
        {
            return base.Ok(content);
        }

        protected override RedirectResult Redirect(string location)
        {
            return base.Redirect(location);
        }

        protected override RedirectResult Redirect(Uri location)
        {
            return base.Redirect(location);
        }

        protected override RedirectToRouteResult RedirectToRoute(string routeName, IDictionary<string, object> routeValues)
        {
            return base.RedirectToRoute(routeName, routeValues);
        }

        protected override ResponseMessageResult ResponseMessage(HttpResponseMessage response)
        {
            return base.ResponseMessage(response);
        }

        protected override StatusCodeResult StatusCode(HttpStatusCode status)
        {
            return base.StatusCode(status);
        }

        protected override UnauthorizedResult Unauthorized(IEnumerable<AuthenticationHeaderValue> challenges)
        {
            return base.Unauthorized(challenges);
        }
    }
}