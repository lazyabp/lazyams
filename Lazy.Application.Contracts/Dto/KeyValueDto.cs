namespace Lazy.Application.Contracts.Dto;

public class KeyValueDto : KeyValueDto<string>
{
    public KeyValueDto()
    {
    }

    public KeyValueDto(string value, string label, string description = "") : base(value, label, description)
    {
    }
}

public class KeyValueDto<T> where T : class
{
    public T Value { get; set; }

    public string Label { get; set; }

    public string Description { get; set; }

    public KeyValueDto()
    {        
    }

    public KeyValueDto(T value, string label, string description)
    {
        Value = value;
        Label = label;
        Description = description;
    }
}