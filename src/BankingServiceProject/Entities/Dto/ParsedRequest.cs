namespace BankingServiceProject.Entities.Dto;

public record ParsedRequest(
    Dictionary<string, string> Headers,
    string Body);