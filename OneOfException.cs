using System;
namespace ForgetJson;
enum ReturnCode {
    OK, Exception,
    InputFileNotFound,
    InvalidJson,
    UnforseenException,
    Tbd5, Tbd6, Tbd7, Tbd8, Tbd9,
    OptionNotImplemented = -1,
}
[Serializable]
class OneOfException : Exception {
    public ReturnCode Code { get; private set; }
    public OneOfException(ReturnCode code, Exception inner = null, string message = null)
    : base(message, inner)
    {
        Code = code;
        WriteError(code, message);
    }
    static internal int WriteError(ReturnCode code, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ResetColor();
        return (int)code;
    }
}