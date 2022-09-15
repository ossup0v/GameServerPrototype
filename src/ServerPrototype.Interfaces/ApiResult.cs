﻿using Orleans.Concurrency;
using System.Net;

namespace ServerPrototype.Interfaces
{
    [Immutable]
    public class ApiResult
    {
        public ApiResult()
        {

        }

        public ApiResult(HttpStatusCode status, string message = null)
        {
            Status = status;
            Message = message;
        }

        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;

        public bool IsOK => Status == HttpStatusCode.OK;

        public bool IsFailed => Status != HttpStatusCode.OK;

        public string Message { get; set; }

        public static ApiResult OK { get; } = new ApiResult(HttpStatusCode.OK);

        public static ApiResult BadRequest { get; } = new ApiResult(HttpStatusCode.BadRequest);

        public static ApiResult NotModified { get; } = new ApiResult(HttpStatusCode.NotModified);

        public static ApiResult InternalError { get; } = new ApiResult(HttpStatusCode.InternalServerError);
    }

    [Immutable]
    public class ApiResult<T> : ApiResult
    {
        public ApiResult() : base(HttpStatusCode.BadRequest)
        {

        }

        public ApiResult(HttpStatusCode status, string message = null) : base(status, message)
        {

        }

        public ApiResult(T value) : base(HttpStatusCode.OK)
        {
            Value = value;
        }

        public ApiResult(T value, HttpStatusCode status, string message = null) : base(status, message)
        {
            Value = value;
        }


        public T Value { get; set; }

        public static ApiResult<T> OK(T value) { return new ApiResult<T>(value, HttpStatusCode.OK); }

        public static ApiResult<T> BadRequest(string msg = null) { return new ApiResult<T>(HttpStatusCode.BadRequest, msg); }

        public static ApiResult<T> InternalError(string msg = null) { return new ApiResult<T>(HttpStatusCode.InternalServerError, msg); }
    }
}