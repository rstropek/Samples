ErrorKind err;

// Note mix int variable declaration and assignment in a deconstruction (C# 10)
(int result, err) = DoSomethingThatMightFail(42);
(result, err) = DoSomethingThatMightFail(41);
if (err != ErrorKind.NoError)
{
    Console.WriteLine(err);
}

// Note deconstruction of default (C# vNext)
// Not yet available in VS, see https://sharplab.io/#gist:75c4ff0b34b9550d6f99ff694addda50 instead
/*
(int result2, ErrorKind err2) = default;
(result2, err2) = DoSomethingThatMightFail(41);
if (err != ErrorKind.NoError)
{
    Console.WriteLine(err);
}
*/

(int result, ErrorKind err) DoSomethingThatMightFail(int parameter)
{
    // Note use of default in the implementation
    if (parameter >= 42) { return (42, default); };
    return (default, ErrorKind.NotImplemented);
}

enum ErrorKind
{
    NoError,
    GeneralError,
    NotImplemented,
    InvalidState,
}
