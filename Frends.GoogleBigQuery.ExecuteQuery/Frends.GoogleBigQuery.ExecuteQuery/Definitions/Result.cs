namespace Frends.GoogleBigQuery.ExecuteQuery.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Result as JToken.
    /// </summary>
    /// <example>
    /// {
    ///   "integer_field": 1,
    ///   "string_field": "Sample String",
    ///   "date_field": "2023-09-20T00:00:00",
    ///   "float_field": 3.14,
    ///   "boolean_field": true,
    ///   "timestamp_field": "2023-09-20T12:34:56Z",
    ///   "time_field": "12:34:56.7890000",
    ///   "datetime_field": "2023-09-20T12:34:56.789",
    ///   "record_field": {
    ///     "nested_string": "Nested String"
    ///   },
    ///   "array_field": [
    ///     "Item 1",
    ///     "Item 2"
    ///   ],
    ///   "name": "John Smith",
    ///   "age": 30
    /// }
    /// </example>
    public dynamic Data { get; private set; }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Error occured...</example>
    public string ErrorMessage { get; private set; }

    internal Result(bool success, dynamic data, string errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }
}