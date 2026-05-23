using FluentResults;

namespace Registration;

public class BadRequest(string message) : Error(message);

public class Concurrency(string message) : Error(message);

public class Forbidden(string message) : Error(message);

public class NotFound(string message) : Error(message);

