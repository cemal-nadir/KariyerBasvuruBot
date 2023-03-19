using System;
using System.Collections.Generic;
using System.Linq;

namespace Browser.Helpers
{
    public static class ExceptionHelper
    {
        private enum ErrorType
        {
            Browser,
            Block,
            Critical,
            Unavailable
        }
        private static readonly Dictionary<ErrorType, List<string>> Errors = new Dictionary<ErrorType, List<string>>() {

            {
                ErrorType.Browser,
                new List<string>()
                {
                    "unknown error: net::ERR_NAME_NOT_RESOLVED",
                    "Beklendi Sayfa Yüklenmedi !",
                    "no such element: Unable to locate element",
                    "unknown error: net::ERR_CONNECTION_TIMED_OUT",
                    "element click intercepted",
                    "javascript error: $ is not defined"
                }
            },
            {
                ErrorType.Block,
                new List<string>(){
                    "Blocked"
                }
            },
            {
                ErrorType.Critical,
                new List<string>()
                {
                    "Critical"
                }
            },
            {
                ErrorType.Unavailable,
                new List<string>()
                {
                    "Unavailable"
                }
            }

        };

        public static bool IsBrowserError(this Exception exception) => Errors[ErrorType.Browser].Any(x => exception.Message.Contains(x));
        public static bool IsBlockedError(this Exception exception) => Errors[ErrorType.Block].Any(x => exception.Message.Contains(x));
        public static bool IsCriticalError(this Exception exception) => Errors[ErrorType.Critical].Any(x => exception.Message.Contains(x));
        public static bool IsUnavailableError(this Exception exception) => Errors[ErrorType.Unavailable].Any(x => exception.Message.Contains(x));
    }
}
