using Carter;
using VisualSoft.Features.File;

namespace VisualSoft.Modules
{
    public class TestModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("test/{x:int}", async (int x, HttpRequest request) =>
            {
                ValidationEnum validation = new FileValidator(request).Validate();
                if (validation == ValidationEnum.Ok)
                {
                    var fileProcessorResult = await new FileProcessor(request.Form.Files.First(), x).ProcessFile();
                    if (fileProcessorResult.ValidationEnum == ValidationEnum.Ok)
                    {
                        return Results.Ok(fileProcessorResult.FileResult);
                    }
                    else
                    {

                        return Results.BadRequest(ValidationToMessage(fileProcessorResult.ValidationEnum,
                            fileProcessorResult.ValidationMessage!));
                    }
                }
                else
                {
                    return Results.BadRequest(ValidationToMessage(validation, string.Empty));
                }
            });
        }

        private string ValidationToMessage(ValidationEnum validationEnum, string message)
        {
            var messageInternal = validationEnum switch
            {
                ValidationEnum.NoFile => "No file.",
                ValidationEnum.ManyFiles => "Too many files.",
                ValidationEnum.Extension => "Invalid extensions (PUT only).",
                ValidationEnum.InvalidFormat => "Invalid format.",
                _ => throw new Exception($"No message for code {validationEnum}"),
            };
            return messageInternal + (!string.IsNullOrEmpty(message) ? " " + message : string.Empty);
        }
    }
}
