namespace Planly.Application.Validation
{
	/// <summary>
	/// Represents a logical error in a request.
	/// </summary>
	/// <param name="Code">The codename of the error.</param>
	/// <param name="Message">A human-readable message that explains the error.</param>
	/// <param name="Target">The name of the field in the request that was invalid.</param>
	public record RequestValidationError(string Code, string? Message = null, string? Target = null);
}