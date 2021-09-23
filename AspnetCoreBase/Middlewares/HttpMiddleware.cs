using AspnetCoreBase.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspnetCoreBase.Middlewares
{
    public static class HttpMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestResponseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpMiddleware>();
        }
    }

    public class HttpMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await responseInterceptor(context);
        }

        private async Task responseInterceptor(HttpContext context)
        {
            var resultJson = new ApiResponseModel();
            var responseStream = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;
                    await _next(context); // Wait for response

                    memStream.Position = 0;
                    string jsonString;
                    using (var streamReader = new StreamReader(memStream))
                    {
                        jsonString = streamReader.ReadToEnd();
                    }

                    try
                    {
                        if (context.Response.StatusCode == 401)
                        {
                            resultJson.Message = "No Permission";
                            resultJson.StatusCode = 401;
                        }
                        else if (context.Response.StatusCode != 200 && jsonString == string.Empty)
                        {
                            resultJson.Message = "Not Found";
                            resultJson.StatusCode = 404;
                        }
                        else
                        {
                            resultJson.StatusCode = context.Response.StatusCode;

                            string oldmessage = resultJson.Message;
                            resultJson.Message = jsonString;

                            var result = JToken.Parse(jsonString);

                            resultJson.Data = result;

                            resultJson.Message = oldmessage;
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        resultJson.StatusCode = 404;
                        resultJson.Message = "The Service is not Availale";
                    }
                    finally
                    {
                        using (var stream = generateStreamFromString(JsonConvert.SerializeObject(resultJson)))
                        {
                            await stream.CopyToAsync(responseStream);
                        }
                    }
                }
            }
            finally
            {
                context.Response.Body = responseStream;
            }
        }

        private Stream generateStreamFromString(string s)
        {
            try
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(s);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}