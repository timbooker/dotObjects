using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using dotObjects.Core.Processing;
using dotObjects.Core.UI;
using dotObjects.Web.Rendering.Configuration;
using dotObjects.Web.Rendering.Helpers;
using dotObjects.Web.Rendering.WebForms;
using Microsoft.Office.Interop.Excel;

namespace dotObjects.Web.Rendering
{
    public class HybridRenderer : WebFormsRenderer
    {
        private const int FileNameLength = 250;
        private const string OutputPath = "OutputPath";
        private const string ExcelExtension = ".xlsx";
        private const string PdfExtension = ".pdf";

        public String FileName { get; set; }

        public override void Process(IProcessResponse response, TextWriter writer)
        {
            if (UrlHelper.CurrentExtension.Equals(UrlHelper.ExcelInteropExtension) || UrlHelper.CurrentExtension.Equals(UrlHelper.PdfExtension))
            {
                Template template = GetTemplate(response);
                if (template != null)
                    ProcessTemplate(writer, template.FileName, response, null);
            }
            else
                base.Process(response, writer);
        }

        public override void ProcessTemplate(TextWriter writer, string templateName, IProcessResponse response, Domain domain)
        {
            if ((UrlHelper.CurrentExtension.Equals(UrlHelper.ExcelInteropExtension) || UrlHelper.CurrentExtension.Equals(UrlHelper.PdfExtension)) && response is QueryProcessResponse)
            {
                if (templateName.EndsWith(ExcelExtension))
                {
                    SetFileName(response);
                    var filePath = CreateExcelFile(templateName);
                    if (UrlHelper.CurrentExtension.Equals(UrlHelper.ExcelInteropExtension))
                    {
                        LoadExcelData(filePath, (QueryProcessResponse)response);
                        HttpContext.Current.Response.WriteFile(filePath);
                    }
                    else
                    {
                        LoadPdfData(filePath, (QueryProcessResponse)response);
                        HttpContext.Current.Response.WriteFile(filePath.Replace(ExcelExtension, PdfExtension));
                    }
                }
            }
            else
                base.ProcessTemplate(writer, templateName, response, domain);
        }

        private string CreateExcelFile(string templateName)
        {
            var sourceFilePath = System.IO.Path.Combine(AppHelper.BaseApplicationPath, System.IO.Path.Combine(RenderingSection.Current.Path, templateName));
            var destinationPath = System.IO.Path.Combine(AppHelper.BaseApplicationPath, String.Format(ConfigurationManager.AppSettings[OutputPath], DateTime.Now));
            var filePath = System.IO.Path.Combine(destinationPath, string.Concat(FileName, ExcelExtension));

            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.Copy(sourceFilePath, filePath);
            File.SetAttributes(filePath, FileAttributes.Archive);

            return filePath;
        }

        private void SetFileName(IProcessResponse response)
        {
            FileName = response.URI.Entity;
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff");
            if (FileName.Length > (FileNameLength - timeStamp.Length - 1))
                FileName = FileName.Substring(0, (FileNameLength - timeStamp.Length - 1));
            FileName = string.Concat(FileName, " ", timeStamp);
        }

        internal static void LoadPdfData(string templateDestinationPath, QueryProcessResponse queryProcessResponse)
        {
            var application = new Application
                                  {
                                      Visible = false,
                                      DisplayAlerts = false,
                                      WindowState = XlWindowState.xlMinimized
                                  };


            var workbook = application.Workbooks.Open(templateDestinationPath,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value);

            LoadWorkbookData(workbook, queryProcessResponse);

            workbook.ExportAsFixedFormat(
                XlFixedFormatType.xlTypePDF,
                templateDestinationPath.Replace("xlsx", "pdf"),
                XlFixedFormatQuality.xlQualityStandard,
                true,
                true,
                Missing.Value,
                Missing.Value,
                false,
                Missing.Value);

            workbook.Save();
            //workbook.SaveCopyAs(templateDestinationPath);
            workbook.Close(true, templateDestinationPath, null);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);

            application.Quit();
        }

        internal static void LoadExcelData(string templateDestinationPath, QueryProcessResponse queryProcessResponse)
        {
            var application = new Application
                                  {
                                      Visible = false,
                                      DisplayAlerts = false,
                                      WindowState = XlWindowState.xlMinimized
                                  };


            var workbook = application.Workbooks.Open(templateDestinationPath,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value);

            LoadWorkbookData(workbook, queryProcessResponse);

            //workbook.Save();
            workbook.Close(true, templateDestinationPath, null);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);

            application.Quit();
        }

        private static void LoadWorkbookData(Workbook workbook, QueryProcessResponse queryProcessResponse)
        {
            var dataSource = queryProcessResponse.Items.ToList();
            ManageWorkbook(workbook, dataSource);
            var deleteSheetList = new List<int>();
            for (int sheetNumber = workbook.Sheets.Count; sheetNumber > 0; sheetNumber--)
            {
                var worksheet = workbook.Sheets[sheetNumber] as Worksheet;
                if (worksheet != null)
                {
                    var checkWhereRange = worksheet.Cells[1, 3] as Range;
                    var whereRange = worksheet.Cells[2, 3] as Range;

                    if (checkWhereRange != null)
                    {
                        var checkWhere = checkWhereRange.Value as string;
                        if (whereRange != null)
                        {
                            var where = whereRange.Value as string;

                            checkWhereRange.EntireRow.Delete(Type.Missing);
                            whereRange.EntireRow.Delete(Type.Missing);

                            var checkWhereCount = dataSource.Count(i => i.Match(checkWhere));

                            if (checkWhereCount.Equals(0))
                                deleteSheetList.Add(sheetNumber);
                            else
                            {
                                var sheetDataSource = dataSource.Where(i => i.Match(@where)).ToList();
                                if (sheetDataSource.Count.Equals(0))
                                    deleteSheetList.Add(sheetNumber);
                                else
                                {
                                    foreach (var domain in sheetDataSource)
                                        dataSource.Remove(domain);

                                    var infoCellList = GetInfoCellList(worksheet);
                                    var dynamicColumns = CreateDynamicColumns(worksheet, infoCellList,
                                                                              sheetDataSource[0].Domains.Where(
                                                                                  x => x.CanRead && x.Visible).ToList());

                                    if (dynamicColumns)
                                        infoCellList = GetInfoCellList(worksheet);

                                    FillFirstInfoCell(worksheet, infoCellList, sheetDataSource.First());
                                    FillItemInfoCell(worksheet, infoCellList, sheetDataSource, dynamicColumns);
                                }
                            }
                        }
                    }
                }
            }

            if (deleteSheetList.Count.Equals(workbook.Sheets.Count))
            {
                workbook.Sheets.Add(Type.Missing, workbook.Sheets[workbook.Sheets.Count], Type.Missing, Type.Missing);
            }

            foreach (var index in deleteSheetList)
            {
                var deleteWorksheet = workbook.Sheets[index] as Worksheet;
                if (deleteWorksheet != null) deleteWorksheet.Delete();
            }

            for (int sheetNumber = workbook.Sheets.Count; sheetNumber > 0; sheetNumber--)
            {
                var worksheet = workbook.Sheets[sheetNumber] as Worksheet;
                if (worksheet != null)
                {
                    worksheet.Name = string.Format("Sheet {0}", sheetNumber);
                    worksheet.Activate();
                }
            }
        }

        private static void ManageWorkbook(Workbook workbook, List<Domain> dataSource)
        {
            var newSheetList = new List<Dictionary<int, string[]>>();
            var deleteSheetList = new List<int>();

            for (int sheetNumber = 1; sheetNumber <= workbook.Sheets.Count; sheetNumber++)
            {
                var worksheet = workbook.Sheets[sheetNumber] as Worksheet;

                if (worksheet != null)
                {
                    var checkWhereRange = worksheet.Cells[1, 3] as Range;
                    var whereRange = worksheet.Cells[2, 3] as Range;
                    var groupByRange = worksheet.Cells[3, 3] as Range;

                    if (checkWhereRange != null)
                    {
                        var checkWhere = checkWhereRange.Value as string;
                        if (whereRange != null)
                        {
                            var where = whereRange.Value as string;
                            if (groupByRange != null)
                            {
                                var groupBy = groupByRange.Value as string;
                                groupByRange.EntireRow.Delete(Type.Missing);

                                checkWhere = string.IsNullOrEmpty(checkWhere) ? @where : checkWhere;

                                if (!string.IsNullOrEmpty(groupBy))
                                {
                                    var groupByColumns = groupBy.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                                    if (!groupByColumns.Length.Equals(0))
                                    {
                                        var groupByValues = dataSource.GroupByMany(groupByColumns);

                                        var whereList = GetWhereList(groupByValues, groupByColumns, 0);

                                        foreach (var groupWhere in whereList)
                                        {
                                            newSheetList.Add(new Dictionary<int, string[]>());
                                            newSheetList[newSheetList.Count - 1].Add(
                                                sheetNumber,
                                                new[] 
                                                    {
                                                        string.Concat(string.IsNullOrEmpty(checkWhere) ? string.Empty : "And/", groupWhere, checkWhere),
                                                        string.Concat(string.IsNullOrEmpty(@where) ? string.Empty : "And/", groupWhere, @where)
                                                    }
                                                );
                                        }

                                        deleteSheetList.Add(sheetNumber);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var item in newSheetList)
            {
                foreach (var newSheet in item)
                {
                    var baseWorksheet = workbook.Sheets[newSheet.Key] as Worksheet;
                    var lastWorksheet = workbook.Sheets[workbook.Sheets.Count] as Worksheet;

                    if (baseWorksheet != null) baseWorksheet.Copy(Type.Missing, lastWorksheet);

                    var newWorksheet = workbook.Sheets[workbook.Sheets.Count] as Worksheet;

                    if (newWorksheet != null)
                    {
                        var checkWhereRange = newWorksheet.Cells[1, 3] as Range;
                        var whereRange = newWorksheet.Cells[2, 3] as Range;

                        if (checkWhereRange != null) checkWhereRange.Value = newSheet.Value[0];
                        if (whereRange != null) whereRange.Value = newSheet.Value[1];
                    }
                }
            }

            for (var index = deleteSheetList.Count - 1; index >= 0; index--)
            {
                var deleteWorksheet = workbook.Sheets[deleteSheetList[index]] as Worksheet;
                if (deleteWorksheet != null) deleteWorksheet.Delete();
            }
        }

        private static List<string> GetWhereList(IEnumerable<GroupResult> groupByValues, string[] groupByColumns, int index)
        {
            var whereList = new List<string>();

            if (groupByValues != null)
            {
                foreach (var group in groupByValues)
                {
                    if (group.Key != null)
                    {
                        var where = string.Format("Equals/{0}/{1}/", groupByColumns[index], group.Key);

                        var subWhereList = GetWhereList(group.SubGroups, groupByColumns, index + 1);

                        if (subWhereList.Count.Equals(0))
                        {
                            whereList.Add(where);
                        }
                        else
                        {
                            whereList.AddRange(subWhereList.Select(subWhere => string.Concat("And/", @where, subWhere)));
                        }
                    }
                }
            }

            return whereList;
        }

        private static bool CreateDynamicColumns(Worksheet worksheet, List<InfoCell> infoCellList, List<Domain> domains)
        {
            var columnInfoCellList = infoCellList.Where(x => x.SheetNumber.Equals(worksheet.Index) && x.IsDynamic && x.Value.StartsWith("Column")).ToList();

            if (!columnInfoCellList.Count.Equals(0))
            {
                foreach (var infoCell in columnInfoCellList)
                {
                    var index = 0;

                    var sourceRange = worksheet.Cells[infoCell.RowNumber, infoCell.ColNumber] as Range;
                    var destinationRange = worksheet.Range[worksheet.Cells[infoCell.RowNumber, infoCell.ColNumber + 1], worksheet.Cells[infoCell.RowNumber, infoCell.ColNumber + domains.Count() - 1]];

                    if (sourceRange != null) sourceRange.Copy(destinationRange);

                    foreach (var domain in domains)
                    {
                        var cell = worksheet.Cells[infoCell.RowNumber, infoCell.ColNumber + index++] as Range;

                        SetFormat(cell, domain);

                        if (infoCell.Value.EndsWith("Header"))
                        {
                            if (cell != null) cell.Value2 = domain.Label;
                        }
                        else if (infoCell.Value.EndsWith("FieldName"))
                        {
                            if (cell != null) cell.Value2 = string.Concat("{Item.", domain.ID, "}");
                        }
                        else if (infoCell.Value.EndsWith("Footer"))
                        {
                            if (cell != null)
                            {
                                cell.Value2 = "	"; //TAB

                                if ((domain.Format != null) && (domain.Format.Summarize))
                                    SetSummarizeField(cell, infoCell, index);
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        private static void SetFormat(Range cell, Domain domain)
        {
            if (domain.IsNumeric)
            {
                if (domain.ID.Equals("Id"))
                {
                    cell.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    cell.NumberFormat = "{1}0{0}_);({1}0{0})";

                    SetNumericFormat(cell, domain, 0);
                }
                else
                {
                    if (domain.IsFraction)
                    {
                        cell.HorizontalAlignment = XlHAlign.xlHAlignRight;
                        cell.NumberFormat = "{1}#,##0{0}_);({1}#,##0{0})";

                        SetNumericFormat(cell, domain, 2);
                    }
                    else
                    {
                        cell.HorizontalAlignment = XlHAlign.xlHAlignRight;
                        cell.NumberFormat = "{1}#,##0{0}_);({1}#,##0{0})";

                        SetNumericFormat(cell, domain, 0);
                    }
                }
            }
            else if (domain.TypeName.Equals("DateTime"))
            {
                cell.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                cell.NumberFormat = "MM/dd/yyyy";
            }
        }

        private static void SetNumericFormat(Range cell, Domain domain, int doublePlaces)
        {
            var currencyFormat = string.Empty;

            if (domain.Format != null)
            {
                var number = string.Join(null, Regex.Split(domain.Format.GetFirstExpressionFormat(domain), "[^\\d]"));

                if (!string.IsNullOrEmpty(number))
                    doublePlaces = Int32.Parse(number);

                if (domain.Parent != null)
                {
                    var valueList = domain.Format.GetEvaluatedParameters(domain);

                    if ((!valueList.Length.Equals(0)) && (!string.IsNullOrEmpty(valueList[valueList.Length - 1].ToString())))
                        currencyFormat = string.Concat("[$", valueList[valueList.Length - 1], "] ");
                }
            }

            var doubleFormat = string.Concat(doublePlaces.Equals(0) ? string.Empty : ".", string.Empty.PadLeft(doublePlaces, '0'));

            cell.NumberFormat = string.Format(cell.NumberFormat, doubleFormat, currencyFormat);
        }

        private static void SetSummarizeField(Range cell, InfoCell infoCell, int index)
        {
            var sumInfoCell = new InfoCell(cell.Worksheet.Index, infoCell.ColNumber + index - 1, infoCell.RowNumber - 1, "=SUM(${0}${1}:${2}{3})", false);

            cell.Value2 = string.Empty;

            cell.Formula = string.Format(sumInfoCell.Value, sumInfoCell.ColString, sumInfoCell.RowNumber, sumInfoCell.ColString, sumInfoCell.RowNumber);

            cell.Calculate();
        }

        private static void FillFirstInfoCell(Worksheet worksheet, List<InfoCell> infoCellList, Domain firstDomain)
        {
            var firstInfoCellList = infoCellList.Where(x => x.SheetNumber.Equals(worksheet.Index) && x.IsDynamic && x.Value.StartsWith("First")).ToList();

            if (!firstInfoCellList.Count.Equals(0))
            {
                foreach (var infoCell in firstInfoCellList)
                {
                    var cell = worksheet.Cells[infoCell.RowNumber, infoCell.ColNumber] as Range;

                    if (cell != null) cell.Value2 = GetValue(cell, firstDomain, infoCell);
                }
            }
        }

        private static void FillItemInfoCell(Worksheet worksheet, List<InfoCell> infoCellList, List<Domain> domains, bool dynamicColumns)
        {
            var itemInfoCellList = infoCellList.Where(x => x.SheetNumber.Equals(worksheet.Index) && x.IsDynamic && x.Value.StartsWith("Item")).ToList();

            if (!itemInfoCellList.Count.Equals(0))
            {
                MoveRange(worksheet, infoCellList, itemInfoCellList.First().RowNumber, domains.Count);

                FormatRange(worksheet, itemInfoCellList.First(), itemInfoCellList.Last(), domains.Count);

                FillRange(worksheet, itemInfoCellList, domains, dynamicColumns);
            }

            if (dynamicColumns)
            {
                foreach (var infoCell in itemInfoCellList)
                {
                    var cell = worksheet.Cells[infoCell.RowNumber, infoCell.ColNumber] as Range;

                    if (cell != null) cell.ColumnWidth = cell.Resize.ColumnWidth + 2;
                }
            }
        }

        private static void MoveRange(Worksheet worksheet, List<InfoCell> infoCellList, int startRowNumber, int rowCount)
        {
            var movableInfoCellList = infoCellList.Where(x => x.SheetNumber.Equals(worksheet.Index) && x.RowNumber > startRowNumber).ToList();

            if (rowCount > 1)
            {
                for (var index = movableInfoCellList.Count - 1; index >= 0; index--)
                {
                    var sourceRange = worksheet.Cells[movableInfoCellList[index].RowNumber, movableInfoCellList[index].ColNumber] as Range;
                    var destinationRange = worksheet.Cells[movableInfoCellList[index].RowNumber + rowCount - 1, movableInfoCellList[index].ColNumber] as Range;

                    if (sourceRange != null)
                    {
                        sourceRange.Copy(destinationRange);

                        if (destinationRange != null) destinationRange.RowHeight = sourceRange.Resize.RowHeight;

                        sourceRange.Clear();
                        sourceRange.ClearFormats();
                        sourceRange.ClearContents();
                    }
                }
            }
        }

        private static void FormatRange(Worksheet worksheet, InfoCell firstInfo, InfoCell lastInfo, int rowCount)
        {
            if (rowCount > 1)
            {
                var sourceRange = worksheet.Range[worksheet.Cells[firstInfo.RowNumber, firstInfo.ColNumber], worksheet.Cells[lastInfo.RowNumber, lastInfo.ColNumber]];
                var destinationRange = worksheet.Range[worksheet.Cells[firstInfo.RowNumber + 1, firstInfo.ColNumber], worksheet.Cells[lastInfo.RowNumber + rowCount - 1, lastInfo.ColNumber]];

                sourceRange.Copy(destinationRange);

                destinationRange.RowHeight = sourceRange.Resize.RowHeight;
            }
        }

        private static void FillRange(Worksheet worksheet, List<InfoCell> itemInfoCellList, List<Domain> domainList, bool dynamicColumns)
        {
            for (var index = 0; index < domainList.Count; index++)
            {
                foreach (var infoCell in itemInfoCellList)
                {
                    var cell = worksheet.Cells[infoCell.RowNumber + index, infoCell.ColNumber] as Range;

                    if (cell != null)
                    {
                        cell.Value2 = GetValue(cell, domainList[index], infoCell);

                        if (dynamicColumns)
                            cell.EntireColumn.AutoFit();
                    }
                }
            }
        }

        private static object GetValue(Range cell, Domain domain, InfoCell infoCell)
        {
            var childDomain = domain.GetChildDomain(infoCell.DomainId);

            if (childDomain != null)
            {
                SetFormat(cell, childDomain);

                if (childDomain.Value != null)
                {
                    if (childDomain is EntityDomain)
                    {
                        return childDomain.ObjectValue.ToString();
                    }
                    if (childDomain.Type.IsEnum)
                    {
                        return childDomain.Value.ToString();
                    }
                    if (childDomain.IsNumeric && childDomain.Format != null && childDomain.Value != null)
                        return string.Format(string.Concat("{0:", childDomain.Format.GetFirstExpressionFormat(childDomain), "}"), childDomain.Value);

                    return childDomain.Value;
                }
                if (childDomain.Format != null)
                {
                    var arrayList = new System.Collections.ArrayList(childDomain.Format.GetEvaluatedParameters(childDomain));
                    arrayList.Insert(0, !(childDomain is EntityDomain) ? childDomain.Value : string.Empty);
                    return string.Format(childDomain.Format.Expression, arrayList.ToArray());
                }
            }

            return string.Empty;
        }

        #region .: InfoCell .

        private static List<InfoCell> GetInfoCellList(Worksheet worksheet)
        {
            var infoCellList = new List<InfoCell>();

            var excelRange = worksheet.UsedRange;
            var valueArray = (object[,])excelRange.Value[XlRangeValueDataType.xlRangeValueDefault];

            if (valueArray != null)
            {
                var cell = excelRange.Item[1, 1] as Range;

                if (cell != null)
                {
                    var firstRow = cell.Row - 1;
                    var firstColumn = cell.Column - 1;

                    if (valueArray.Rank.Equals(2))
                    {
                        for (var rowNumber = 1; rowNumber <= valueArray.GetUpperBound(0); rowNumber++)
                        {
                            for (var colNumber = 1; colNumber <= valueArray.GetUpperBound(1); colNumber++)
                            {
                                var value = valueArray[rowNumber, colNumber];

                                if (value != null)
                                {
                                    var matchs = Regex.Matches(value.ToString(), @"(\{)(.*?)(\})");

                                    infoCellList.Add(matchs.Count.Equals(1)
                                                         ? new InfoCell(worksheet.Index, colNumber + firstColumn,
                                                                        rowNumber + firstRow, matchs[0].Groups[2].Value,
                                                                        true)
                                                         : new InfoCell(worksheet.Index, colNumber + firstColumn,
                                                                        rowNumber + firstRow, value.ToString(), false));
                                }
                            }
                        }
                    }
                }
            }

            return infoCellList;
        }

        private class InfoCell
        {
            public InfoCell(int sheetNumber, int colNumber, int rowNumber, string value, bool isDynamic)
            {
                SheetNumber = sheetNumber;
                ColNumber = colNumber;
                RowNumber = rowNumber;
                Value = value;
                IsDynamic = isDynamic;
            }

            public int SheetNumber { get; private set; }

            public int ColNumber { get; private set; }

            public string ColString { get { return Convert.ToChar(64 + ColNumber).ToString(CultureInfo.InvariantCulture); } }

            public int RowNumber { get; private set; }

            public string Value { get; private set; }

            public string DomainId { get { return Value.Substring(Value.IndexOf('.') + 1); } }

            public bool IsDynamic { get; private set; }

            public override string ToString()
            {
                return string.Format("[{0}]{1}{2}: {3}", SheetNumber, ColString, RowNumber, Value);
            }
        }

        #endregion
    }
}