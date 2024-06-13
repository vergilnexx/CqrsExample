using FluentValidation;
using Grpc.Core;
using Meta.Common.Contracts.Exceptions.Common;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Meta.Common.Hosts.Grpc.Features.AppFeatures.ExceptionHandler
{
    /// <summary>
    /// Helper для обработки исключений
    /// </summary>
    public static class ExceptionHelpers
    {
        private const string LogTemplate = "GRPC {Host} {RequestMethod} ответ: {StatusCode}";

        /// <summary>
        /// Обработка.
        /// </summary>
        /// <typeparam name="T">Тип.</typeparam>
        /// <param name="exception">Исключение.</param>
        /// <param name="context">Контекст.</param>
        /// <param name="logger">Логгер.</param>
        /// <param name="correlationId">Идентификатор.</param>
        /// <returns></returns>
        public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            if(TryGetException<UnauthorizedAccessException>(exception, out var unauthorizedAccessException))
            {
                return HandleUnauthorizedAccessException(unauthorizedAccessException!, context, logger, correlationId);
            }
            if (TryGetException<ReadableException>(exception, out var readableException))
            {
                return HandleReadableException(readableException!, context, logger, correlationId);
            }
            if (TryGetException<OperationCanceledException>(exception, out var operationCanceledException))
            {
                return HandleOperationCanceledException(operationCanceledException!, context, logger, correlationId);
            }
            if (TryGetException<ValidationException>(exception, out var validationException))
            {
                return HandleValidationException(validationException!, context, logger, correlationId);
            }
            if (TryGetException<TimeoutException>(exception, out var timeoutException))
            {
                return HandleTimeoutException(timeoutException!, context, logger, correlationId);
            }
            if (TryGetException<RpcException>(exception, out var rpcException))
            {
                return HandleRpcException(rpcException!, context, logger, correlationId);
            }

            return HandleDefault(exception, context, logger, correlationId);
        }

        private static RpcException HandleReadableException<T>(
            ReadableException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            LogWithRequestParams(context, () =>
            {
                logger.LogError(exception, "При выполнении запроса '{CorrelationId}' произошла ошибка.", correlationId);
            }, correlationId);

            var status = new Status(GetStatusCode(exception), exception.Message, exception);

            return new RpcException(status, CreateTrailers(correlationId));
        }

        private static RpcException HandleOperationCanceledException<T>(
            OperationCanceledException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            LogWithRequestParams(context, () =>
            {
                logger.LogError(exception, "При выполнении запроса '{CorrelationId}' выполнение операции было прервано.", correlationId);
            }, correlationId);

            var status = new Status(GetStatusCode(exception), "Выполнение операции было прервано.", exception);

            return new RpcException(status, CreateTrailers(correlationId));
        }

        private static RpcException HandleUnauthorizedAccessException<T>(
            UnauthorizedAccessException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            LogWithRequestParams(context, () =>
            {
                logger.LogError(exception, "При выполнении запроса '{CorrelationId}' произошла ошибка проверки авторизации.", correlationId);
            }, correlationId);

            var status = new Status(GetStatusCode(exception), "Ошибка проверки авторизации.", exception);

            return new RpcException(status, CreateTrailers(correlationId));
        }

        private static RpcException HandleValidationException<T>(ValidationException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            LogWithRequestParams(context, () =>
            {
                logger.LogError(exception, "При выполнении запроса '{CorrelationId}' произошла ошибка валидации переданных данных.", correlationId);
            }, correlationId);

            var status = new Status(GetStatusCode(exception), "Переданные параметры переданы с ошибкой.", exception);

            return new RpcException(status, CreateTrailers(correlationId));
        }

        private static RpcException HandleTimeoutException<T>(TimeoutException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            LogWithRequestParams(context, () =>
            {
                logger.LogError(exception, "При выполнении запроса '{CorrelationId}' произошла ошибка времени выполнения.", correlationId);
            }, correlationId);

            var status = new Status(GetStatusCode(exception), "Ресурс не ответил в отведенное время", exception);

            return new RpcException(status, CreateTrailers(correlationId));
        }

        private static RpcException HandleRpcException<T>(RpcException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            LogWithRequestParams(context, () =>
            {
                logger.LogError(exception, "При выполнении запроса '{CorrelationId}' произошла необрабатываемая ошибка.", correlationId);
            }, correlationId);

            var metadata = CreateTrailers(correlationId).FirstOrDefault();
            var trailers = exception.Trailers;

            if (metadata is not null)
            {
                trailers.Add(metadata);
            }

            return new RpcException(new Status(exception.StatusCode, exception.Message, exception), trailers);
        }

        private static void LogWithRequestParams(ServerCallContext context, Action logging, Guid correlationId)
        {
            using (LogContext.PushProperty("Request.TraceIdentifier", correlationId))
            using (LogContext.PushProperty("Request.DisplayUrl", context.Host))
            {
                logging();
            }
        }

        private static RpcException HandleDefault<T>(Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            var statusCode = GetStatusCode(exception);
            LogWithRequestParams(context, () =>
            {
                using (LogContext.PushProperty("Request.UserIdentityName", context.AuthContext?.PeerIdentity?.FirstOrDefault()?.Name ?? string.Empty))
                using (LogContext.PushProperty("Request.ConnectionIpAddress", context.Peer))
                using (LogContext.PushProperty("Request.Headers", context.RequestHeaders, destructureObjects: true))
                {
                    logger.LogError(exception, LogTemplate, context.Host, context.Method, statusCode);
                }
            }, correlationId);

            return new RpcException(new Status(statusCode, exception.Message, exception), CreateTrailers(correlationId));
        }

        private static StatusCode GetStatusCode(Exception exception) => exception switch
        {
            UnauthorizedAccessException _ => StatusCode.Unauthenticated,
            ValidationException _ => StatusCode.InvalidArgument,
            OperationCanceledException _ => StatusCode.Cancelled,
            _ => StatusCode.Internal,
        };

        /// <summary>
        /// Добавление идентификатора в метаданные.
        /// </summary>
        /// <param name="correlationId">Идентификатор.</param>
        /// <returns>Метаданные</returns>
        private static Metadata CreateTrailers(Guid correlationId)
        {
            var trailers = new Metadata
            {
                { "CorrelationId", correlationId.ToString() }
            };
            return trailers;
        }

        private static bool TryGetException<TException>(Exception source, out TException? exception)
            where TException : Exception
        {
            exception = null;
            var current = source;
            while (current != null)
            {
                exception = current as TException;
                if (exception != null)
                {
                    return true;
                }

                current = current.InnerException;
            }

            return false;
        }
    }
}
