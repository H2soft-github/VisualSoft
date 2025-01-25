using Microsoft.AspNetCore.Http.HttpResults;

namespace VisualSoft.Features.File
{
    public enum ValidationEnum
    {
        Ok,
        NoFile,
        Extension,
        ManyFiles,
        InvalidFormat
    }
}
