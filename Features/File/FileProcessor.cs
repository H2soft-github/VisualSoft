using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using VisualSoft.Features.File.Models;
using FileResult = VisualSoft.Features.File.Models.FileResult;

namespace VisualSoft.Features.File
{
    public class FileProcessor(IFormFile file, int xPosDiscriminator)
    {
        private readonly string PARAMETER_SEPARATOR = ",";
        private readonly int ANSI = 1252;

        public async Task<FileProcessorResult> ProcessFile()
        {
            RegisterEncodingProvider();
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.GetEncoding(ANSI));
            var fileResult = new FileResult();
            var fileProcessorResult = new FileProcessorResult();
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                try
                {
                    if (line != null)
                    {
                        var recordType = GetRecordType(line);
                        SimpleStatistic(fileResult, line, recordType);
                        switch (recordType)
                        {
                            case RecordType.Document:
                                AddDocument(fileResult, line);
                                break;
                            case RecordType.DocumentComment:
                                AddComment(fileResult, line);
                                break;
                            case RecordType.Position:
                                AddPosition(fileResult, line);
                                break;
                        }
                    }
                }
                catch
                {
                    fileProcessorResult.ValidationEnum = ValidationEnum.InvalidFormat;
                    fileProcessorResult.ValidationMessage = $"Invalid format in line {fileResult.LineCount}.";
                    return fileProcessorResult;
                }
            }
            ComplexStatistics(fileResult);
            fileProcessorResult.ValidationEnum = ValidationEnum.Ok;
            fileProcessorResult.FileResult = fileResult;
            return fileProcessorResult;
        }

        private void SimpleStatistic(FileResult fileResult, string line, RecordType recordType)
        {
            fileResult.CharCount += line!.Count();
            fileResult.LineCount++;
            if (recordType == RecordType.Position)
            {
                fileResult.Sum++;
            }
        }

        private void ComplexStatistics(FileResult fileResult)
        {
            XCount(fileResult);
            ProductsWithMaxNetValue(fileResult);
        }

        private void XCount(FileResult fileResult)
        {
            fileResult.Xcount = fileResult.Documents!
                .Where(x => x.PositionDetails.Count() > xPosDiscriminator).Count();
        }

        private void ProductsWithMaxNetValue(FileResult fileResult)
        {
            var positions = fileResult.Documents!.SelectMany(x => x.PositionDetails);
            var max = positions.Max(x => x.NetValue);
            fileResult.ProductsWithMaxNetValue = String.Join(
                ",", positions.Where(x => x.NetValue == max)
                .OrderBy(x => x.ProductName)
                .Select(x => x.ProductName)
                .Distinct().ToArray());   
        }

        private RecordType GetRecordType(string line)
        {
            return line.Substring(0, 1) switch
            {
                "C" => RecordType.DocumentComment,
                "B" => RecordType.Position,
                "H" => RecordType.Document,
                _ => throw new Exception("Unrecognized record type.")
            };
        }

        private void AddDocument(FileResult fileResult, string line)
        {
            var documentDetails = new DocumentDetails();
            var values = line.Split(PARAMETER_SEPARATOR);
            documentDetails.CodeBA = values[1];
            documentDetails.Type = values[2];
            documentDetails.DocumentNumber = values[3];
            documentDetails.OperationData = DateTime.ParseExact(values[4],
                "dd-MM-yyyy", CultureInfo.InvariantCulture);
            documentDetails.DocumentDayNumber = Convert.ToInt32(values[5]);
            documentDetails.ContractCode = values[6];
            documentDetails.ContractName = values[7];
            documentDetails.ExternalDocumentNumber = values[8];
            documentDetails.ExternalDocumentDate = DateTime.ParseExact(values[9],
                "dd-MM-yyyy", CultureInfo.InvariantCulture);
            documentDetails.Net = decimal.Parse(values[10], CultureInfo.InvariantCulture);
            documentDetails.Vat = decimal.Parse(values[11], CultureInfo.InvariantCulture);
            documentDetails.Brutto = decimal.Parse(values[12], CultureInfo.InvariantCulture); ;
            documentDetails.F1 = decimal.Parse(values[13], CultureInfo.InvariantCulture);
            documentDetails.F2 = decimal.Parse(values[14], CultureInfo.InvariantCulture);
            documentDetails.F3 = decimal.Parse(values[15], CultureInfo.InvariantCulture);
            (fileResult.Documents as List<DocumentDetails>)!.Add(documentDetails);
        }

        private void AddComment(FileResult fileResult, string line)
        {
            var documentDetails = fileResult.Documents!.LastOrDefault();
            if (documentDetails == null)
            {
                (fileResult.Documents as List<DocumentDetails>)!.Add(new DocumentDetails());
                documentDetails = fileResult.Documents!.LastOrDefault();
            }
            documentDetails!.CommentDetails = new CommentDetails();
            var commentDetails = documentDetails.CommentDetails;
            var values = line.Split(PARAMETER_SEPARATOR);
            commentDetails.Comment = values[1];
            commentDetails.SomeNumber = values[2];
        }

        private void AddPosition(FileResult fileResult, string line)
        {
            var documentDetails = fileResult.Documents!.LastOrDefault();
            if (documentDetails == null)
            {
                throw new Exception("It is not possible to add items because there are no documents.");
            }
            var position = new PositionDetails();
            (documentDetails.PositionDetails as List<PositionDetails>)!.Add(position);
            var values = line.Split(PARAMETER_SEPARATOR);
            position.ProductCode = values[1];
            position.ProductName = values[2];
            position.Quantity = decimal.Parse(values[3], CultureInfo.InvariantCulture);
            position.NetPrice = decimal.Parse(values[4], CultureInfo.InvariantCulture);
            position.NetValue = decimal.Parse(values[5], CultureInfo.InvariantCulture);
            position.VatValue = decimal.Parse(values[6], CultureInfo.InvariantCulture);
            position.QuantityBefore = decimal.Parse(values[7], CultureInfo.InvariantCulture);
            position.AvgBefore = decimal.Parse(values[8], CultureInfo.InvariantCulture);
            position.QuantityAfter = decimal.Parse(values[9], CultureInfo.InvariantCulture);
            position.AvgAfter = decimal.Parse(values[10], CultureInfo.InvariantCulture);
            position.Group = values[11];
        }

        private void RegisterEncodingProvider()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
