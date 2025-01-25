namespace VisualSoft.Features.File
{
    public class FileValidator(HttpRequest request)
    {
        public ValidationEnum Validate()
        {
            if (NoFile())
            {
                return ValidationEnum.NoFile;
            } else
            {
                if (OneFile())
                {
                    if (IsCorrectExtension())
                    {
                        return ValidationEnum.Ok;
                    }
                    else
                    {
                        return ValidationEnum.Extension;
                    }
                }
                else
                {
                    return ValidationEnum.ManyFiles;
                }
            }
        }

        public bool NoFile()
        {
           return (request.Form == null ||
                request.Form.Files == null ||
                request.Form.Files.Count == 0) ;
        }

        public bool OneFile()
        {
            return (request.Form != null &&
                request.Form.Files != null && 
                request.Form.Files.Count == 1);
        }

        public bool IsCorrectExtension()
        {
            return (request.Form != null &&
                request.Form.Files != null &&
                Path.GetExtension(request.Form.Files[0].FileName) == ".PUR");

        }
    }
}
