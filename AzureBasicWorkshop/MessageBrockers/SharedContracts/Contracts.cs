namespace Contracts;

public sealed record MessageEnvelope(
	string Source,
	string EventType,
	string Payload,
	DateTimeOffset CreatedAt,
	string CorrelationId);

public sealed record SendMessageRequest(
	string Payload,
	string? EventType,
	string? CorrelationId);
